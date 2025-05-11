using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenBehavior : MonoBehaviour
{
    public GameObject player;
    public Text scoreText;

    // Count and update the score.
    public void updateScore() {
        if (player != null) {
            scoreText.text = $"Fine: ${0}";
        }
    }
    
    // Reload the Scene and play again
    public void playAgain() {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Quit the game
    public void quitGame() {
        Application.Quit();
    }

    // On Enabling the UI we will update the score.
    private void OnEnable() {
        updateScore();
    }
}
