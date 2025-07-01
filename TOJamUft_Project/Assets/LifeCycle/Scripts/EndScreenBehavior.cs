/*
 * Handles the end of the game.
 */

// TODO: Maybe updateScore should not be a concern of the end game? Shouldn't that be in FineManager anyways?

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndScreenBehavior : MonoBehaviour
{
    [Header("Display 1 UI")]
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI p1TimeText;
    public TextMeshProUGUI p2TimeText;

    [Header("Display 2 UI")]
    public TextMeshProUGUI winnerText2;
    public TextMeshProUGUI p1ScoreText2;
    public TextMeshProUGUI p2ScoreText2;
    public TextMeshProUGUI p1TimeText2;
    public TextMeshProUGUI p2TimeText2;

    // Count and update the score.
    public void updateWinnerUI(int winnerID)
    {
        if (winnerID == 0) // Player 1 is the winner
        {
            winnerText.text = "Player 1\nWins!";
        }
        else if (winnerID == 1) // Player 2 is the winner
        {
            winnerText.text = "Player 2\nWins!";
        }
        else // Just say it's a draw
        {
            winnerText.text = "Tied!";
        }
        winnerText2.text = winnerText.text;
    }

    // Update the scores and times for both players
    public void updateScoreUI(float p1Score, float p2Score, float p1Time, float p2Time)
    {
        // Update the scores and times for the first display
        p1ScoreText.text = $"Player 1 Score: {p1Score:F2}";
        p2ScoreText.text = $"Player 2 Score: {p2Score:F2}";

        int p1ElapsedMinutes = Mathf.FloorToInt(p1Time / 60);
        int p1ElapsedSeconds = Mathf.FloorToInt(p1Time % 60);
        int p2ElapsedMinutes = Mathf.FloorToInt(p2Time / 60);
        int p2ElapsedSeconds = Mathf.FloorToInt(p2Time % 60);

        p1TimeText.text = $"Player 1 Time: {p1ElapsedMinutes:D2}:{p1ElapsedSeconds:D2}";
        p2TimeText.text = $"Player 2 Time: {p2ElapsedMinutes:D2}:{p2ElapsedSeconds:D2}";

        // Update the scores and times for the second display
        p1ScoreText2.text = p1ScoreText.text;
        p2ScoreText2.text = p2ScoreText.text;
        p1TimeText2.text = p1TimeText.text;
        p2TimeText2.text = p2TimeText.text;
    }

    // Reload the Scene and reset the round manager to play again
    public void playAgain()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Quit the game
    public void quitGame()
    {
        Application.Quit();
    }

    // Show the end screen UI
    public void showEndScreenUI()
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

    // Hide the end screen UI
    public void hideEndScreenUI()
    {
        // Deactivate the end screen UI
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
