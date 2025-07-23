using UnityEngine;

public class TeleportationTunnel : MonoBehaviour
{
    public GameObject ExitTunnel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider Other)
    {
        if (Other.tag == "CopCar") {
            Debug.Log("Teleporting Cop");
            Teleport(Other.gameObject, ExitTunnel);
        }
    }

    void Teleport(GameObject Cop, GameObject OtherTunnel)
    {
        Rigidbody RB = Cop.GetComponent<Rigidbody>();

        // Store original speed
        float TempSpeed = RB.linearVelocity.magnitude;

        // Teleport to the new location
        Vector3 StartPos = Cop.transform.position;
        Quaternion StartRot = Cop.transform.rotation;

        Vector3 GroundDir = OtherTunnel.transform.up;
        RB.MovePosition(OtherTunnel.transform.position + GroundDir * 10);

        Quaternion GroundRot = Quaternion.LookRotation(GroundDir, Vector3.up);
        RB.MoveRotation(GroundRot);

        // Reapply original speed at new direction
        RB.linearVelocity = GroundDir * TempSpeed;
    }
}
