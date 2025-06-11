/*
 * This file deals with swapping the PlayerInput components attached to each player.
 * It attaches itself to the PlayerSwapEventSender
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class InputSwapper : MonoBehaviour
{
    public PlayerInput drunkPlayerInput;
    public PlayerInput copPlayerInput;

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
        }
        else
        {
            drunkPlayerInput.SwitchCurrentActionMap("CopPlayer");
            copPlayerInput.SwitchCurrentActionMap("DDPlayer");
        }
    }
}