/*
 * Simulates the movement of a player's car.
 */

// TODO: remove coupling with alcohol Manager

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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

    private bool accerated = true;

    public GameObject speedParticles;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set physical limits
        rb.maxLinearVelocity = maxSpeed;
        rb.maxAngularVelocity = maxTurnSpeed;
        rb.linearDamping = dragForce;
        rb.angularDamping = angularDragForce;

        movementModifier = MovementManager.GetComponent<IMovementModifier>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        PlaySound(idleClip, "Idle");
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
            rb.AddTorque(Vector3.up * moveValue.x * useTurnTorque);
        }

        if (moveValue.y != 0)
        {
            rb.AddForce(transform.forward * moveValue.y * useAccelerationForce);
        }


        float deltaSpeed = rb.linearVelocity.magnitude - previousSpeed;

        if (rb.linearVelocity.magnitude < speedThreshold)
        {
            PlaySound(idleClip, "Idle");
            accerated = true;
        }
        else if (deltaSpeed > changeThreshold && accerated && moveValue.y >0)
        {
            PlaySound(accelerateClip, "Accelerate", false);
            accerated = false;
        }
        else if (moveValue.y <= 0)
        {
            PlaySound(decelerateClip, "Decelerate", false);
            accerated = true;
        }
        else if (!audioSource.isPlaying && moveValue.y > 0)
        {
            PlaySound(moveClip, "Move");
        }
    }
    
        
    public void speedBoost(float boost)
    {
        // this function gets called once a frame if boost is being held, so apply a large force
        if (rb == null) return;
        // shoould move at consistent velocity
        rb.AddForce(transform.forward * boost * accelerationForce, ForceMode.Acceleration);

        //speedParticles.SetActive(true);

        //if (boost >= 0) { speedParticles.SetActive(false); }
    }

    public AudioClip idleClip;
    public AudioClip accelerateClip;
    public AudioClip moveClip;
    public AudioClip decelerateClip;

    public float speedThreshold = 10f;
    public float changeThreshold = 30f;

    private AudioSource audioSource;
    private float previousSpeed = 0f;
    private string currentState = "";

    void PlaySound(AudioClip clip, string stateName, bool loop = true)
    {
        if (currentState == stateName) return;

        currentState = stateName;
        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();
    }
}


