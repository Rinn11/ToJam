/*
 * Simulates the movement of a player's car.
 */

// TODO: remove coupling with alcohol Manager

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour, IMovementModel
{
    [Header("Movement Settings")]
    public float accelerationForce, brakeForce, turnTorque, maxSpeed, maxTurnSpeed;
    // recommended default 50, 50, 50, 20, 10

    private Rigidbody rb;
    private Vector2 moveValue;

    public Text speedUI;
    public GameObject MovementManager;
    private IMovementModifier movementModifier;

    [Header("Audio Events")]
    public UnityEvent onAccelerate;
    public UnityEvent onBrake;
    public UnityEvent onIdle;

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
            rb.maxAngularVelocity = maxTurnSpeed; // Adjust angular velocity to be half of linear velocity
        }

        if (MovementManager == null)
        {
            Debug.LogError("MovementManager not found!");
        }
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
        float useBrakeForce = brakeForce * movementModifier.GetBrakeMultiplier();
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

        if (Mathf.Abs(moveValue.y) > 0)
        {
            //float isMoving = rb.linearVelocity.magnitude != 0 ? 1.0f : 0.0f; // Only apply turning if we're already moving
            float turnDirection = Math.Sign(rb.linearVelocity.magnitude); // This inverts steering when reversing
            rb.AddTorque(Vector3.up * useTurnTorque * turnDirection * moveValue.x);
        }

        if (moveValue.y > 0)
        {
            rb.AddForce(transform.forward * moveValue.y * useAccelerationForce);
            onAccelerate?.Invoke();
        }
        else if (moveValue.y < 0)
        {
            // If moving forward, apply brake force. If moving backward or stationary, apply reverse force.
            if (Vector3.Dot(rb.linearVelocity, transform.forward) > 0.1f) // Check if moving forward
            {
                rb.AddForce(transform.forward * moveValue.y * useBrakeForce);
                onBrake?.Invoke();
            }
            else
            {
                rb.AddForce(transform.forward * moveValue.y * useReverseForce);
                onBrake?.Invoke();
            }
        }
        else
        {
            onIdle?.Invoke();
        }
    }
}
