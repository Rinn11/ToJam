/*
 * Simulates the movement of a player's car.
 */

// TODO: remove coupling with alcohol Manager

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
  public float accelerationForce, brakeForce, turnTorque, maxSpeed;
  // recommended default 50, 50, 40, 20
  private Rigidbody rb;
  private Vector2 moveValue;
  
  public Text speedUI;
  public GameObject AlcoholManager;
  private AlcoholManager alcoholManager;
  
  public AudioSource[] audioSources;

  void Start()
  {
    //Time.timeScale = 0;

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
    
    if (AlcoholManager == null)
    {
      Debug.LogError("AlcoholManager not found!");
    }
    alcoholManager = AlcoholManager.GetComponent<AlcoholManager>();
    
    audioSources = GetComponents<AudioSource>();
    if (audioSources.Length < 2)
    {
      Debug.LogError("Add at least two AudioSource components to this GameObject.");
    }
  }

  public void ProcessInputs(float x, float y)
  {
    if (rb == null) return;

    moveValue.x = x;
    moveValue.y = y;
  }

  public void startGame()
  {
    Time.timeScale = 1.0f;
  }

  void FixedUpdate()
  {
    if (rb == null) return;

    // --- Acceleration and Braking ---
    float alcoholMultiplier = alcoholManager.GetAlcoholMultiplier();

    float useAccelerationForce = accelerationForce * (alcoholMultiplier * 2);
    float useBrakeForce = brakeForce * (alcoholMultiplier);

    if (speedUI != null)
    {
      float speed = rb.linearVelocity.magnitude;
      speedUI.text = $"Speed: {Mathf.RoundToInt(speed)} km/h";
    }
    
    if (Mathf.Abs(moveValue.x) > 0)
    {
      float isMoving = rb.linearVelocity.magnitude != 0 ? 1.0f : 0.0f;

      float useTurnTorque = turnTorque * (float)Math.Pow(1.6, alcoholMultiplier) * (alcoholMultiplier / 2);  // sensitivity increases with alcohol
      rb.AddTorque(Vector3.up * useTurnTorque * isMoving * moveValue.x);
      // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + 
      // new Vector3(0f, moveValue.x * useTurnTorque * Time.deltaTime * isMoving, 0f));
    }
    
    rb.maxLinearVelocity = maxSpeed * (alcoholMultiplier * 2); // note maxSpeed is a constant.
    if (moveValue.y > 0)
    {
      rb.AddForce(transform.forward * moveValue.y * useAccelerationForce);

      if (!audioSources[0].isPlaying)
      {
        audioSources[0].Play();
      }


    }
    else if (moveValue.y < 0)
    {
      // If moving forward, apply brake force. If moving backward or stationary, apply reverse force.
      if (Vector3.Dot(rb.linearVelocity, transform.forward) > 0.1f) // Check if moving forward
      {
        rb.AddForce(transform.forward * moveValue.y * useBrakeForce);
        if (!audioSources[2].isPlaying)
        {
          audioSources[2].Play();
        }
      }
      else
      {
        rb.AddForce(transform.forward * moveValue.y * useAccelerationForce * 0.7f); // reverse is 70% of forward
        if (!audioSources[0].isPlaying)
        {
          audioSources[0].Play();
        }
      }
    }
    else
    {
      audioSources[0].Stop();
      if (!audioSources[1].isPlaying)
      {
        audioSources[1].Play();
      }
    }
  }
}
