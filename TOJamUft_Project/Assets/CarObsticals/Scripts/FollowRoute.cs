using System.Collections.Generic;
using UnityEngine;

public class FollowRoute : MonoBehaviour
{
    [SerializeField] private float carSpeed = 10f;

    private List<Transform> orderedRoadGroups = new List<Transform>();
    private int route; // 0 = left, 1 = middle, 2 = right
    private Transform targetPosition;
    private int currentRoadIndex = 0;
    private bool firstMove = true;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void setRoute(int currentRoute, Transform roadsContainer, Transform spawnPosition)
    {
        orderedRoadGroups.Clear();
        route = -1;

        // Gather ordered groups
        for (int i = 0; i < roadsContainer.childCount; i++)
        {
            orderedRoadGroups.Add(roadsContainer.GetChild(i));
        }

        // Find which group and lane the spawnPosition belongs to
        for (int groupIndex = 0; groupIndex < orderedRoadGroups.Count; groupIndex++)
        {
            Transform group = orderedRoadGroups[groupIndex];
            for (int laneIndex = 0; laneIndex < group.childCount; laneIndex++)
            {
                if (group.GetChild(laneIndex) == spawnPosition)
                {
                    currentRoadIndex = groupIndex;
                    route = laneIndex;
                    targetPosition = spawnPosition;
                    Debug.Log($"Spawned at group {group.name} (index {groupIndex}), lane {laneIndex}");
                    return;
                }
            }
        }

        Debug.LogError("Spawn position not found in any road group!");
    }

    private void setTarget()
    {
        if (firstMove)
        {
            firstMove = false;
            return; // Stay at the spawnPosition on first call
        }

        // Advance based on route direction
        if (route < 2)
            currentRoadIndex++;
        else
            currentRoadIndex--;

        // End of route
        if (currentRoadIndex >= orderedRoadGroups.Count || currentRoadIndex < 0)
        {
            Debug.Log("Route complete. Destroying car.");
            Destroy(gameObject);
            return;
        }

        Transform group = orderedRoadGroups[currentRoadIndex];

        if (route >= group.childCount)
        {
            Debug.LogWarning("Route index out of range for group: " + group.name);
            Destroy(gameObject);
            return;
        }

        targetPosition = group.GetChild(route);
        Debug.Log("New Target: " + targetPosition.name);
    }

    void FixedUpdate()
    {
        if (targetPosition == null) return;

        rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition.position, Time.fixedDeltaTime * carSpeed));
        transform.LookAt(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition.position) < 0.5f)
        {
            setTarget();
        }
    }

    private void OnDrawGizmos()
    {
        if (targetPosition != null)
        {
            // Change line color based on route
            switch (route)
            {
                case 0:
                    Gizmos.color = Color.blue;
                    break;
                case 1:
                    Gizmos.color = Color.yellow;
                    break;
                case 2:
                    Gizmos.color = Color.magenta;
                    break;
                default:
                    Gizmos.color = Color.white;
                    break;
            }

            // Draw line from car to target
            Gizmos.DrawLine(transform.position, targetPosition.position);

            // Draw sphere at target
            Gizmos.DrawSphere(targetPosition.position, 0.3f);
        }
    }

}
