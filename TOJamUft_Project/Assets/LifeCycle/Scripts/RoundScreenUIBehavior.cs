/*
 *  This shows up at the end of the round.
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundScreenUIBehavior : MonoBehaviour
{
    public Text scoreText;
    public Text timeText;

    // Count and update the score.
    public void updateScoreText(float score)
    {
        Debug.Log($"Updating score text with score: {score}");
        scoreText.text = $"Fine: ${score}";
    }

    // Update the timer text
    public void updateTimeText(float elapsedTime)
    { 
        Debug.Log($"Police car caught the drunk driver in {elapsedTime}");

        // Format elapsed time as a string of Minutes:Seconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timeText.text = string.Format("Time: {0:D2}:{1:D2}", minutes, seconds);
    }

    // Show the whole round screen UI
    public void showRoundScreenUI()
    {
        // We need to activate the canvas object under the round screen UI, the object needs to be active for the event system to work.
        Transform child = transform.Find("Canvas");
        child.gameObject.SetActive(true);

    }

    // Hide the whole round screen UI
    public void hideRoundScreenUI()
    {
        // Deactivate the round screen UI
        Transform child = transform.Find("Canvas");
        child.gameObject.SetActive(false);
    }
}
