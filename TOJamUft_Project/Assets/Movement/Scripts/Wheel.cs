using UnityEngine;

public class Wheel : MonoBehaviour
{
  public bool powered;
  public float maxAngle = 90f;

  private float turnAngle;
  private WheelCollider wcol;
  private Transform wmesh;

  private void Start()
  {
    wcol = GetComponentInChildren<WheelCollider>();
    wmesh = transform.Find("wheel-mesh");
  }

  public void Steer(float x)
  {
    turnAngle = x * maxAngle;
    wcol.steerAngle = turnAngle;
  }

  public void Accelerate(float force)
  {
    if (powered)
    {
      wcol.motorTorque = force;
    }
    else
    {
      wcol.motorTorque = 0; // Explicitly set to 0 if not powered
    }
    wcol.brakeTorque = 0; // Always release brake when accelerating
  }

  public void Brake(float force)
  {
    wcol.motorTorque = 0;
    wcol.brakeTorque = force;
  }

  public void UpdatePos()
  {
    Vector3 pos = transform.position;
    Quaternion rot = transform.rotation;

    wcol.GetWorldPose(out pos, out rot);
    wmesh.transform.position = pos;
    wmesh.transform.rotation = rot;
  }
}
