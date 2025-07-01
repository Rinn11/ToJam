using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Unity.Cinemachine;

public class PlayerJoinManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager;

    [Header("Assigned at runtime")]
    public PlayerInput drunkPlayerInput;
    public PlayerInput copPlayerInput;

    [Header("Cinemachine Controls")]
    public CinemachineInputAxisController drunkCameraController;
    public CinemachineInputAxisController copCameraController;
    public InputActionReference drunkCameraInputAction;
    public InputActionReference copCameraInputAction;

    private int joinedPlayers = 0;

    void Update()
    {
        if (joinedPlayers >= 2) return;

        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad gamepad && !IsDevicePaired(gamepad))
            {
                if (gamepad.startButton.wasPressedThisFrame)
                {
                    var playerInput = playerInputManager.JoinPlayer(joinedPlayers, -1, null, gamepad);
                    AssignPlayer(playerInput);
                    joinedPlayers++;

                    if (joinedPlayers >= 2)
                        playerInputManager.enabled = false;
                }
            }
        }
    }

    private bool IsDevicePaired(InputDevice device)
    {
        return PlayerInput.all.Any(p => p.devices.Contains(device));
    }

    private void AssignPlayer(PlayerInput player)
    {
        if (drunkPlayerInput == null)
        {
            drunkPlayerInput = player;
            drunkPlayerInput.gameObject.name = "DrunkPlayer";
            drunkPlayerInput.SwitchCurrentActionMap("DDPlayer");
            SetCameraInput(drunkCameraController, drunkCameraInputAction);
        }
        else
        {
            copPlayerInput = player;
            copPlayerInput.gameObject.name = "CopPlayer";
            copPlayerInput.SwitchCurrentActionMap("CopPlayer");
            SetCameraInput(copCameraController, copCameraInputAction);
        }
    }

    public void SwapPlayers()
    {
        Debug.Log("Reach");
        if (drunkPlayerInput == null || copPlayerInput == null)
            return;

        Debug.Log("Reached Event");

        var drunkDevice = drunkPlayerInput.devices.FirstOrDefault();
        var copDevice = copPlayerInput.devices.FirstOrDefault();

        if (drunkDevice == null || copDevice == null)
            return;

        Debug.Log("Reached 2ed Event");

        Destroy(drunkPlayerInput.gameObject);
        Destroy(copPlayerInput.gameObject);

        StartCoroutine(RejoinPlayersAfterSwap(drunkDevice, copDevice));
    }

    private IEnumerator RejoinPlayersAfterSwap(InputDevice drunkDevice, InputDevice copDevice)
    {
        yield return null;

        var newCop = playerInputManager.JoinPlayer(0, -1, "CopPlayer", drunkDevice);
        var newDrunk = playerInputManager.JoinPlayer(1, -1, "DDPlayer", copDevice);

        drunkPlayerInput = newDrunk;
        copPlayerInput = newCop;

        SetCameraInput(drunkCameraController, copCameraInputAction); // swapped
        SetCameraInput(copCameraController, drunkCameraInputAction);
    }

    private void SetCameraInput(CinemachineInputAxisController controller, InputActionReference actionRef)
    {
        foreach (var c in controller.Controllers)
        {
            if (c.Name == "Look Orbit X" || c.Name == "Look Orbit Y")
                c.Input.InputAction = actionRef;
        }
    }
}
