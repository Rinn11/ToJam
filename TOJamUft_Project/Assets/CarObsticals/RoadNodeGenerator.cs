using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

[ExecuteAlways]
public class LaneGenerator : MonoBehaviour
{
    public float tileSpacing = 10f;
    public int nodesPerLane = 5;
    public float nodeSpacing = 2f;
    public float laneOffset = 1.5f;
    public float connectThreshold = 20;

    public bool visualizeGizmos = true;

    public string exportPath = "Assets/Lanes";

    public Dictionary<string, LanePath> laneSegments = new();
    private HashSet<string> junctionSegmentNames = new();

    private Vector3 RoundVec(Vector3 v) => new(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

    [ContextMenu("Generate + Link + Export Lanes")]
    public void GenerateAndLinkLanes()
    {
        GenerateLanes();

        var allSegments = laneSegments.Values.ToList();
        LinkLaneSegments(allSegments);

        ExportCombinedPaths(allSegments);
    }

    public void GenerateLanes()
    {
        laneSegments.Clear();
        junctionSegmentNames.Clear();

        List<Transform> tiles = new();
        foreach (Transform child in transform)
            tiles.Add(child);

        if (tiles.Count < 1) return;

        tileSpacing = tiles.Count >= 2 ? Vector3.Distance(tiles[0].position, tiles[1].position) : 10f;

        foreach (Transform tile in tiles)
        {
            Vector3 current = RoundVec(tile.position);
            GenerateLanesForTile(current, tile.name, tiles);
        }

        Debug.Log($"Lane generation complete. Total: {laneSegments.Count}");
    }

    private void GenerateLanesForTile(Vector3 center, string tileName, List<Transform> tiles)
    {
        Vector3[] dirs = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
        List<Vector3> connected = new();
        foreach (var dir in dirs)
        {
            Vector3 check = center + dir * tileSpacing;
            if (tiles.Any(t => RoundVec(t.position) == RoundVec(check)))
                connected.Add(dir);
        }

        bool isStraight = connected.Count == 2 && Mathf.Approximately(Vector3.Dot(connected[0], connected[1]), -1);
        bool isBend = connected.Count == 2 && Mathf.Approximately(Vector3.Dot(connected[0], connected[1]), 0);
        bool isJunction = connected.Count >= 3;

        if (!isStraight && !isBend && !isJunction)
            return;

        List<Vector3> fwd = new();
        List<Vector3> back = new();

        if (isStraight)
        {
            Vector3 dir = connected[0];
            Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
            float half = (nodesPerLane - 1) / 2f;

            for (int i = 0; i < nodesPerLane; i++)
            {
                float offset = (i - half) * nodeSpacing;
                fwd.Add(center + right * laneOffset + dir * offset);
                back.Add(center - right * laneOffset - dir * offset);
            }
        }
        else if (isBend || isJunction)
        {
            foreach (var from in connected)
            {
                foreach (var to in connected)
                {
                    if (from == to || to == -from)
                        continue;

                    Vector3 centerOffset = center;

                    // Forward lane
                    Vector3 rightFrom = Vector3.Cross(Vector3.up, from).normalized * laneOffset;
                    Vector3 rightTo = Vector3.Cross(Vector3.up, to).normalized * laneOffset;
                    Vector3 startFwd = centerOffset + from * tileSpacing / 2f + rightFrom;
                    Vector3 endFwd = centerOffset + to * tileSpacing / 2f + rightTo;
                    Vector3 controlFwd = centerOffset + (from + to).normalized * tileSpacing / 3f + rightFrom;
                    fwd.AddRange(Bezier(startFwd, controlFwd, endFwd));

                    // Backward lane (reverse direction)
                    Vector3 reverseFrom = -to;
                    Vector3 reverseTo = -from;
                    Vector3 rightFromBack = Vector3.Cross(Vector3.up, reverseFrom).normalized * laneOffset;
                    Vector3 rightToBack = Vector3.Cross(Vector3.up, reverseTo).normalized * laneOffset;
                    Vector3 startBack = centerOffset + reverseFrom * tileSpacing / 2f + rightFromBack;
                    Vector3 endBack = centerOffset + reverseTo * tileSpacing / 2f + rightToBack;
                    Vector3 controlBack = centerOffset + (reverseFrom + reverseTo).normalized * tileSpacing / 3f + rightFromBack;
                    List<Vector3> backCurve = Bezier(startBack, controlBack, endBack);
                    backCurve.Reverse();
                    back.AddRange(backCurve);
                }
            }
        }

        string fwdName = tileName + "_" + center + "_Forward";
        string backName = tileName + "_" + center + "_Backward";

        AddLane(fwdName, fwd);
        AddLane(backName, back);

        if (isJunction || isBend)
        {
            junctionSegmentNames.Add(fwdName);
            junctionSegmentNames.Add(backName);
        }
    }

    private void AddLane(string name, List<Vector3> points)
    {
        LanePath segment = ScriptableObject.CreateInstance<LanePath>();
        segment.name = name;
        segment.points = points;
        laneSegments[name] = segment;
    }

    private List<Vector3> Bezier(Vector3 p0, Vector3 p1, Vector3 p2, int steps = 5)
    {
        List<Vector3> curve = new();
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 pt = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
            curve.Add(pt);
        }
        return curve;
    }

