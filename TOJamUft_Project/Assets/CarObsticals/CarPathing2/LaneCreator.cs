using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public enum RoadType { Straight, TJunction, Corner, Roundabout }

[DisallowMultipleComponent]
public class RoadTileInfo : MonoBehaviour
{
    public RoadType roadType = RoadType.Straight;
}

public class LaneCreator : MonoBehaviour
{
    [Header("Lane Settings")]
    public GameObject lanePointPrefab;
    public int numLanes = 2;
    public float tileEdgeOffset = 1f;

    [Header("Raycast Settings")]
    public float rayHeight = 10f;
    public float rayDistance = 20f;
    public LayerMask wallLayer;

    [Header("Lane Offset")]
    public float laneOffsetAmount = 0.5f;

    [Header("Export Settings")]
    public string exportPath = "Assets/GeneratedLanes";
    public string laneAssetPrefix = "Lane_";

    private Dictionary<int, List<Transform>> lanes = new();

    [ContextMenu("Generate Lanes")]
    void GenerateLanes()
    {
        ClearGeneratedPoints();
        lanes.Clear();

        foreach (Transform tile in transform)
        {
            RoadTileInfo tileInfo = tile.GetComponent<RoadTileInfo>();
            if (!tileInfo)
            {
                tileInfo = tile.gameObject.AddComponent<RoadTileInfo>();
                string name = tile.name.ToLower();
                if (name.Contains("round")) tileInfo.roadType = RoadType.Roundabout;
                else if (name.Contains("inter")) tileInfo.roadType = RoadType.TJunction;
                else if (name.Contains("bend")) tileInfo.roadType = RoadType.Corner;
                else tileInfo.roadType = RoadType.Straight;
                //Debug.Log($"Auto-assigned {tileInfo.roadType} to {tile.name}");
            }

            switch (tileInfo.roadType)
            {
                case RoadType.Roundabout:
                    CreateRoundaboutLanes(tile);
                    break;
                case RoadType.TJunction:
                    CreateTJunctionLanes(tile);
                    break;
                case RoadType.Corner:
                    CreateCornerLanes(tile);
                    break;
                case RoadType.Straight:
                    CircleCastInDirections(tile);
                    break;
            }

        }
    }

    void CastAndPlaceLanes(Transform tile, Vector3[] directions)
    {
        MeshFilter mf = tile.GetComponent<MeshFilter>();
        if (!mf || mf.sharedMesh == null) return;

        Bounds bounds = mf.sharedMesh.bounds;
        Vector3 center = tile.TransformPoint(bounds.center);

        foreach (Vector3 dir in directions)
        {
            Vector3 roughStart = center + Vector3.up * rayHeight;

            if (Physics.SphereCast(roughStart, 10f, dir, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.CompareTag("Road") && hit.transform != tile)
                {
                    // Now call PlaceLaneAtEdge that will individually raycast down from above the edge
                    PlaceLaneAtEdge(tile, hit.transform, dir, bounds, 0);
                    PlaceLaneAtEdge(hit.transform, tile, -dir, bounds, 1);
                }
            }
        }
    }


    void CreateTJunctionLanes(Transform tile)
    {
        // Treat TJunction like a T with 3 connections
        Vector3[] directions = { Vector3.forward, Vector3.left, Vector3.right };
        CastAndPlaceLanes(tile, directions);
    }

    void CreateCornerLanes(Transform tile)
    {
        // Corners have two connections (like an L shape)
        Vector3[] directions = { Vector3.forward, Vector3.right };
        CastAndPlaceLanes(tile, directions);
    }

