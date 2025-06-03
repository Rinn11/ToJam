using UnityEngine;
using System;

// Credit to Ethan for writing this file.
// There was a merge conflict, so I had to manually port it over.

// This file defines a C# event representing the drunk driver broadcasting its location to the cop

public class AlertDDOfCopLocationEventSender : MonoBehaviour
{
    // sends location of DD to the cop. Position is sent as x and z coordinates.
    public event Action<Vector2> OnLocationEvent;
    public GameObject CopPlayer;

    public bool DebugMode = true;

    public void Trigger(Vector2 locationToSend)
    {
        // Invoke the event
        OnLocationEvent?.Invoke(locationToSend);
    }

    public void Update()
    {
        if (DebugMode)
        {
            // For debugging purposes, you can trigger the event with a key press of key 4
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                // Get the position of the DD player
                Vector2 copPosition = new Vector2(CopPlayer.transform.position.x, CopPlayer.transform.position.z);
                Trigger(copPosition);
            }
        }
    }
}