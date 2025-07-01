using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[System.Serializable]
public class PlayerSlot
{
    public PlayerInput input;
    public InputUser user;
    public bool occupied;

    // visuals used for debuging
    //public Renderer highlightRenderer;
}

public class PlayerJoinManager : MonoBehaviour
{
    public List<PlayerSlot> playerSlots = new();
    public InputAction joinAction;
    private Dictionary<InputUser, InputDevice> assignedDevices = new();

    private bool keyboardMouseTaken = false;

    private void OnEnable()
    {
        joinAction.Enable();
        joinAction.performed += OnJoinPressed;
    }

    private void OnDisable()
    {
        joinAction.performed -= OnJoinPressed;
        joinAction.Disable();
    }
    private void Start()
    {
        // Clear any stale pairings
        foreach (var user in InputUser.all.ToArray())
        {
            user.UnpairDevices();
        }

        // Clear KB/M flag
        keyboardMouseTaken = false;
    }


    private void OnJoinPressed(InputAction.CallbackContext ctx)
    {
        var device = ctx.control.device;
        Debug.Log($"Join pressed by: {device.displayName}");

        // Prevent joining if device already paired
        if (InputUser.FindUserPairedToDevice(device).HasValue)
        {
            Debug.Log("Device is already in use.");
            return;
        }

        // Only allow one keyboard/mouse player
        if ((device is Keyboard || device is Mouse) && keyboardMouseTaken)
        {
            Debug.Log("Keyboard & mouse already taken.");
            return;
        }

        // Finds first available slot
        foreach (var slot in playerSlots)
        {
            if (!slot.occupied && slot.input != null)
            {
                // Match a control scheme to the device
                var scheme = slot.input.actions.controlSchemes
                    .FirstOrDefault(s => s.SupportsDevice(device));

                if (scheme.name == null)
                {
                    Debug.LogError($"No control scheme supports device: {device.displayName}");
                    return;
                }

                // Assign the device & scheme to the PlayerInput
                slot.input.SwitchCurrentControlScheme(scheme.name, new[] { device });
                slot.input.ActivateInput();

                // Set user reference and slot state
                slot.user = slot.input.user;
                slot.occupied = true;

                // Tracks which device this user is supposed to have
                assignedDevices[slot.user] = device;

                if (device is Keyboard || device is Mouse)
                    keyboardMouseTaken = true;

                // Makes sure unity does add a controler to the wrong object
                EnforceSingleDevicePerUser(slot.user);

                // Visual feedback, used for debugging
                /*
                if (slot.highlightRenderer != null)
                    slot.highlightRenderer.material.color = Color.green;
                */

                // allows for deactivation of controler, currently inactive 
                //var leaveAction = slot.input.actions.FindAction("Leave", true);
                // leaveAction.performed += ctx => LeavePlayer(slot);

                LogAllPairings();

                return;
            }
        }

        Debug.Log("No open slots available.");
    }



    public void LeavePlayer(PlayerSlot slot)
    {
        if (!slot.occupied) return;

        slot.input.DeactivateInput();
        slot.user.UnpairDevicesAndRemoveUser();
        slot.user = default;
        slot.occupied = false;

        if (slot.input.user.pairedDevices.Any(d => d is Keyboard || d is Mouse))
            keyboardMouseTaken = false;

        LogAllPairings();
    }

    public void SwapPlayers()
    {
        if (playerSlots.Count < 2)
        {
            Debug.LogWarning("Need 2 players to swap.");
            return;
        }

        var slot1 = playerSlots[0];
        var slot2 = playerSlots[1];

        if (!slot1.occupied || !slot2.occupied)
        {
            Debug.LogWarning("Both players must be joined to swap.");
            return;
        }

        Debug.Log("Swapping player control...");

        var user1 = slot1.user;
        var user2 = slot2.user;

        var input1 = slot1.input;
        var input2 = slot2.input;

        var device1 = user1.pairedDevices.FirstOrDefault();
        var device2 = user2.pairedDevices.FirstOrDefault();

        if (device1 == null || device2 == null)
        {
            Debug.LogError("One or both users have no assigned device.");
            return;
        }

        // Store devices to swap
        assignedDevices[user1] = device2;
        assignedDevices[user2] = device1;

        // Deactivate inputs
        input1.DeactivateInput();
        input2.DeactivateInput();

        // Unpair
        user1.UnpairDevices();
        user2.UnpairDevices();

        // Pair swapped devices
        InputUser.PerformPairingWithDevice(device2, user1);
        InputUser.PerformPairingWithDevice(device1, user2);

        // Associate actions
        user1.AssociateActionsWithUser(input2.actions);
        user2.AssociateActionsWithUser(input1.actions);

        // Switch schemes
        var scheme1 = input2.actions.controlSchemes.FirstOrDefault(s => s.SupportsDevice(device2));
        var scheme2 = input1.actions.controlSchemes.FirstOrDefault(s => s.SupportsDevice(device1));

        if (scheme1.name == null || scheme2.name == null)
        {
            Debug.LogError("Missing control scheme for one of the devices.");
            return;
        }

        input1.SwitchCurrentControlScheme(scheme2.name, new[] { device1 });
        input2.SwitchCurrentControlScheme(scheme1.name, new[] { device2 });

        // Reactivate
        input1.ActivateInput();
        input2.ActivateInput();

        // Swap users in slots
        (slot1.user, slot2.user) = (slot2.user, slot1.user);

        // Makes sure unity does add a controler to the wrong object
        EnforceSingleDevicePerUser(slot1.user);
        EnforceSingleDevicePerUser(slot2.user);

        Debug.Log("Players successfully swapped control.");
        LogAllPairings();
    }

    public void LogAllPairings()
    {
        Debug.Log("=== InputUser Pairings ===");

        foreach (var user in InputUser.all)
        {
            string devices = string.Join(", ", user.pairedDevices.Select(d => d.displayName));
            Debug.Log($"User {user.id} paired with: {devices}");
        }

        Debug.Log("=== Player Slots ===");

        for (int i = 0; i < playerSlots.Count; i++)
        {
            var slot = playerSlots[i];
            if (slot.occupied)
            {
                var devices = string.Join(", ", slot.user.pairedDevices.Select(d => d.displayName));
                Debug.Log($"Slot {i} ? User {slot.user.id} using: {devices}");
            }
            else
            {
                Debug.Log($"Slot {i} ? Open");
            }
        }
    }

    // this is made to make sure that two devices cannot be paired to the same object
    private void EnforceSingleDevicePerUser(InputUser user)
    {
        var devices = user.pairedDevices.ToList();
        if (devices.Count > 1)
        {
            var intendedDevice = assignedDevices.ContainsKey(user) ? assignedDevices[user] : devices[0];
            foreach (var d in devices)
            {
                if (d != intendedDevice)
                {
                    Debug.Log($"Unpairing extra device: {d.displayName} from user {user.id}");
                    user.UnpairDevice(d);
                }
            }
        }
    }
}