    [ContextMenu("Link Lane Segments")]
    private void LinkLaneSegments(List<LanePath> allSegments)
    {
        foreach (var segment in allSegments)
            segment.nextLanes.Clear();

        foreach (var segment in allSegments)
        {
            bool isForward = segment.name.Contains("Forward");
            Vector3 end = segment.points[^1];

            foreach (var other in allSegments)
            {
                if (segment == other) continue;

                bool otherIsForward = other.name.Contains("Forward");

                if (isForward != otherIsForward) continue;

                Vector3 start = other.points[0];

                if (IsAdjacent(RoundVec(end), RoundVec(start)))
                {
                    segment.nextLanes.Add(other);
                }
            }
        }
    }

    private void ExportCombinedPaths(List<LanePath> allSegments)
    {
        List<LanePath> forward = allSegments
            .Where(l => l.name.Contains("Forward") && !junctionSegmentNames.Contains(l.name))
            .ToList();

        List<LanePath> backward = allSegments
            .Where(l => l.name.Contains("Backward") && !junctionSegmentNames.Contains(l.name))
            .ToList();

        List<Vector3> finalForward = BuildLongestPath(forward);
        List<Vector3> finalBackward = BuildLongestPath(backward);

        ExportLanePath("ForwardPath", finalForward);
        ExportLanePath("BackwardPath", finalBackward);
    }

    private List<Vector3> BuildLongestPath(List<LanePath> segments)
    {
        HashSet<LanePath> visited = new();
        List<Vector3> longestPath = new();

        foreach (var start in segments)
        {
            if (visited.Contains(start)) continue;

            List<Vector3> currentPath = new(start.points);
            LanePath current = start;
            visited.Add(current);

            while (current.nextLanes.Count > 0)
            {
                var next = current.nextLanes.FirstOrDefault(n => !visited.Contains(n));
                if (next == null) break;

                visited.Add(next);
                currentPath.AddRange(next.points);
                current = next;
            }

            if (currentPath.Count > longestPath.Count)
                longestPath = currentPath;
        }

        return longestPath;
    }

    private void ExportLanePath(string name, List<Vector3> path)
    {
        if (!AssetDatabase.IsValidFolder(exportPath))
        {
            Directory.CreateDirectory(exportPath);
            AssetDatabase.Refresh();
        }

        var asset = ScriptableObject.CreateInstance<LanePath>();
        asset.points = path;
        asset.name = name;

        AssetDatabase.CreateAsset(asset, Path.Combine(exportPath, name + ".asset"));
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeGizmos || laneSegments == null) return;

        foreach (var lane in laneSegments)
        {
            Gizmos.color = lane.Key.Contains("Forward") ? Color.green : Color.red;
            List<Vector3> points = lane.Value.points;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
                Vector3 direction = (points[i + 1] - points[i]).normalized;
                Vector3 arrowHead = points[i + 1] - direction * 0.5f;
                Vector3 side1 = Quaternion.Euler(0, 30, 0) * -direction;
                Vector3 side2 = Quaternion.Euler(0, -30, 0) * -direction;
                Gizmos.DrawRay(arrowHead, side1 * 0.3f);
                Gizmos.DrawRay(arrowHead, side2 * 0.3f);
            }

            if (points.Count > 0)
            {
                Handles.color = Color.white;
                Handles.Label(points[0], lane.Key);
            }

            foreach (var next in lane.Value.nextLanes)
            {
                if (next != null && next.points.Count > 0 && points.Count > 0)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(points[^1], next.points[0]);
                }
            }
        }
    }

    bool IsAdjacent(Vector3 a, Vector3 b)
    {
        Vector3 diff = b - a;
        return (Mathf.Abs(diff.x) <= connectThreshold && Mathf.Approximately(diff.z, 0)) ||
               (Mathf.Abs(diff.z) <= connectThreshold && Mathf.Approximately(diff.x, 0));
    }
}