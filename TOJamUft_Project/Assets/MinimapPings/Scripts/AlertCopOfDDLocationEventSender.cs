using UnityEngine;
using System;

// Credit to Ethan for writing this file.
// There was a merge conflict, so I had to manually port it over.

// This file defines a C# event representing the cop broadcasting its location to the drunk driver

public class AlertCopOfDDLocationEventSender : MonoBehaviour
{
    // sends location of DD to the cop. Position is sent as x and z coordinates.
    public event Action<Vector2> OnLocationEvent;
    public GameObject DDplayer;

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
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                // Get the position of the DD player
                Vector2 ddPosition = new Vector2(DDplayer.transform.position.x, DDplayer.transform.position.z);
                Trigger(ddPosition);
            }
        }
    }
}