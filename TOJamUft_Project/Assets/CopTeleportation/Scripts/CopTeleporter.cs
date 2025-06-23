using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CopTeleporter : MonoBehaviour
{
    public GameObject CopCar;       // The cop car that is teleported
    public GameObject DDCar;        // The drunk driver to aim at

    public GameObject[] TeleportLocations;

    public AlertDDOfCopLocationEventSender AlertDDOfCopLocationEventSender;

    [SerializeField] private PlayerInput playerInput;

    // Update is called once per frame
    Vector2 last = Vector2.zero;

    void Update()
    {
        if (playerInput == null) return;

        var dpad = playerInput.actions["Camera"];
        Vector2 curr = dpad.ReadValue<Vector2>();
        Debug.Log("DPAD" + curr);

        if (curr == Vector2.up && last != Vector2.up) {
            Debug.Log("Dpad_Up");
            Teleport(CopCar, DDCar, TeleportLocations[0].transform.position);
        }
        if (curr == Vector2.left && last != Vector2.left) {
            Debug.Log("Dpad_Left");
            Teleport(CopCar, DDCar, TeleportLocations[1].transform.position);
        }
        if (curr == Vector2.down && last != Vector2.down)
        {
            Debug.Log("Dpad_Down");
            Teleport(CopCar, DDCar, TeleportLocations[2].transform.position);
        }
        if (curr == Vector2.right && last != Vector2.right)
        {
            Debug.Log("Dpad_Right");
            Teleport(CopCar, DDCar, TeleportLocations[3].transform.position);
        }

        last = curr;
    }

    // Teleports the Cop to Location, looking towards Driver
    void Teleport(GameObject Cop, GameObject Driver, Vector3 Location) {
        Rigidbody RB = Cop.GetComponent<Rigidbody>();

        // Store original speed
        float TempSpeed = RB.linearVelocity.magnitude;

        // Teleport to the new location
        Cop.transform.position = Location;

        // Angle towards DD on the horizontal plane
        Vector3 GroundDir = Driver.transform.position - Cop.transform.position; // Angle from cop to driver
        GroundDir.y = 0;
        GroundDir.Normalize();

        Quaternion GroundRot = Quaternion.LookRotation(GroundDir, Vector3.up);
        Cop.transform.rotation = GroundRot;

        // Reapply original speed at new direction
        RB.linearVelocity = GroundDir * TempSpeed;

        AlertDDOfCopLocationEventSender.Trigger(new Vector2(CopCar.transform.position.x, CopCar.transform.position.z)); // Alert drunk driver of cop
    }

    // Swaps the locations, orientations, velocities and angular velocities of two objects
    void Swap(GameObject Obj1, GameObject Obj2) {
        Rigidbody RB1 = Obj1.GetComponent<Rigidbody>();
        Rigidbody RB2 = Obj2.GetComponent<Rigidbody>();

        // Swap positions
        Vector3 TempPos = Obj1.transform.position;
        Obj1.transform.position = Obj2.transform.position;
        Obj2.transform.position = TempPos;

        // Swap rotations
        Quaternion TempRot = Obj1.transform.rotation;
        Obj1.transform.rotation = Obj2.transform.rotation;
        Obj2.transform.rotation = TempRot;

        // Swap velocities
        Vector3 TempVel = RB1.linearVelocity;
        RB1.linearVelocity = RB2.linearVelocity;
        RB2.linearVelocity = TempVel;

        // Swap angular velocities
        Vector3 TempAngVel = RB1.angularVelocity;
        RB1.angularVelocity = RB2.angularVelocity;
        RB2.angularVelocity = TempAngVel;
    }
}
