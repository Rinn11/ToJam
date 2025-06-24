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
    public void updateWinnerUI(string winnerName)
    {
        winnerText.text = $"Winner: {winnerName}";
    }

    // Reload the Scene and reset the round manager to play again
    public void playAgain()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Quit the game
    public void quitGame() {
        Application.Quit();
    }

}
