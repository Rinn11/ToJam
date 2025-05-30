/*
 * Interface that PlayerMove uses to determine its statistics
 * See: AlcoholManager.cs and CopManager.cs
 */

using UnityEngine;

public interface IMovementModifier
{
    float GetAccelerationMultiplier();
    float GetReverseMultiplier();
    float GetBrakeMultiplier();
    float GetTurnMultiplier();
    float GetMaxSpeedMultiplier();
}
