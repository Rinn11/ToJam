using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerUnmappedInputChecker : MonoBehaviour
{
    public GameObject theVecical;

    private PlayerInput playerInput;
    private InputDevice device;
    private HashSet<string> mappedControlPaths;
    private bool coroutineActive;

    void Awake()
    {
        playerInput = theVecical.GetComponent<PlayerInput>();
        device = playerInput.devices.FirstOrDefault(); // Only the paired device

        if (device == null)
        {
            Debug.LogWarning($"No device paired with {gameObject.name}");
        }

        mappedControlPaths = new HashSet<string>();

        // Collect all used control paths in the player's current action map
        foreach (var action in playerInput.currentActionMap.actions)
        {
            foreach (var binding in action.bindings)
            {
                if (!string.IsNullOrEmpty(binding.effectivePath))
                    mappedControlPaths.Add(binding.effectivePath);
            }
        }
    }

    void Update()
    {
        if (device == null) return;

        foreach (var control in device.allControls)
        {
            if (control is ButtonControl button && button.wasPressedThisFrame)
            {
                if (!mappedControlPaths.Contains(control.path) && !coroutineActive)
                {
                    Debug.Log($"{gameObject.name} pressed UNMAPPED button: {control.displayName} ({control.path})");
                    StartCoroutine(ShowControls());

                }
            }
        }
    }

    IEnumerator ShowControls()
    {
        coroutineActive = true;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3);

        coroutineActive = false;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
