using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenBehavior : MonoBehaviour
{
    public FineManagerBehavior scoreManager;
    public Text scoreText;

    // Count and update the score.
    public void updateScore() {
        if (scoreManager != null) {
            scoreText.text = $"Fine: ${scoreManager.fine}";
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
        Debug.Log("Enabling End Screen");
        updateScore();
    }
}
