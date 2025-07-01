using System;
using TMPro;
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

    public float cooldown = 5f;         // Minimum Time after target was lost to use the ability again
    private float lastLostTime = -999;  // Timestamp of time when target was lost

    public TMP_Text cooldownTimerText;  // Timer text representing the number of seconds left before next use

    private bool currentlyPulling = false;
    
    public bool GetIsPulling()
    {
        return currentlyPulling;
    }

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
        if (uiCamera == null) return;

        InputAction ability2Action = playerInput.actions["Ability2"];
        if (ability2Action == null) return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance <= maxDistance)
        {
            // If target is within range
            Ray ray = new Ray(transform.position, direction.normalized);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, distance, obstacleMask))
            {
                Debug.Log("Raycast" + hit);
                // If target is visible

                // Calculate the 2D screen position of the 3D target
                Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position);

                RectTransform indicatorRectTransform = lockOnIndicator.GetComponent<RectTransform>();
                RectTransform canvasRectTransform = lockOnIndicator.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

                // Adjust the 2D screen position to the parent canvas
                Vector2 localPointerPos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, uiCamera, out localPointerPos))
                {
                    // Avoid showing if behind camera
                    lockOnIndicator?.SetActive(screenPos.z > 0);
                    indicatorRectTransform.anchoredPosition = localPointerPos;
                }

                // Start pulling sequence if not already in one
                float TimeSinceLastLoss = Time.time - lastLostTime;
                cooldownTimerText.text = "" + Math.Round(Math.Max(0, cooldown - TimeSinceLastLoss));

                bool hasCooledDown = TimeSinceLastLoss >= cooldown;
                cooldownTimerText.color = hasCooledDown ? Color.green : Color.red;

                if (ability2Action.WasPressedThisFrame() && !currentlyPulling && hasCooledDown)
                {
                    currentlyPulling = true;
                }

                // Main looping logic
                if (currentlyPulling)
                {
                    particleSystem.SetActive(true);
                    Debug.DrawRay(transform.position, direction, Color.green);
                    TarRB.AddForce(-direction.normalized * forceMult * 1000000 / (distance * distance));
                }
            }
            else
            {
                // Target no longer visible
                OnTargetLost();
            }            
        }
        else
        {
            // Target out of range
            OnTargetLost();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Once target collides with cop, stop pulling
            OnTargetLost();
            return;
        }
    }

    private void OnTargetLost() {
        if (currentlyPulling)
        {
            lastLostTime = Time.time;
            currentlyPulling = false;
        }
        particleSystem?.SetActive(false);
        lockOnIndicator?.SetActive(false);
    }
}