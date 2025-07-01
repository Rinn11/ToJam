using UnityEngine;

public class shakeBottle : MonoBehaviour
{
    // should have a reference to the AlcoholManager
    public float shakeDuration = 1.0f; // Duration of the shake effect
    
    private float shakeTimer = 0f; // Timer to track the shake duration

    public void setShakeTimer() 
    {
        shakeTimer = shakeDuration; // Set the shake timer to the specified duration
    }
    
    public bool GetIsShaking()
    {
        return shakeTimer > 0f; // Returns true if the bottle is currently shaking
    }
    
    void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime; // Increment the timer
            // Apply the shake effect (shakes left right on the x-axis)
            // use relative anchor rotation
            float shakeAmount = Mathf.Sin(Time.time * 20f) * 5.0f; // Adjust the shake intensity
            transform.localRotation = Quaternion.Euler(0f, 0f, shakeAmount); // Apply the shake rotation
            
        }
        else
        {
            shakeTimer = 0f; // Ensure timer does not go negative
            // Reset the rotation to the original state
            transform.localRotation = Quaternion.identity; // Reset to original rotation
        }
    }
}
