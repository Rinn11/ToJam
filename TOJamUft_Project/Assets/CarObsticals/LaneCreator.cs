using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class LaneCreator : MonoBehaviour
{
    [Header("Lane Settings")]
    public GameObject lanePointPrefab;
    public int numLanes = 2;
    public float pointSpacing = 5f;
    public float tileEdgeOffset = 1f;

    [Header("Raycast Settings")]
    public float rayHeight = 10f;
    public float rayDistance = 20f;
    public LayerMask wallLayer;
    public float maxHeightDiff = 1f;

    private Dictionary<int, List<Transform>> lanes = new();

    [ContextMenu("Generate Lanes")]
    void GenerateLanes()
    {
        ClearGeneratedPoints();
        lanes.Clear();

        // Gather all child tiles
        List<Transform> tiles = new();
        foreach (Transform tile in transform)
        {
            tiles.Add(tile);
        }

        // Per-lane data for starts and ends
        Dictionary<(int lane, Transform tile), Vector3> startPoints = new();

        // Pass 1: Set starting points
        foreach (Transform tile in tiles)
        {
            MeshFilter meshFilter = tile.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;

            Mesh mesh = meshFilter.sharedMesh;
            Bounds bounds = mesh.bounds;

            float tileWidth = bounds.size.x;
            float laneWidth = tileWidth / numLanes;
            float startZ = bounds.min.z + tileEdgeOffset;
            float leftEdgeX = bounds.min.x + (laneWidth / 2f);

            for (int lane = 0; lane < numLanes; lane++)
            {
                float xPos = leftEdgeX + lane * laneWidth;
                Vector3 localStart = new Vector3(xPos, 0, startZ);
                Vector3 worldStart = tile.TransformPoint(localStart);
                Vector3 snappedStart = SnapToFloor(worldStart + Vector3.up * rayHeight);

                if (snappedStart == Vector3.zero) continue;

                GameObject startGO = Instantiate(lanePointPrefab, snappedStart, Quaternion.identity, transform);
                startGO.name = $"LanePoint_{lane}_Start_{tile.name}";

                if (!lanes.ContainsKey(lane)) lanes[lane] = new List<Transform>();
                lanes[lane].Add(startGO.transform);
                startPoints[(lane, tile)] = snappedStart;
            }
        }

        // Pass 2: Set end points & optional intermediate points
        foreach (Transform tile in tiles)
        {
            MeshFilter meshFilter = tile.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;

            Mesh mesh = meshFilter.sharedMesh;
            Bounds bounds = mesh.bounds;

            float tileWidth = bounds.size.x;
            float laneWidth = tileWidth / numLanes;
            float endZ = bounds.max.z - tileEdgeOffset;
            float leftEdgeX = bounds.min.x + (laneWidth / 2f);

            for (int lane = 0; lane < numLanes; lane++)
            {
                // Check if this tile has a next tile
                int currentIndex = tiles.IndexOf(tile);
                if (currentIndex == -1 || currentIndex + 1 >= tiles.Count) continue;

                Transform nextTile = tiles[currentIndex + 1];
                if (!startPoints.TryGetValue((lane, nextTile), out Vector3 nextStart)) continue;

                float xPos = leftEdgeX + lane * laneWidth;
                Vector3 localEnd = new Vector3(xPos, 0, endZ);
                Vector3 worldEnd = tile.TransformPoint(localEnd);
                Vector3 snappedEnd = SnapToFloor(worldEnd + Vector3.up * rayHeight);

                if (snappedEnd == Vector3.zero) continue;

                Vector3 direction = (nextStart - snappedEnd).normalized;
                GameObject endGO = Instantiate(lanePointPrefab, snappedEnd, Quaternion.LookRotation(direction, Vector3.up), transform);
                endGO.name = $"LanePoint_{lane}_End_{tile.name}";
                lanes[lane].Add(endGO.transform);

                // Add curve points if direction mismatch
                Vector3 startPos = startPoints[(lane, tile)];
                float angle = Vector3.Angle((snappedEnd - startPos).normalized, direction);
                if (angle > 1f)
                {
                    float dist = Vector3.Distance(startPos, snappedEnd);
                    int segments = Mathf.Max(3, Mathf.FloorToInt(dist / pointSpacing));

                    for (int i = 1; i < segments; i++)
                    {
                        float t = (float)i / segments;
                        Vector3 linear = Vector3.Lerp(startPos, snappedEnd, t);

                        // Curve outward from start → end (perpendicular to direction)
                        Vector3 straight = (snappedEnd - startPos).normalized;
                        Vector3 curveDir = Vector3.Cross(straight, Vector3.up);
                        float waveOffset = Mathf.Sin(t * Mathf.PI) * 0.5f;

                        Vector3 wavy = linear + curveDir * waveOffset;
                        Vector3 snapped = SnapToFloor(wavy + Vector3.up * rayHeight);

                        if (snapped == Vector3.zero || Physics.CheckSphere(snapped + Vector3.up * 0.1f, 0.2f, wallLayer)) continue;

                        GameObject mid = Instantiate(lanePointPrefab, snapped, Quaternion.identity, transform);
                        mid.name = $"LanePoint_{lane}_Mid_{tile.name}_{i}";
                        lanes[lane].Insert(lanes[lane].Count - 1, mid.transform);
                    }
                }
            }
        }
    }



    Vector3 SnapToFloor(Vector3 start)
    {
        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, rayDistance))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    void ClearGeneratedPoints()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("LanePoint"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) // Ensure proper gizmo colors before Play Mode
            UnityEditor.Handles.color = Color.white;

        // Draw all points
        foreach (var child in transform.Cast<Transform>())
        {
            if (child.name.StartsWith("LanePoint"))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(child.position, 0.2f);
            }
        }

        // Draw connections and directions
        if (lanes != null)
        {
            int laneIndex = 0;
            foreach (var lane in lanes.Values)
            {
                // Cycle through some distinct colors
                Gizmos.color = Color.HSVToRGB((laneIndex * 0.2f) % 1f, 0.8f, 1f);
                Color arrowColor = Gizmos.color;
                laneIndex++;

                for (int i = 0; i < lane.Count - 1; i++)
                {
                    Vector3 from = lane[i].position;
                    Vector3 to = lane[i + 1].position;

                    // Line
                    Gizmos.DrawLine(from, to);

                    // Arrow head (with Handles)
                    Vector3 dir = (to - from).normalized;
                    Vector3 mid = Vector3.Lerp(from, to, 0.5f);
                    float arrowSize = 0.5f;

                    UnityEditor.Handles.color = arrowColor;
                    UnityEditor.Handles.ArrowHandleCap(0, mid, Quaternion.LookRotation(dir), arrowSize, EventType.Repaint);
                }
            }
        }
    }


    [Header("Export Settings")]
    public string exportPath = "Assets/GeneratedLanes";
    public string laneAssetPrefix = "Lane_";

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
            System.IO.Directory.CreateDirectory(exportPath);
            AssetDatabase.Refresh();
        }

        foreach (var kvp in lanes)
        {
            int laneIndex = kvp.Key;
            List<Transform> lanePoints = kvp.Value;

            LanePath laneAsset = ScriptableObject.CreateInstance<LanePath>();
            foreach (var t in lanePoints)
            {
                laneAsset.points.Add(t.position);
            }

            string assetName = $"{laneAssetPrefix}{laneIndex}.asset";
            AssetDatabase.CreateAsset(laneAsset, $"{exportPath}/{assetName}");
            AssetDatabase.SaveAssets();
        }

        Debug.Log($"Exported {lanes.Count} lanes to {exportPath}");
    }
}
