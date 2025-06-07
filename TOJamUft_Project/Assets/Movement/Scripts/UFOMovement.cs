/*
 * Simulates the movement of a player's car.
 */

// TODO: remove coupling with alcohol Manager

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UFOMovement : MonoBehaviour, IMovementModel
{
    public float accelerationForce, turnTorque, maxSpeed, maxTurnSpeed;
    public float dragForce, angularDragForce;

    private Rigidbody rb;
    private Vector2 moveValue;

    public Text speedUI;
    public GameObject MovementManager;
    private IMovementModifier movementModifier;

    public UnityEvent onAccelerate;
    public UnityEvent onBrake;
    public UnityEvent onIdle;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set physical limits
        rb.maxLinearVelocity = maxSpeed;
        rb.maxAngularVelocity = maxTurnSpeed;
        rb.linearDamping = dragForce;
        rb.angularDamping = angularDragForce;

        movementModifier = MovementManager.GetComponent<IMovementModifier>();
    }

    public void ProcessInputs(float x, float y)
    {
        if (rb == null) return;

        moveValue.x = x;
        moveValue.y = y;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // --- Acceleration and Braking ---
        float useAccelerationForce = accelerationForce * movementModifier.GetAccelerationMultiplier();
        float useTurnTorque = turnTorque * movementModifier.GetTurnMultiplier(); // sensitivity increases with alcohol
        float useReverseForce = accelerationForce * movementModifier.GetReverseMultiplier();
        rb.maxLinearVelocity = maxSpeed * movementModifier.GetMaxSpeedMultiplier();
        rb.maxAngularVelocity = maxTurnSpeed * movementModifier.GetMaxSpeedMultiplier(); // Adjust angular velocity to be half of linear velocity

        // Coupled but atleast speedUI being unspecified won't break the script.
        if (speedUI != null)
        {
            float speed = rb.linearVelocity.magnitude;
            speedUI.text = $"Speed: {Mathf.RoundToInt(speed)} km/h";
        }

        if (moveValue.x != 0)
        {
            rb.AddTorque(Vector3.up * useTurnTorque * moveValue.x);
        }

        if (moveValue.y != 0)
        {
            rb.AddForce(transform.forward * moveValue.y * useAccelerationForce);
        }
        
    }
}
