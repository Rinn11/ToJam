using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMover : MonoBehaviour
{
    public LanePath lanePath;
    public float speed = 10f;
    public float turnSpeed = 180f; // degrees per second
    public float closeEnoughDistance = 1f;
    public bool loopPath = false; // Optional: loop path or stop at end

    private Rigidbody rb;
    private int currentIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (lanePath == null || lanePath.points == null || lanePath.points.Count == 0)
        {
            Debug.LogWarning("LanePath not assigned or empty.");
            enabled = false;
            return;
        }

        // Snap to first point (optional)
        Vector3 startPos = lanePath.points[0];
        rb.position = startPos;
        transform.position = startPos;
    }

    void FixedUpdate()
    {
        if (currentIndex >= lanePath.points.Count)
        {
            if (loopPath)
                currentIndex = 0;
            else
                return;
        }

        Vector3 target = lanePath.points[currentIndex];
        Vector3 currentPos = rb.position;

        // Direction towards target on horizontal plane only
        Vector3 direction = (target - currentPos);
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance < closeEnoughDistance)
        {
            currentIndex++;
            return;
        }

        Vector3 dirNormalized = direction.normalized;

        // Calculate movement step without overshooting target
        float step = speed * Time.fixedDeltaTime;
        if (step > distance)
            step = distance;

        Vector3 move = currentPos + dirNormalized * step;

        // Move rigidbody smoothly
        rb.MovePosition(move);

        // Rotate smoothly toward movement direction
        if (dirNormalized != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirNormalized);
            Quaternion newRot = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRot);
        }
    }
}