    void CircleCastInDirections(Transform tile)
    {
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        MeshFilter mf = tile.GetComponent<MeshFilter>();
        if (!mf || mf.sharedMesh == null) return;

        Bounds bounds = mf.sharedMesh.bounds;
        Vector3 center = tile.TransformPoint(bounds.center);

        foreach (Vector3 dir in directions)
        {
            Vector3 start = center + Vector3.up * rayHeight;
            if (Physics.SphereCast(start, 10f, dir, out RaycastHit hit, rayDistance))
            {
                
                if (hit.collider.CompareTag("Road") && hit.transform != tile)
                {
                    PlaceLaneAtEdge(tile, hit.transform, dir, bounds, 0);
                    PlaceLaneAtEdge(hit.transform, tile, -dir, bounds, 1);
                }
                
            }
        }
    }
    void CreateRoundaboutLanes(Transform tile)
    {
        MeshFilter mf = tile.GetComponent<MeshFilter>();
        if (!mf || mf.sharedMesh == null) return;

        Bounds bounds = mf.sharedMesh.bounds;
        Vector3 center = tile.TransformPoint(bounds.center);
        float radius = Mathf.Max(bounds.extents.x, bounds.extents.z) - tileEdgeOffset;

        for (int lane = 0; lane < numLanes; lane++)
        {
            float innerRadius = radius - lane * laneOffsetAmount;
            float angleStep = 360f / 8f; // 8 segments around the circle

            for (int i = 0; i < 8; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float nextAngle = (i + 1) * angleStep * Mathf.Deg2Rad;

                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * innerRadius + center;
                Vector3 nextPos = new Vector3(Mathf.Cos(nextAngle), 0, Mathf.Sin(nextAngle)) * innerRadius + center;
                Quaternion rot = Quaternion.LookRotation((nextPos - pos).normalized);
                Vector3 snapped = SnapToFloor(pos + Vector3.up * rayHeight);
                if (snapped == Vector3.zero) continue;

                Vector3 roughPos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * innerRadius + center + Vector3.up * rayHeight;

                if (Physics.Raycast(roughPos, Vector3.down, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Ignore))
                {
                    Vector3 finalPos = hit.point;
                    //Quaternion rot = Quaternion.LookRotation((nextPos - pos).normalized);
                    GameObject pt = Instantiate(lanePointPrefab, finalPos, rot, this.transform);
                    pt.name = $"Roundabout_{lane}_{i}";
                    AddLanePoint(lane, pt.transform);
                }
                else
                {
                    Debug.LogWarning($"Roundabout lane point failed snap at index {i}, lane {lane}");
                }

            }
        }
    }
    void PlaceLaneAtEdge(Transform fromTile, Transform toTile, Vector3 direction, Bounds bounds, int lane)
    {
        if (fromTile == null || toTile == null || fromTile == toTile) return;

        // Calculate max extent of tile along X and Z (for edge)
        float maxExtentX = bounds.extents.x;
        float maxExtentZ = bounds.extents.z;

        Vector3 fromCenter = fromTile.TransformPoint(bounds.center);
        Vector3 toCenter = toTile.TransformPoint(bounds.center);

        Vector3 worldDir = direction.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(worldDir);

        
        // Calculate lane side offset (left or right offset based on lane index)
        Vector3 sideDir = Vector3.Cross(Vector3.up, worldDir).normalized;
        float sideMultiplier = (lane == 0) ? 1 : -1;
        Vector3 sideOffset = sideDir * sideMultiplier * laneOffsetAmount;

        // Forward offset pushes point towards tile edge
        float forwardOffsetAmount = Mathf.Max(maxExtentX, maxExtentZ) - tileEdgeOffset;
        Vector3 forwardOffset = worldDir * forwardOffsetAmount;

        // Calculate raw position above the edge of the tile
        Vector3 roughPos = fromCenter + forwardOffset + sideOffset + Vector3.up * rayHeight;

        // Raycast down from roughPos to find exact floor position
        if (Physics.Raycast(roughPos, Vector3.down, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 finalPos = hit.point;

            // Instantiate lane point prefab at final snapped position
            GameObject pt = Instantiate(lanePointPrefab, finalPos, lookRotation, this.transform);
            pt.name = $"LanePoint_{lane}_{fromTile.name}_to_{toTile.name}";
            AddLanePoint(lane, pt.transform);
        }
        else
        {
            Debug.LogWarning($"Failed to snap lane point from {fromTile.name} towards {toTile.name}");
        }
    }

    void AddLanePoint(int laneIndex, Transform point)
    {
        if (!lanes.ContainsKey(laneIndex))
            lanes[laneIndex] = new List<Transform>();

        lanes[laneIndex].Add(point);

        // Add collider and LanePoint component if missing
        SphereCollider col = point.GetComponent<SphereCollider>();
        if (col == null)
        {
            col = point.gameObject.AddComponent<SphereCollider>();
            col.radius = 0.3f; // small radius for detection
            col.isTrigger = true;
        }

        LanePoint lanePointComp = point.GetComponent<LanePoint>();
        if (lanePointComp == null)
            lanePointComp = point.gameObject.AddComponent<LanePoint>();

        // Assign the lanePath asset reference if it exists, else null for now
        lanePointComp.lanePath = null;
    }
    Vector3 SnapToFloor(Vector3 start) =>
        Physics.Raycast(start, Vector3.down, out var hit, rayDistance) ? hit.point : Vector3.zero;

    void ClearGeneratedPoints()
    {
        var garbage = new[] { "LanePoint", "LaneMid", "Roundabout", "LaneCurve", "LaneEnd" };
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (garbage.Any(g => child.name.StartsWith(g)))
                DestroyImmediate(child.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (lanes == null) return;

        Color[] colors = { Color.green, Color.cyan, Color.magenta, Color.red };

        foreach (var kvp in lanes)
        {
            lanes[kvp.Key].RemoveAll(p => p == null);
            Gizmos.color = colors[kvp.Key % colors.Length];

            var points = kvp.Value;
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (!points[i] || !points[i + 1]) continue;

                Gizmos.DrawSphere(points[i].position, 0.2f);
                Gizmos.DrawLine(points[i].position, points[i + 1].position);

                Vector3 dir = (points[i + 1].position - points[i].position).normalized;
                Vector3 arrowHead = points[i].position + dir * 0.5f;
                Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 160, 0) * Vector3.forward;
                Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -160, 0) * Vector3.forward;
                Gizmos.DrawLine(arrowHead, arrowHead + right * 0.2f);
                Gizmos.DrawLine(arrowHead, arrowHead + left * 0.2f);
            }
        }

#if UNITY_EDITOR
        Handles.color = Color.white;
        foreach (var kvp in lanes)
        {
            foreach (Transform pt in kvp.Value)
            {
                if (pt != null)
                    Handles.Label(pt.position + Vector3.up * 0.3f, $"Lane {kvp.Key}");
            }
        }
#endif
    }

    [ContextMenu("Export Lanes to ScriptableObjects")]
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

        foreach (var kvp in lanes)
        {
            LanePath asset = ScriptableObject.CreateInstance<LanePath>();
            foreach (var pt in kvp.Value) asset.points.Add(pt.position);

            string path = $"{exportPath}/{laneAssetPrefix}{kvp.Key}.asset";
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            // Assign lanePath reference to each LanePoint component on points of this lane
            foreach (var pt in kvp.Value)
            {
                if (pt == null) continue;
                var lanePointComp = pt.GetComponent<LanePoint>();
                if (lanePointComp != null)
                {
                    lanePointComp.lanePath = asset;
                    EditorUtility.SetDirty(lanePointComp);
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Exported {lanes.Count} lanes to {exportPath}");
    }

    
}
