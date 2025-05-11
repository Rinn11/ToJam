using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float accelerationForce, brakeForce, turnTorque, maxSpeed;
    // recommended default 50, 50, 100, 20
    private Rigidbody rb;
    private Vector2 moveValue;
    private GameObject speedUI;
    
    private AlcoholManager _alcoholManager;
    private GameObject wheel;
    
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
        
        speedUI = GameObject.Find("SpeedUI");
        
        
        GameObject alcoholGameObject = GameObject.Find("Alcohol");
        if (alcoholGameObject != null)
        {
            _alcoholManager = alcoholGameObject.GetComponent<AlcoholManager>();
        }
        else
        {
            Debug.LogError("GameObject 'Alcohol' not found!");
        }
        
        wheel = GameObject.Find("M_CarWheel");
        
        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            Debug.LogError("Add at least two AudioSource components to this GameObject.");
        }
    }

    void Update()
    {
        if (rb == null) return;

        moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();

        float isMoving = rb.linearVelocity.magnitude != 0 ? 1.0f : 0.0f;

        float alcoholMultiplier = _alcoholManager.GetAlcoholMultiplier();
        rb.maxLinearVelocity = maxSpeed * (alcoholMultiplier * 2); // note maxSpeed is a constant.
        
        
        float useTurnTorque = turnTorque * (float)Math.Pow(1.6, alcoholMultiplier) * (alcoholMultiplier / 2);  // sensitivity increases with alcohol
        UnityEngine.Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles + 
                                              new Vector3(0f, moveValue.x * useTurnTorque * Time.deltaTime * isMoving, 0f));
        transform.rotation = rot;
        // wheel.transform.rotation.z = transform.rotation.eulerAngles.z;
        
        transform.position = rb.transform.position;
        
    }

    public void startGame()
    {
        Time.timeScale = 1.0f;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // --- Acceleration and Braking ---
        float alcoholMultiplier = _alcoholManager.GetAlcoholMultiplier();

        float useAccelerationForce = accelerationForce * (alcoholMultiplier * 2);
        float useBrakeForce = brakeForce * (alcoholMultiplier);
        
        if (speedUI != null)
        {
            // Assuming the alcoholCounterUI has a Text component
            var textComponent = speedUI.GetComponent<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                textComponent.text = "Speed: " + Math.Round(rb.linearVelocity.magnitude, 2) + " km/h";
            }
        }
        
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
