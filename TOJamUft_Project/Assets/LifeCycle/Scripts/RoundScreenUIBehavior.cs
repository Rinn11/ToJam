/*
 *  This shows up at the end of the round.
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundScreenUIBehavior : MonoBehaviour
{
    public Text scoreText;

    // Count and update the score.
    public void updateScoreText(float score)
    {
        scoreText.text = $"Fine: {score}";
    }

    // Show the whole round screen UI
    public void showRoundScreenUI()
    {
        // Activate the round screen UI
        gameObject.SetActive(true);
    }

    // Hide the whole round screen UI
    public void hideRoundScreenUI()
    {
        // Deactivate the round screen UI
        gameObject.SetActive(false);
    }
}
