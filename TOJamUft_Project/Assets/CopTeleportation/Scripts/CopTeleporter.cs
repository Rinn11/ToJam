using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class CopTeleporter : MonoBehaviour
{
    public float speed = 0.25f;     // The speed of the cursor when being moved with the joystick

    public GameObject CopCar;       // The cop car that is teleported

    public Camera CopMinimapCamera;

    public AlertDDOfCopLocationEventSender AlertDDOfCopLocationEventSender;

    private bool state = false;

    private int teleportMask;

    // Swap stuff (we should refactor this later...)
    public PlayerSwapEventSender swapSender;
    [SerializeField] private string mouseXField;
    [SerializeField] private string mouseYField;
    [SerializeField] private string mouseXFieldOpposite;
    [SerializeField] private string mouseYFieldOpposite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int teleportLayer = LayerMask.NameToLayer("CopTeleportationElements");
        teleportMask = 1 << teleportLayer;
        CopMinimapCamera.cullingMask &= ~teleportMask;
    }

    private void OnEnable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent += recievePlayerSwap;
    }

    private void OnDisable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent -= recievePlayerSwap;
    }

    public void recievePlayerSwap(bool isPlayer1DrunkDriver)
    {
        // Swap the mouse X and Y axes
        string tempMouseX = mouseXField;
        string tempMouseY = mouseYField;

        mouseXField = mouseXFieldOpposite;
        mouseYField = mouseYFieldOpposite;

        mouseXFieldOpposite = tempMouseX;
        mouseYFieldOpposite = tempMouseY;
        Debug.Log($"Swapped mouse controls: {mouseXField}, {mouseYField} <-> {mouseXFieldOpposite}, {mouseYFieldOpposite}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
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
                    Vector3 diff = car.transform.position - transform.position;
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
                    transform.position = CopCar.transform.position;

                    AlertDDOfCopLocationEventSender.Trigger(new Vector2(CopCar.transform.position.x, CopCar.transform.position.z)); // Alert drunk driver of cop
                }
            }
        }

        // Main loop
        if (state) {
            Vector3 delta = new Vector3(Input.GetAxis(mouseXField), 0f, Input.GetAxis(mouseYField));
            transform.position += delta * speed;   
        }
    }
}
