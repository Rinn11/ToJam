/*
 * When started, hides the mouse and locks the cursor.
 * When updating, allows the player to rotate the camera using their mouse to look around.
 */

// TODO: Maybe remove the unused commented out code if applicable

using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class MouseLook : MonoBehaviour
{
  public PlayerSwapEventSender swapSender;
  
  public float mouseSensitivity;
  [SerializeField] private string mouseXField;
  [SerializeField] private string mouseYField;

  // Fields for swapping controls. it's a little hacky but it works for now.
  [SerializeField] private string mouseXFieldOpposite;
  [SerializeField] private string mouseYFieldOpposite;

  private Vector2 lookAction;

  private float yRotation = 0f; // Stores the current rotation of the camera

  void Awake()
  {
    /*
var inputActionAsset = InputSystem.actions;
if (inputActionAsset != null)
{
  lookAction = inputActionAsset.FindAction("Look");
}

if (lookAction == null)
{
  Debug.LogError("Move action not found! Please ensure it's defined in your Input Actions asset.");
  enabled = false;
  return;
}
lookAction.Enable();
    */
    lookAction = new Vector2(Input.GetAxis(mouseXField), Input.GetAxis(mouseYField));
  }

  /* This is Unity's new Input System code. it won't work anymore so it will be commented out and removed later.
void OnEnable()
{
  lookAction.Enable();
}

void OnDisable()
{
  lookAction.Disable();
} */

  void Start()
  {
    // transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
    // Lock the cursor to the center of the screen and make it invisible
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update()
  {
    lookAction = new Vector2(Input.GetAxis(mouseXField), Input.GetAxis(mouseYField));
    Vector2 mouseDelta = lookAction * mouseSensitivity * Time.deltaTime;

    // --- Vertical Look (Pitch) ---
    // Adjust the xRotation based on mouse Y input
    // Subtracting because mouseY positive is typically upwards, and we want to rotate camera down for that.
    // xRotation -= mouseDelta.y;
    // Clamp the vertical rotation to prevent over-rotation (e.g., looking completely upside down)
    // xRotation = Mathf.Clamp(xRotation, -89f, 89f);

    yRotation += mouseDelta.x;
    yRotation = Mathf.Clamp(yRotation, -89f, 89f);

    // Apply the vertical rotation to the camera itself
    transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
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
}