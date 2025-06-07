/*
 * Manages the current fine of the player.
 * Increases the fine based on a rate from the alcohol multiplier and updates the relevant UI element.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class ScoreEvent : UnityEvent<float> { };

public class FineManagerBehavior : MonoBehaviour
{
    public float fine;
    public AlcoholManager am;
    private AlcoholManager alcoholManager;
    public Text fineUI;

    // Events
    public ScoreEvent sendScoreEvent;

    void Start()
    {
        fine = 0.0f;
        alcoholManager = am.GetComponent<AlcoholManager>();
    }

    public void increaseFine(int amount)
    {
        fine += amount;
        fine = (float)Math.Round(fine, 2);

        if (fineUI != null)
        {
            fineUI.text = $"Fine: ${fine}";
        }
    }


    public void sendScoreInvoker()
    {
        // Fine manager will not persist across rounds, so we send the score to the RoundManager so that the data can be stored.
        Debug.Log("Sending score to round manager");
        sendScoreEvent.Invoke(fine);
    }
    
    public void ResetFines()  // invoked by round manager's reset scene event
    {
        fine = 0.0f;
        if (fineUI != null)
        {
            fineUI.text = $"Fine: ${fine}";
        }
    }
}
