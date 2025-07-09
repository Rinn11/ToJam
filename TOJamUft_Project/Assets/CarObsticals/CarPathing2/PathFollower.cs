using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PathFollower : MonoBehaviour
{
    public LanePath lanePath;

    public float speed = 10f;
    public float turnSpeed = 180f; // degrees per second, increased for better responsiveness
    public float closeEnoughDistance = 1f;
    public float maxSearchDistance = 20f; // max distance to consider for next points

    private Rigidbody rb;
    private Vector3 targetPoint;
    private Vector3 previousTargetPoint;

    public Vector3[] points;

    private void Start()
    {
        // to not delete srictible object points
        for (int i = 0; i < lanePath.points.Count; i++)
        {
            points[i] = lanePath.points[i];
        }

        Debug.Log(lanePath.points.Count);

        rb = GetComponent<Rigidbody>();

        if (lanePath == null || lanePath.points == null || lanePath.points.Count == 0)
        {
            Debug.LogWarning("LanePath or its points are empty!");
            enabled = false;
            return;
        }

        targetPoint = FindClosestPoint();
    }

    private void FixedUpdate()
    {
        //if (targetPoint == null) return;

        Vector3 directionToTarget = targetPoint - transform.position;
        float distance = directionToTarget.magnitude;

        if (distance < closeEnoughDistance)
        {
            targetPoint = FindClosestPoint();
        }

        Vector3 directionNormalized = directionToTarget.normalized;

        // Smooth rotation with clamp on turn speed (degrees per second)
        Quaternion targetRotation = Quaternion.LookRotation(directionNormalized);
        float maxStep = turnSpeed * Time.fixedDeltaTime;
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxStep);
        rb.MoveRotation(newRotation);

        // Move forward at speed
        rb.linearVelocity = transform.forward * speed;

        Debug.DrawLine(transform.position, targetPoint, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
    }

    private Vector3 FindClosestPoint()
    {
        Vector3 previousPoint = new Vector3(1000000, 100000, 100000);

        Debug.Log(previousPoint+ " " + points.Length);

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].magnitude - this.transform.position.magnitude < previousPoint.magnitude-  this.transform.position.magnitude)
            {
                Debug.Log("To big");
                Debug.Log(i);

                previousPoint = points[i];
            }
        }

        lanePath.points.Remove(previousPoint);
        
        return previousPoint;
    }

    private void OnDrawGizmos()
    {
        if (lanePath == null || lanePath.points == null) return;

        Gizmos.color = Color.yellow;
        foreach (var p in lanePath.points)
            Gizmos.DrawSphere(p, 0.3f);

        if (Application.isPlaying && targetPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPoint);
        }
    }
}
