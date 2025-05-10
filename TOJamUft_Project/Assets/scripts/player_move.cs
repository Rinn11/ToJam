using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour
{
    public float accelerationForce = 1f;
    public float brakeForce = 1f;
    public float turnTorque = 1f;
    public float maxSpeed = 100f; // Optional: to limit car speed

    private Rigidbody rb;

    InputAction moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on this GameObject. Please add one.");
            enabled = false; // Disable the script if no Rigidbody is found
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        rb.maxLinearVelocity = 30f;

        moveInput = InputSystem.actions.FindAction("Move");
        Vector2 moveValue = moveInput.ReadValue<Vector2>();

        // --- Acceleration and Braking ---
        if (moveValue.y > 0)
        {
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddRelativeForce(Vector3.forward * moveValue.y * accelerationForce);
            }
        }
        else if (moveValue.y < 0)
        {
            // If moving forward, apply brake force. If moving backward or stationary, apply reverse force.
            if (Vector3.Dot(rb.linearVelocity, transform.forward) > 0.1f) // Check if moving forward
            {
                 rb.AddRelativeForce(Vector3.forward * moveValue.y * brakeForce);
            }
            else
            {
                 rb.AddRelativeForce(Vector3.forward * moveValue.y * accelerationForce * 0.7f); // reverse is 70% of forward
            }
        }

        // --- Steering ---
        // Apply torque for turning
        // The ForceMode can be adjusted (e.g., ForceMode.VelocityChange for more instant turns, ForceMode.Acceleration for smoother)
        rb.AddRelativeTorque(Vector3.up * moveValue.x * turnTorque, ForceMode.Acceleration);
    }
}