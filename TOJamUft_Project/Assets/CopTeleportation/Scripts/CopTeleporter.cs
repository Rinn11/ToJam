using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class CopTeleporter : MonoBehaviour
{
    public float speed = 0.25f;     // The speed of the cursor when being moved with the joystick

    public GameObject CopCar;       // The cop car that is teleported
    public GameObject Selector;

    public Camera CopMinimapCamera;

    public AlertDDOfCopLocationEventSender AlertDDOfCopLocationEventSender;

    [SerializeField] private PlayerInput playerInput;

    private bool state = false;

    private int teleportMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int teleportLayer = LayerMask.NameToLayer("CopTeleportationElements");
        teleportMask = 1 << teleportLayer;
        CopMinimapCamera.cullingMask &= ~teleportMask;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput == null) return;
        InputAction abilityAction = playerInput.actions["Ability"];
        if (abilityAction == null) return;

        if (abilityAction.WasPerformedThisFrame()) {
            // When the key is pressed
            state = !state;
            if (state) {
                // Show layer relavant to selection
                CopCar.GetComponent<PlayerControl>().SetLocked(true); // TODO: Refactor
                CopMinimapCamera.cullingMask |= teleportMask;
            }
            else {
                // Hide layer relevant to selection
                CopCar.GetComponent<PlayerControl>().SetLocked(false); // TODO: Refactor
                CopMinimapCamera.cullingMask &= ~teleportMask;

                // Find closest marked car
                GameObject closestCar = null;
                float minDist = Mathf.Infinity;

                GameObject[] markedCars = GameObject.FindGameObjectsWithTag("MarkedCar");
                foreach (GameObject car in markedCars) {
                    Vector3 diff = car.transform.position - Selector.transform.position;
                    float dist = diff.sqrMagnitude;

                    if (dist < minDist) {
                        minDist = dist;
                        closestCar = car;
                    }
                }

                // If a suitable car was found
                if (closestCar != null) {
                    // "Stop" both entities in place
                    Rigidbody rb1 = CopCar.GetComponent<Rigidbody>();
                    rb1.linearVelocity = Vector3.zero;
                    rb1.angularVelocity = Vector3.zero;

                    Rigidbody rb2 = closestCar.GetComponent<Rigidbody>();
                    rb2.linearVelocity = Vector3.zero;
                    rb2.angularVelocity = Vector3.zero;

                    // Swap positions and angles
                    Vector3 TempPos = CopCar.transform.position;
                    CopCar.transform.position = closestCar.transform.position;
                    closestCar.transform.position = TempPos;

                    Quaternion tempRot = CopCar.transform.rotation;
                    CopCar.transform.rotation = closestCar.transform.rotation;
                    closestCar.transform.rotation = tempRot;

                    // Set the crosshair to the new position to make it easier for next use
                    Selector.transform.position = CopCar.transform.position;

                    AlertDDOfCopLocationEventSender.Trigger(new Vector2(CopCar.transform.position.x, CopCar.transform.position.z)); // Alert drunk driver of cop
                }
            }
        }

        // Running the cursor slew
        if (state) {
            // Always pull from current action map
            if (playerInput == null) return;
            InputAction camAction = playerInput.actions["Camera"];
            if (camAction == null) return;

            Vector2 slew = camAction.ReadValue<Vector2>();

            Vector3 delta = new Vector3(slew.x, 0f, slew.y);
            Selector.transform.position += delta * speed;   
        }
    }
}
