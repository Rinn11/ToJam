using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class PlayerJoinManager : MonoBehaviour
{
    public enum ControlSchemeType { Gamepad, KeyboardMouse }

    [System.Serializable]
    public class PlayerSlot
    {
        public GameObject playerObject;
        public Renderer highlightRenderer;
        public TextMeshProUGUI deviceLabel; // UI label that shows device type

        [HideInInspector] public PlayerInput input;
        [HideInInspector] public InputUser user;
        [HideInInspector] public bool occupied = false;
        [HideInInspector] public ControlSchemeType controlScheme;
    }

    [Header("Input Settings")]
    public InputAction joinAction;

    [Header("Player Slots")]
    public List<PlayerSlot> playerSlots = new();

    private bool keyboardMouseTaken = false;

    private void Awake()
    {
        foreach (var slot in playerSlots)
        {
            if (slot.playerObject != null)
            {
                slot.input = slot.playerObject.GetComponent<PlayerInput>();
                if (slot.input != null)
                    slot.input.DeactivateInput();
            }
        }
    }

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

        // Prevent joining with same device
        if (InputUser.FindUserPairedToDevice(device).HasValue)
            return;

        // Prevent duplicate keyboard/mouse join
        if ((device is Keyboard || device is Mouse) && keyboardMouseTaken)
        {
            Debug.Log("Keyboard/Mouse already assigned.");
            return;
        }

        foreach (var slot in playerSlots)
        {
            if (!slot.occupied && slot.input != null)
            {
                Debug.Log($"Player joined on {device.displayName}");

                // Setup user
                var user = InputUser.CreateUserWithoutPairedDevices();
                InputUser.PerformPairingWithDevice(device, user);
                user.AssociateActionsWithUser(slot.input.actions);
                user.ActivateControlScheme("Gamepad"); // or set per device

                // Enable input
                slot.input.ActivateInput();
                slot.user = user;
                slot.occupied = true;

                // Listen for input changes
                slot.input.onControlsChanged += OnControlsChanged;

                // Device type logic
                var scheme = GetControlSchemeType(device);
                slot.controlScheme = scheme;

                if (scheme == ControlSchemeType.KeyboardMouse)
                    keyboardMouseTaken = true;

                // UI Highlight
                if (slot.highlightRenderer != null)
                    slot.highlightRenderer.material.color = Color.green;

                if (slot.deviceLabel != null)
                    slot.deviceLabel.text = scheme.ToString();

                // Leave logic
                var leaveAction = slot.input.actions.FindAction("Leave", true);
                leaveAction.performed += ctx => LeavePlayer(slot);

                return;
            }
        }

        Debug.Log("No free player slots.");
    }

    public void LeavePlayer(PlayerSlot slot)
    {
        if (!slot.occupied) return;

        Debug.Log($"Player left: {slot.user.pairedDevices[0].displayName}");

        slot.user.UnpairDevicesAndRemoveUser();
        slot.input.DeactivateInput();
        slot.user = default;
        slot.occupied = false;

        if (slot.controlScheme == ControlSchemeType.KeyboardMouse)
            keyboardMouseTaken = false;

        if (slot.highlightRenderer != null)
            slot.highlightRenderer.material.color = Color.white;

        if (slot.deviceLabel != null)
            slot.deviceLabel.text = "Press Start to Join";
    }

    private ControlSchemeType GetControlSchemeType(InputDevice device)
    {
        if (device is Gamepad)
            return ControlSchemeType.Gamepad;

        if (device is Keyboard || device is Mouse)
            return ControlSchemeType.KeyboardMouse;

        return ControlSchemeType.Gamepad; // fallback
    }

    private void OnControlsChanged(PlayerInput input)
    {
        var slot = playerSlots.Find(s => s.input == input);
        if (slot == null) return;

        var devices = input.devices;

        foreach (var d in devices)
        {
            var type = GetControlSchemeType(d);
            slot.controlScheme = type;

            if (slot.deviceLabel != null)
                slot.deviceLabel.text = type.ToString();

            Debug.Log($"Player input changed: now using {type}");
            return;
        }
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
            Debug.LogWarning("Both players must be joined to swap control.");
            return;
        }

        Debug.Log("Swapping player controls...");

        var user1 = slot1.user;
        var user2 = slot2.user;

        var input1 = slot1.input;
        var input2 = slot2.input;

        var actions1 = input1.actions;
        var actions2 = input2.actions;

        var devices1 = new List<InputDevice>(user1.pairedDevices);
        var devices2 = new List<InputDevice>(user2.pairedDevices);

        input1.DeactivateInput();
        input2.DeactivateInput();

        user1.UnpairDevices();
        user2.UnpairDevices();

        foreach (var d in devices1)
            InputUser.PerformPairingWithDevice(d, user2);
        foreach (var d in devices2)
            InputUser.PerformPairingWithDevice(d, user1);

        user1.AssociateActionsWithUser(actions2);
        user2.AssociateActionsWithUser(actions1);

        var scheme1 = input1.actions.controlSchemes
            .FirstOrDefault(s => s.name == input2.currentControlScheme);

        var scheme2 = input2.actions.controlSchemes
            .FirstOrDefault(s => s.name == input1.currentControlScheme);

        if (scheme1 == null || scheme2 == null)
        {
            Debug.LogError("Control scheme not found!");
            return;
        }

        // ? Correct usage: string name + InputDevice[]
        input1.SwitchCurrentControlScheme(scheme1.name, devices2.ToArray());
        input2.SwitchCurrentControlScheme(scheme2.name, devices1.ToArray());

        input1.ActivateInput();
        input2.ActivateInput();

        (slot1.user, slot2.user) = (slot2.user, slot1.user);
        (slot1.input, slot2.input) = (slot2.input, slot1.input);

        Debug.Log("Player controls swapped!");
    }

}
