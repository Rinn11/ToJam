using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour
{
  public float accelerationForce, brakeForce, turnTorque, maxSpeed;

  private Rigidbody rb;
  private Vector2 moveValue;

  void Start()
  {
    rb = GetComponent<Rigidbody>();
    if (rb == null)
    {
      Debug.LogError("Rigidbody component not found on this GameObject. Please add one.");
      enabled = false; // Disable the script if no Rigidbody is found
    }
    else
    {
      rb.maxLinearVelocity = maxSpeed;
    }
  }

  void Update()
  {
    if (rb == null) return;

    moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();

    float isMoving = rb.linearVelocity.magnitude != 0 ? 1.0f : 0.0f;

    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, moveValue.x * turnTorque * Time.deltaTime * isMoving, 0f));

    transform.position = rb.transform.position;
  }

  void FixedUpdate()
  {
    if (rb == null) return;

    // --- Acceleration and Braking ---
    if (moveValue.y > 0)
    {
      rb.AddForce(transform.forward * moveValue.y * accelerationForce);
    }
    else if (moveValue.y < 0)
    {
      // If moving forward, apply brake force. If moving backward or stationary, apply reverse force.
      if (Vector3.Dot(rb.linearVelocity, transform.forward) > 0.1f) // Check if moving forward
      {
        rb.AddForce(transform.forward * moveValue.y * brakeForce);
      }
      else
      {
        rb.AddForce(transform.forward * moveValue.y * accelerationForce * 0.7f); // reverse is 70% of forward
      }
    }
  }
}
