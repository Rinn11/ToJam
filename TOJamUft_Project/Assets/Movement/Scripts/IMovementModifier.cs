/*
 * Interface for affecting the statistics of a car
 */

using UnityEngine;

public interface IMovementModifier
{
    float GetAccelerationMultiplier();
    float GetReverseAccelerationMultiplier();
    float GetBrakeMultiplier();
    float GetTurnMultiplier();
    float GetMaxSpeedMultiplier();
}
