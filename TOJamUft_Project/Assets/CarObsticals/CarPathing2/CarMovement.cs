using UnityEngine;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    public List<Transform> lanePoints;  // Set these to the lane points of the lane to follow
    public float speed = 5f;
    public float rotationSpeed = 5f;

    // Angle threshold to consider a point "ahead" (in degrees)
    public float maxAngleAhead = 90f;

    private Transform targetPoint;
    private LanePath currentLane;

    void Update()
    {


        if (lanePoints == null || lanePoints.Count == 0)
            return;

        // Find the nearest point ahead of the car
        //targetPoint = FindNearestPointAhead();

        if (targetPoint == null)
            return;

        Vector3 direction = targetPoint.position - transform.position;
        Vector3 move = direction.normalized * speed * Time.deltaTime;

        // Move towards the target point
        if (direction.magnitude <= move.magnitude)
        {
            transform.position = targetPoint.position;
            // Next update will find a new nearest ahead point
        }
        else
        {
            transform.position += move;
        }

        // Rotate smoothly towards movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Transform FindNearestTile(Vector3 position, List<Transform> tiles, float maxDistance = 0.5f)
    {
        Transform closest = null;
        float minDist = float.MaxValue;
        foreach (var tile in tiles)
        {
            float dist = Vector3.Distance(tile.position, position);
            if (dist < minDist && dist <= maxDistance)
            {
                minDist = dist;
                closest = tile;
            }
        }
        return closest;
    }
}
