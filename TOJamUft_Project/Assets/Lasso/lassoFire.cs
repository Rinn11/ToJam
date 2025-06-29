using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class lassoFire : MonoBehaviour
{
    public Transform target;            // Target to check
    private Rigidbody TarRB;

    public float maxDistance = 10f;     // Max distance force applicable
    public float forceMult = 1f;      // Linear multiplier for force

    public LayerMask obstacleMask;      // What counts as an obstacle

    [SerializeField] private PlayerInput playerInput;

    public GameObject particleSystem;   // Particle system to activate during ability usage

    private void Start()
    {
        TarRB = target.GetComponent<Rigidbody>();
        ParticleSystem ps = particleSystem.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startSize = maxDistance * 2;
    }

    void Update()
    {
        if (target == null) return;

        if (playerInput == null) return;
        InputAction ability2Action = playerInput.actions["Ability2"];
        if (ability2Action == null) return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (ability2Action.IsPressed())
        {
            particleSystem.SetActive(true);

            // 1. Check if within range
            if (distance <= maxDistance)
            {
                Ray ray = new Ray(transform.position, direction.normalized);
                RaycastHit hit;

                // 2. Perform the raycast
                if (!Physics.Raycast(ray, out hit, distance, obstacleMask))
                {
                    Debug.DrawRay(transform.position, direction, Color.green);
                    TarRB.AddForce(-direction.normalized * forceMult * 1000000 / (distance * distance));
                }
            }
        }
        else
        {
            particleSystem.SetActive(false);
        }
    }
}
