/*
 * Manages the current fine of the player.
 * Increases the fine based on a rate from the alcohol multiplier and updates the relevant UI element.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class ScoreEvent : UnityEvent<float> { };

public class FineManagerBehavior : MonoBehaviour
{
    [HideInInspector]
    public float fine; // Current fine amount
    public TMP_Text  fineUI;

    [Header("Fine Settings")] // Settings for each fine type not just alcohol fines. it could be property damage, speeding, etc.
    public float minFine; //100
    public float maxFine; //500
    public float collisionFineAmount = 10.0f;  //10
    public float alcoholDecayAlpha; // The alpha decay rate for the alcohol fine, this will be used to reduce the fine over time. 

    // Events
    public ScoreEvent sendScoreEvent;

    void Start()
    {
        fine = 0.0f;
    }

    public void increaseFine(int amount)
    {
        fine += amount;
        fine = (float) Math.Round(fine, 2);

        if (fineUI != null)
        {
            fineUI.text = $"${fine}";
        }
    }


    public void sendScoreInvoker()
    {
        // Fine manager will not persist across rounds, so we send the score to the RoundManager so that the data can be stored.
        sendScoreEvent.Invoke(fine);
    }
    
    public void ResetFines()  // invoked by round manager's reset scene event
    {
        fine = 0.0f;
        if (fineUI != null)
        {
            fineUI.text = $"${fine}";
        }
    }
}
