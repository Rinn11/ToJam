using UnityEngine;

public class WheelSettings : MonoBehaviour
{
    [Header("Suspension Settings")]
    public float suspensionDistance = 0.3f;
    public float wheelDampingRate = 0.25f;
    public float forceAppPointDistance = 0f;
    public Vector3 center = Vector3.zero;

    [Header("Suspension Spring")]
    public float spring = 35000f;
    public float damper = 4500f;
    public float targetPosition = 0.5f;

    [Header("Forward Friction")]
    public float forwardExtremumSlip = 0.4f;
    public float forwardExtremumValue = 1f;
    public float forwardAsymptoteSlip = 0.8f;
    public float forwardAsymptoteValue = 0.5f;
    public float forwardStiffness = 1f;

    [Header("Sideways Friction")]
    public float sidewaysExtremumSlip = 0.2f;
    public float sidewaysExtremumValue = 1f;
    public float sidewaysAsymptoteSlip = 0.5f;
    public float sidewaysAsymptoteValue = 0.75f;
    public float sidewaysStiffness = 4f;

    void Start()
    {
        WheelCollider[] wheelColliders = GetComponentsInChildren<WheelCollider>();

        foreach (WheelCollider wc in wheelColliders)
        {
            wc.suspensionDistance = suspensionDistance;
            wc.wheelDampingRate = wheelDampingRate;
            wc.forceAppPointDistance = forceAppPointDistance;
            wc.center = center;

            JointSpring suspensionSpring = wc.suspensionSpring;
            suspensionSpring.spring = spring;
            suspensionSpring.damper = damper;
            suspensionSpring.targetPosition = targetPosition;
            wc.suspensionSpring = suspensionSpring;

            WheelFrictionCurve forwardFriction = wc.forwardFriction;
            forwardFriction.extremumSlip = forwardExtremumSlip;
            forwardFriction.extremumValue = forwardExtremumValue;
            forwardFriction.asymptoteSlip = forwardAsymptoteSlip;
            forwardFriction.asymptoteValue = forwardAsymptoteValue;
            forwardFriction.stiffness = forwardStiffness;
            wc.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
            sidewaysFriction.extremumSlip = sidewaysExtremumSlip;
            sidewaysFriction.extremumValue = sidewaysExtremumValue;
            sidewaysFriction.asymptoteSlip = sidewaysAsymptoteSlip;
            sidewaysFriction.asymptoteValue = sidewaysAsymptoteValue;
            sidewaysFriction.stiffness = sidewaysStiffness;
            wc.sidewaysFriction = sidewaysFriction;
        }
    }
}