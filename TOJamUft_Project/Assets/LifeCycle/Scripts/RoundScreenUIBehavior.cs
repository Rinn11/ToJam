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
        Debug.Log($"Updating score text with score: {score}");
        scoreText.text = $"Fine: ${score}";
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
