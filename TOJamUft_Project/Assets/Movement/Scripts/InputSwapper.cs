/*
 * This file deals with swapping the PlayerInput components attached to each player.
 * It attaches itself to the PlayerSwapEventSender
 */

/*
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSwapper : MonoBehaviour
{
    public PlayerInput drunkPlayerInput;
    public PlayerInput copPlayerInput;
    public CinemachineInputAxisController drunkCinemachineInputAxisController;
    public CinemachineInputAxisController copCinemachineInputAxisController;
    public InputActionReference drunkCameraInputActionReference;
    public InputActionReference copCameraInputActionReference;

    public PlayerSwapEventSender swapSender;

    private bool isPlayer1Driving = true;

    private void OnEnable()
    {
        if (swapSender != null)
        {
            swapSender.OnBoolEvent += HandleSwapEvent;
        }
    }

    private void OnDisable()
    {
        if (swapSender != null)
        {
            swapSender.OnBoolEvent -= HandleSwapEvent;
        }
    }

    private void HandleSwapEvent(bool player1Driving)
    {
        isPlayer1Driving = player1Driving;
        SwapInputs();
    }

    public void SwapInputs()
    {
        if (isPlayer1Driving)
        {
            drunkPlayerInput.SwitchCurrentActionMap("DDPlayer");
            copPlayerInput.SwitchCurrentActionMap("CopPlayer");

            // Yes for some reason, this is the only way to set the input actions for the CinemachineInputAxisController...
            foreach (var c in drunkCinemachineInputAxisController.Controllers)
            {
                if (c.Name == "Look Orbit X")
                    c.Input.InputAction = drunkCameraInputActionReference;

                if (c.Name == "Look Orbit Y")
                    c.Input.InputAction = drunkCameraInputActionReference;
            }
            foreach (var c in copCinemachineInputAxisController.Controllers)
            {
                if (c.Name == "Look Orbit X")
                    c.Input.InputAction = copCameraInputActionReference;

                if (c.Name == "Look Orbit Y")
                    c.Input.InputAction = copCameraInputActionReference;
            }
        }
        else
        {
            drunkPlayerInput.SwitchCurrentActionMap("CopPlayer");
            copPlayerInput.SwitchCurrentActionMap("DDPlayer");
            
            foreach (var c in drunkCinemachineInputAxisController.Controllers)
            {
                if (c.Name == "Look Orbit X")
                    c.Input.InputAction = copCameraInputActionReference;

                if (c.Name == "Look Orbit Y")
                    c.Input.InputAction = copCameraInputActionReference;
            }
            foreach (var c in copCinemachineInputAxisController.Controllers)
            {
                if (c.Name == "Look Orbit X")
                    c.Input.InputAction = drunkCameraInputActionReference;

                if (c.Name == "Look Orbit Y")
                    c.Input.InputAction = drunkCameraInputActionReference;
            }
        }
    }
}*/