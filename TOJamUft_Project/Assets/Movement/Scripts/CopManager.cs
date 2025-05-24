/*
 * Manages the statistics of the cop car
 */


/*
 * Implements IMovementModifier, which specifies the statistics of the car.
 */

using UnityEngine;

public class CopManager : MonoBehaviour, IMovementModifier
{
    // TODO: Replace these as we develop what we want to do with the cop more
    public float GetAccelerationMultiplier() => 1f;
    public float GetReverseMultiplier() => 0.7f;
    public float GetBrakeMultiplier() => 1f;
    public float GetTurnMultiplier() => 1f;
    public float GetMaxSpeedMultiplier() => 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
