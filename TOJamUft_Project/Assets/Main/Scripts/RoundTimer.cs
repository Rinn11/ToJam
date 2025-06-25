using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundTimer : MonoBehaviour
{
    // This is class that modifies UI text elements to display a timer.
    public List<UnityEngine.UI.Text> timerTextElements; // List of UI text elements to display the timer for this script to modify
    public float roundDuration; // Max duration of the round in seconds

    // Keep both time remaining and elapsed time to calculate the timer, in case we want to utilize both in the future
    private float timeRemaining; // Time remaining in the current round
    private float elapsedTime; // Time elapsed since the round started

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        // Update both private variables with the delta time provided by each frame
        elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, roundDuration);
        timeRemaining = Mathf.Max(roundDuration - Time.deltaTime, 0f);

        // Seperate into minutes and seconds for display purposes.
        int elapsedMinutes = Mathf.FloorToInt(elapsedTime / 60);
        int elapsedSeconds = Mathf.FloorToInt(elapsedTime % 60);

        int remainingMinutes = Mathf.FloorToInt(timeRemaining / 60);
        int remainingSeconds = Mathf.FloorToInt(timeRemaining % 60);

        // Format elapsed and remaining time as a string of Minutes:Seconds
        string elapsedTimeFormatted = string.Format("{0:D2}:{1:D2}", elapsedMinutes, elapsedSeconds);
        string remainingTimeFormatted = string.Format("{0:D2}:{1:D2}", remainingMinutes, remainingSeconds);

        // Iterate through all the timer text elements and update their text.
        foreach (var timerText in timerTextElements)
        {
            if (timerText != null)
            {
                // Update the text to show time remaining for now.
                timerText.text = remainingTimeFormatted;
            }
        }
    }

    // Method to reset the timer to the initial round duration
    public void ResetTimer()
    { 
        timeRemaining = roundDuration;
        elapsedTime = 0f;

        // Initialize all timer text elements to show the initial time remaining
        foreach (var timerText in timerTextElements)
        {
            if (timerText != null)
            {
                // Set the initial text to show time remaining for now.
                timerText.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(roundDuration / 60), Mathf.FloorToInt(roundDuration % 60));
            }
        }
    }
}
