/*
 * Handles the end of the game.
 */

// TODO: Maybe updateScore should not be a concern of the end game? Shouldn't that be in FineManager anyways?

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenBehavior : MonoBehaviour
{
    [Header("Display 1 UI")]
    public Text winnerText;

    [Header("Display 2 UI")]
    public Text winnerText2;

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
