using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PathFollower : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 5f;
    public float closeEnoughDistance = 0.5f;

    private Rigidbody rb;
    private LanePath currentLane;
    private int currentPointIndex;
    private bool isDriving = false;

    public void AssignLane(LanePath lane)
    {
        currentLane = lane;
        currentPointIndex = 0;
        isDriving = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isDriving || currentLane == null || currentPointIndex >= currentLane.points.Count)
            return;

        Vector3 target = currentLane.points[currentPointIndex];
        Vector3 direction = (target - transform.position).normalized;
        Vector3 move = direction * speed;

        // Apply movement via Rigidbody
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // Rotate smoothly toward movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * turnSpeed));
        }

        // Check if close to current point
        if (Vector3.Distance(transform.position, target) < closeEnoughDistance)
        {
            currentPointIndex++;

            if (currentPointIndex >= currentLane.points.Count)
            {
                TrySwitchToNextLane();
            }
        }
    }

    void TrySwitchToNextLane()
    {
        Vector3 endPoint = currentLane.points[^1];
        LanePath newLane = LaneManager.Instance.FindNearestLane(endPoint, 5f);

        if (newLane != null && newLane != currentLane)
        {
            AssignLane(newLane);
        }
        else
        {
            Debug.Log("Car reached end of route, despawning.");
            Destroy(gameObject);
        }
    }
}

