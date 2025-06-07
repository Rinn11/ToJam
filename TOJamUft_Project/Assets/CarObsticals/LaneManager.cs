using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance;

    private List<LanePath> allLanes = new();

    void Awake()
    {
        Instance = this;
        LoadAllLanes();
    }

    void LoadAllLanes()
    {
        allLanes.Clear();
        allLanes.AddRange(Resources.LoadAll<LanePath>("")); // Assumes lanes are in a Resources folder
    }

    public LanePath FindNearestLane(Vector3 position, float threshold = 5f)
    {
        LanePath nearest = null;
        float minDist = threshold;

        foreach (var lane in allLanes)
        {
            if (lane.points.Count == 0) continue;

            float dist = Vector3.Distance(position, lane.points[0]);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = lane;
            }
        }

        return nearest;
    }
}

