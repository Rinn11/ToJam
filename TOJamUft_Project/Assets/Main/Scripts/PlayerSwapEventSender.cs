using UnityEngine;
using System;

public class PlayerSwapEventSender : MonoBehaviour
{
    // This event will be used to swap the player controls between Player 1 and Player 2.
    // It will pass a boolean indicating whether Player 1 is currently driving.'
    public event Action<bool> OnBoolEvent;

    public GameObject roundManager;
    
    public void Trigger(bool valueToSend)
    {
        // Invoke the event
        OnBoolEvent?.Invoke(valueToSend);
    }
    
    
    public void Update() 
    {
        // For debugging purposes, you can trigger the event with a key press of key 5
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {   
            bool isPlayer1Driving = roundManager.GetComponent<RoundManager>().toggleIsP1Driving();
            
            Trigger(isPlayer1Driving);
            Debug.Log("Player swap event triggered.");
        }
    }
};