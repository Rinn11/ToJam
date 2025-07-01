/*
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

    public Renderer highlightRenderer;
    public TMPro.TMP_Text deviceLabel;
}

public class PlayerJoinManager : MonoBehaviour
{
    public List<PlayerSlot> playerSlots = new();
    public InputAction joinAction;

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

    private void OnJoinPressed(InputAction.CallbackContext ctx)
    {
        var device = ctx.control.device;
        Debug.Log($"Join pressed by: {device.displayName}");

        if (InputUser.FindUserPairedToDevice(device).HasValue)
        {
            Debug.Log("Device already in use.");
            return;
        }

        if ((device is Keyboard || device is Mouse) && keyboardMouseTaken)
        {
            Debug.Log("Keyboard & mouse already taken.");
            return;
        }

        foreach (var slot in playerSlots)
        {
            if (!slot.occupied && slot.input != null)
            {
                var scheme = slot.input.actions.controlSchemes
                    .FirstOrDefault(s => s.SupportsDevice(device));

                if (scheme.name == null)
                {
                    Debug.LogError($"No scheme supports {device.displayName}");
                    return;
                }

                slot.input.SwitchCurrentControlScheme(scheme.name, new[] { device });
                slot.input.ActivateInput();

                slot.user = slot.input.user;
                slot.occupied = true;

                if (device is Keyboard || device is Mouse)
                    keyboardMouseTaken = true;

                if (slot.highlightRenderer != null)
                    slot.highlightRenderer.material.color = Color.green;
                if (slot.deviceLabel != null)
                    slot.deviceLabel.text = scheme.name;

                var leaveAction = slot.input.actions.FindAction("Leave", true);
                leaveAction.performed += ctx => LeavePlayer(slot);

                LogAllPairings();
                return;
            }
        }

        Debug.Log("No available slots.");
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

        if (slot.highlightRenderer != null)
            slot.highlightRenderer.material.color = Color.white;
        if (slot.deviceLabel != null)
            slot.deviceLabel.text = "Press Start to Join";

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

        var devices1 = user1.pairedDevices.ToArray();
        var devices2 = user2.pairedDevices.ToArray();

        input1.DeactivateInput();
        input2.DeactivateInput();

        user1.UnpairDevices();
        user2.UnpairDevices();

        // Rebind user1 to control input2
        user1.AssociateActionsWithUser(input2.actions);
        foreach (var device in devices1)
            InputUser.PerformPairingWithDevice(device, user1);
        input2.SwitchCurrentControlScheme(input2.currentControlScheme, devices1);
        input2.ActivateInput();
        slot2.user = user1;

        // Rebind user2 to control input1
        user2.AssociateActionsWithUser(input1.actions);
        foreach (var device in devices2)
            InputUser.PerformPairingWithDevice(device, user2);
        input1.SwitchCurrentControlScheme(input1.currentControlScheme, devices2);
        input1.ActivateInput();
        slot1.user = user2;

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
}*/
