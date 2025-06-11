using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMover : MonoBehaviour
{
    public LanePath lanePath;
    public float speed = 10f;
    public float turnSpeed = 180f;
    public float closeEnoughDistance = 1f;

    private Rigidbody rb;
    private int currentIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (lanePath == null || lanePath.points.Count == 0)
        {
            Debug.LogWarning("LanePath not assigned or empty.");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (currentIndex >= lanePath.points.Count) return;

        Vector3 target = lanePath.points[currentIndex];
        Vector3 direction = (target - transform.position).normalized;

        Vector3 move = direction * speed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) < closeEnoughDistance)
            currentIndex++;
    }
}
