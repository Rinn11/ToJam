using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance;

    public List<LanePath> allLanes = new List<LanePath>(); // Assign all your exported LanePaths here in inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Finds the nearest LanePath based on point proximity to the given position.
    /// </summary>
    public LanePath FindNearestLane(Vector3 position, float maxDistance = 10f)
    {
        LanePath closestLane = null;
        float closestDist = float.MaxValue;

        foreach (var lane in allLanes)
        {
            foreach (var point in lane.points)
            {
                float dist = Vector3.Distance(position, point);
                if (dist < closestDist && dist <= maxDistance)
                {
                    closestDist = dist;
                    closestLane = lane;
                }
            }
        }

        return closestLane;
    }

    /// <summary>
    /// Finds the nearest point **in forward direction** on the given LanePath.
    /// </summary>
    public Vector3? FindNearestPointInDirection(Vector3 fromPosition, Vector3 forward, LanePath lane)
    {
        float bestDot = -1f;
        Vector3? bestPoint = null;

        foreach (var point in lane.points)
        {
            Vector3 toPoint = (point - fromPosition).normalized;
            float dot = Vector3.Dot(forward.normalized, toPoint);
            float dist = Vector3.Distance(fromPosition, point);

            if (dot > 0.5f && dist < 15f && dot > bestDot) // Optional: tweak dot/dist threshold
            {
                bestDot = dot;
                bestPoint = point;
            }
        }

        return bestPoint;
    }
}
