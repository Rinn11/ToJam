using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
  public float accelerationForce, brakeForce, turnTorque, maxSpeed;
  // recommended default 50, 50, 100, 20
  private Rigidbody rb;
  private Vector2 moveValue;

  private AlcoholManager _alcoholManager;

  private InputAction moveAction;

  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    if (rb == null)
    {
      Debug.LogError("Rigidbody component not found on this GameObject. Please add one.", this);
      return;
    }

    var inputActionAsset = InputSystem.actions;
    if (inputActionAsset != null)
    {
      moveAction = inputActionAsset.FindAction("Move");
    }

    if (moveAction == null)
    {
      Debug.LogError("Move action not found! Please ensure it's defined in your Input Actions asset.");
      enabled = false;
      return;
    }
    moveAction.Enable();
  }

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


    GameObject alcoholGameObject = GameObject.Find("Alcohol");
    if (alcoholGameObject != null)
    {
      _alcoholManager = alcoholGameObject.GetComponent<AlcoholManager>();
    }
    else
    {
      Debug.LogError("GameObject 'Alcohol' not found!");
    }
  }

  void Update()
  {
    if (rb == null || moveAction == null) return;

    moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();



    float alcoholMultiplier = _alcoholManager.GetAlcoholMultiplier();
    rb.maxLinearVelocity = maxSpeed * (alcoholMultiplier * 2); // note maxSpeed is a constant.


    // float useTurnTorque = turnTorque * (float)Math.Pow(1.6, alcoholMultiplier) * (alcoholMultiplier / 2);  // sensitivity increases with alcohol
    // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles +
    //                                       new Vector3(0f, moveValue.x * useTurnTorque * Time.deltaTime * isMoving, 0f));
  }

  void FixedUpdate()
  {
    if (rb == null) return;

    // --- Acceleration and Braking ---
    float alcoholMultiplier = _alcoholManager.GetAlcoholMultiplier();

    float useAccelerationForce = accelerationForce * (alcoholMultiplier * 2);
    float useBrakeForce = brakeForce * (alcoholMultiplier);

    float isMoving = Math.Abs(rb.linearVelocity.magnitude) > 1e-8 ? 1.0f : 0.0f;

    // float currentSpeedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / (maxSpeed / 4f));
    float useTurnTorquePhysical = turnTorque * (float)Math.Pow(1.6, alcoholMultiplier) * (alcoholMultiplier / 2f); // May need to scale this value significantly
    rb.AddTorque(Vector3.up * moveValue.x * useTurnTorquePhysical * Time.fixedDeltaTime * isMoving * 1000f);


    if (moveValue.y > 0)
    {
      rb.AddForce(transform.forward * moveValue.y * useAccelerationForce);
    }
    else if (moveValue.y < 0)
    {
      // If moving forward, apply brake force. If moving backward or stationary, apply reverse force.
      if (Vector3.Dot(rb.linearVelocity, -transform.up) > 0.1f) // Check if moving forward
      {
        rb.AddForce(transform.forward * moveValue.y * useBrakeForce);
      }
      else
      {
        rb.AddForce(transform.forward * moveValue.y * useAccelerationForce * 0.7f); // reverse is 70% of forward
      }
    }
  }
}
