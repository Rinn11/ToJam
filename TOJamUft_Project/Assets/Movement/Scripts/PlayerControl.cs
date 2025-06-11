using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public MonoBehaviour movementScript; // Drag any script that implements IMovementModel
    private IMovementModel movementModel;

    [SerializeField] private PlayerInput playerInput;

    private bool locked = false;

    private void Start()
    {
        movementModel = movementScript as IMovementModel;
    }

    public void SetLocked(bool newLocked)
    {
        locked = newLocked;
    }

    private void Update()
    {
        // Always pull from current action map
        if (playerInput == null) return;
        InputAction steerAction = playerInput.actions["Steer"];
        if (steerAction == null) return;

        Vector2 steer = steerAction.ReadValue<Vector2>();
        if (locked)
        {
            steer = Vector2.zero;
        }

        movementModel.ProcessInputs(steer.x, steer.y);
    }
}