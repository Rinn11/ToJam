using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements; // Not strictly needed for this specific UI positioning, but kept as it was in your original script.

public class lassoFire : MonoBehaviour
{
    public Transform target;            // Target to check
    private Rigidbody TarRB;

    public float maxDistance = 10f;     // Max distance force applicable
    public float forceMult = 1f;        // Linear multiplier for force

    public LayerMask obstacleMask;      // What counts as an obstacle

    [SerializeField] private PlayerInput playerInput;

    public GameObject particleSystem;   // Particle system to activate during ability usage

    public GameObject lockOnIndicator;  // Indicator that tells cop when they can use their ability
    public Camera uiCamera;             // Camera the HUD's parent canvas is tied to

    public Boolean CurrentlyPulling = false;

    private void Start()
    {
        TarRB = target.GetComponent<Rigidbody>();
        ParticleSystem ps = particleSystem.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startSize = maxDistance * 2;

        // Initially hide the indicator and particle system
        particleSystem?.SetActive(false);
        lockOnIndicator?.SetActive(false);
    }

    void Update()
    {
        if (target == null) return;
        if (playerInput == null) return;
        if (uiCamera == null) return; // Ensure the camera is available for positioning

        InputAction ability2Action = playerInput.actions["Ability2"];
        if (ability2Action == null) return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        // Only when starting a new pulling sequence
        if (ability2Action.WasPressedThisFrame() && !CurrentlyPulling)
        {
            CurrentlyPulling = true;
        }

        // Looped logic during pulling sequence
        if (CurrentlyPulling)
        {
            particleSystem?.SetActive(true);
            lockOnIndicator?.SetActive(true);

            // Calculate the 2D screen position of the 3D target
            Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position);

            RectTransform indicatorRectTransform = lockOnIndicator.GetComponent<RectTransform>();
            RectTransform canvasRectTransform = lockOnIndicator.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            // Adjust the 2D screen position to the parent canvas
            Vector2 localPointerPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, uiCamera, out localPointerPos))
            {
                indicatorRectTransform.anchoredPosition = localPointerPos;
            }

            lockOnIndicator.SetActive(true);

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
                else
                {
                    // If an obstacle is hit, stop pulling
                    CurrentlyPulling = false;
                }
            }
            else
            {
                // If target goes out of range, stop pulling
                CurrentlyPulling = false;
            }
        }
        else
        {
            particleSystem?.SetActive(false);
            lockOnIndicator.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Once target collides with cop, stop pulling
            CurrentlyPulling = false;
            return;
        }
    }
}