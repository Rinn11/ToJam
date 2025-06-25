/*
 * Handles the end of the game.
 */

// TODO: Maybe updateScore should not be a concern of the end game? Shouldn't that be in FineManager anyways?

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenBehavior : MonoBehaviour
{
    public Text winnerText;

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
        Transform child = transform.Find("Canvas");
        child.gameObject.SetActive(true);
    }

    // Hide the end screen UI
    public void hideEndScreenUI()
    {
        Transform child = transform.Find("Canvas");
        child.gameObject.SetActive(false);
    }

}
