using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class lassoFire : MonoBehaviour
{
    public Transform target;            // Target to check
    private Rigidbody TarRB;

    public float maxDistance = 10f;     // Max distance force applicable
    public float forceMult = 0.1f;      // Linear multiplier for force

    public LayerMask obstacleMask;      // What counts as an obstacle

    [SerializeField] private PlayerInput playerInput;

    public GameObject ParticleSystem;

    private void Start()
    {
        TarRB = target.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (target == null) return;

        if (playerInput == null) return;
        InputAction ability2Action = playerInput.actions["Ability2"];
        if (ability2Action == null) return;

        if (ability2Action.IsPressed())
        {
            ParticleSystem.SetActive(true);

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
                }
                else
                {
                    // 4. Clear line of sight
                    Debug.DrawRay(transform.position, direction, Color.green);
                    TarRB.AddForce(-direction.normalized * forceMult * (distance * distance));
                }
            }
            else
            {
                // Target out of range
            }
        }
        else
        {
            ParticleSystem.SetActive(false);
        }
    }
}
