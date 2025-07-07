/*
 *  This shows up at the end of the round.
 */

using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundScreenUIBehavior : MonoBehaviour
{
    [Header("Display 1 UI")]
    public Text scoreText;
    public Text timeText;

    [Header("Display 2 UI")]
    public Text scoreText2;
    public Text timeText2;

    // Count and update the score.
    public void updateScoreText(float score)
    {
        Debug.Log($"Updating score text with score: {score}");
        scoreText.text = $"Fine: ${score}";
        scoreText2.text = scoreText.text;
    }

    // Update the timer text
    public void updateTimeText(float elapsedTime)
    {
        Debug.Log($"Police car caught the drunk driver in {elapsedTime}");

        // Format elapsed time as a string of Minutes:Seconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timeText.text = string.Format("Time: {0:D2}:{1:D2}", minutes, seconds);
        timeText2.text = timeText.text;
    }

    // Show the whole round screen UI
    public void showRoundScreenUI()
    {
        if (Display.displays.Length > 1)
        {
            Transform child1 = transform.Find("Canvas");
            Transform child2 = transform.Find("Canvas (1)");
            child1.gameObject.SetActive(true);
            child2.gameObject.SetActive(true);
        }
        else
        {
            // We need to activate the canvas object under the round screen UI, the object needs to be active for the event system to work.
            Transform child = transform.Find("Canvas");
            child.gameObject.SetActive(true);
        }

    }

    // Hide the whole round screen UI
    public void hideRoundScreenUI()
    {
        // Deactivate the round screen UI
        if (Display.displays.Length > 1)
        {
            Transform child1 = transform.Find("Canvas");
            Transform child2 = transform.Find("Canvas (1)");
            child1.gameObject.SetActive(false);
            child2.gameObject.SetActive(false);
        }
        else
        {
            // We need to activate the canvas object under the round screen UI, the object needs to be active for the event system to work.
            Transform child = transform.Find("Canvas");
            child.gameObject.SetActive(false);
        }
    }
}
