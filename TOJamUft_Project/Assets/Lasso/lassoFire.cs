using UnityEngine;

public class lassoFire : MonoBehaviour
{
    public Transform target;            // Target to check
    UFOMovement uFOMovement;

    public float maxDistance = 10f;     // Max distance to check
    public LayerMask obstacleMask;      // What counts as an obstacle

    public float acceleration = 0.75f;

    private void Start()
    {
        uFOMovement = target.GetComponent<UFOMovement>();
    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        // 1. Check if within range
        if (distance <= maxDistance)
        {
            Ray ray = new Ray(transform.position, direction.normalized);
            RaycastHit hit;

            // 2. Perform the raycast
            if (Physics.Raycast(ray, out hit, distance, obstacleMask))
            {
                // 3. Something is in the way
                Debug.Log($"Obstacle in the way: {hit.collider.name}");
            }
            else
            {
                uFOMovement.maxSpeed*=acceleration;
                // 4. Clear line of sight
                Debug.Log("Target is visible and within range.");
            }
        }
        else
        {
            Debug.Log("Target is too far.");
        }
    }
}
