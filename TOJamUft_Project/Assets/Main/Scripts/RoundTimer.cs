using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class TimeEvent : UnityEvent<float> { };

public class RoundTimer : MonoBehaviour
{
    // This is class that modifies UI text elements to display a timer.
    public List<TMP_Text> timerTextElements; // List of UI text elements to display the timer for this script to modify
    public List<UnityEngine.UI.Text> timeElapsedTextElements; // List of UI text elements to display the time elapsed for this script to modify
    public float roundDuration; // Max duration of the round in seconds
    public TimeEvent sendTimeEvent; // Event to send the elapsed time to other components
    public UnityEvent roundEndEvent; // Event to invoke when the round ends.

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
        timeRemaining = Mathf.Max(timeRemaining - Time.deltaTime, 0f);

        //Debug.Log($"Elapsed Time: {elapsedTime:F2}, Time Remaining: {timeRemaining:F2}");

        if (timeRemaining <= 0f)
        {
            // Invoke an event to tell the round manager that the round has ended.
            roundEndEvent.Invoke();
            ResetTimer();
            Debug.Log("Round ended due to time running out.");
        }

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

        foreach (var timeElapsedText in timeElapsedTextElements)
        {
            if (timeElapsedText != null)
            {
                // Update the text to show elapsed time.
                timeElapsedText.text = elapsedTimeFormatted;
            }
        }
    }

    // Method to reset the timer to the initial round duration
    public void ResetTimer()
    {
        timeRemaining = roundDuration;
        elapsedTime = 0f;

        // Initialize all timer text elements to show the initial time remaining, and all time elapsed text elements to show 00:00.
        foreach (var timerText in timerTextElements)
        {
            if (timerText != null)
            {
                // Set the initial text to show time remaining for now.
                timerText.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(roundDuration / 60), Mathf.FloorToInt(roundDuration % 60));
            }
        }

        foreach (var timeElapsedText in timeElapsedTextElements)
        {
            if (timeElapsedText != null)
            {
                // Set the initial text to show elapsed time as 00:00.
                timeElapsedText.text = "00:00";
            }
        }
    }

    // Method to send the elapsed time to other components as a event invokation.
    public void sendTimeInvoker()
    {
        // This method can be used to send the elapsed time to other components if needed.
        // For now, we will just log it.
        Debug.Log($"Elapsed Time: {elapsedTime:F2} seconds");
        sendTimeEvent.Invoke(elapsedTime);
    }
}
