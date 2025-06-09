// LaneGenerator.cs
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
    public bool visualizeGizmos = true;

    public string exportPath = "Assets/Lanes";
    public string forwardAssetName = "Lane_Forward.asset";
    public string backwardAssetName = "Lane_Backward.asset";

    public Dictionary<string, List<Vector3>> lanes = new();

    private Vector3 RoundVec(Vector3 v) => new(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

    private Transform FindNearestTile(Vector3 pos, List<Transform> tiles, float maxDist = 0.6f)
    {
        Transform closest = null;
        float min = float.MaxValue;
        foreach (var t in tiles)
        {
            float dist = Vector3.Distance(RoundVec(t.position), RoundVec(pos));
            if (dist < min && dist <= maxDist)
            {
                min = dist;
                closest = t;
            }
        }
        return closest;
    }

    [ContextMenu("Generate Lanes")]
    public void GenerateLanes()
    {
        lanes.Clear();

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

        Debug.Log($"Lane generation complete. Total: {lanes.Count}");
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
        else if (isBend)
        {
            Vector3 from = connected[0];
            Vector3 to = connected[1];
            Vector3 fromRight = Vector3.Cross(Vector3.up, from).normalized * laneOffset;
            Vector3 toRight = Vector3.Cross(Vector3.up, to).normalized * laneOffset;

            Vector3 startFwd = center + from * tileSpacing / 2f + fromRight;
            Vector3 endFwd = center + to * tileSpacing / 2f + toRight;
            fwd.AddRange(Bezier(startFwd, center, endFwd));

            Vector3 startBack = center + to * tileSpacing / 2f - toRight;
            Vector3 endBack = center + from * tileSpacing / 2f - fromRight;
            back.AddRange(Bezier(startBack, center, endBack));
        }
        else if (isJunction)
        {
            foreach (var dir in connected)
            {
                Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
                float half = (nodesPerLane - 1) / 2f;
                for (int i = 0; i < nodesPerLane; i++)
                {
                    float offset = (i - half) * nodeSpacing;
                    fwd.Add(center + right * laneOffset + dir * offset);
                    back.Add(center - right * laneOffset - dir * offset);
                }
            }
        }

        lanes[$"{tileName}_{center}_Forward"] = fwd;
        lanes[$"{tileName}_{center}_Backward"] = back;
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

    [ContextMenu("Export Lanes To Assets")]
    public void ExportLanesToAssets()
    {
        if (lanes == null || lanes.Count == 0)
        {
            Debug.LogWarning("No lanes to export. Run GenerateLanes first.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(exportPath))
        {
            Directory.CreateDirectory(exportPath);
            AssetDatabase.Refresh();
        }

        List<Vector3> forwardPoints = new();
        List<Vector3> backwardPoints = new();

        foreach (var kvp in lanes)
        {
            if (kvp.Key.Contains("Forward"))
                forwardPoints.AddRange(kvp.Value);
            else if (kvp.Key.Contains("Backward"))
                backwardPoints.AddRange(kvp.Value);
        }

        LanePath forwardAsset = ScriptableObject.CreateInstance<LanePath>();
        forwardAsset.points = forwardPoints;
        AssetDatabase.CreateAsset(forwardAsset, Path.Combine(exportPath, forwardAssetName));

        LanePath backwardAsset = ScriptableObject.CreateInstance<LanePath>();
        backwardAsset.points = backwardPoints;
        AssetDatabase.CreateAsset(backwardAsset, Path.Combine(exportPath, backwardAssetName));

        AssetDatabase.SaveAssets();
        Debug.Log("Exported combined forward and backward lanes as two ScriptableObjects.");
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeGizmos || lanes == null) return;

        foreach (var lane in lanes)
        {
            bool isForward = lane.Key.Contains("Forward");
            Gizmos.color = isForward ? Color.green : Color.red;
            List<Vector3> points = lane.Value;

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
        }

        Gizmos.color = Color.cyan;
        foreach (Transform tile in transform)
        {
            Gizmos.DrawWireCube(RoundVec(tile.position), Vector3.one * 0.5f);
        }
    }
}

