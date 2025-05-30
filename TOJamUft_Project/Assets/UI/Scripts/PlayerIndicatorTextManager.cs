using TMPro;
using UnityEngine;

public class PlayerIndicatorTextManager : MonoBehaviour
{
    public PlayerSwapEventSender swapSender;

    public TextMeshProUGUI player1indicatorText;
    public TextMeshProUGUI player2indicatorText;

    void Start()
    {
        // Initialize the text for both players
        player1indicatorText.text = "Player 1 - Drunk Driver";
        player2indicatorText.text = "Player 2 - Cop";
    }

    private void OnEnable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent += recievePlayerSwap;
    }

    private void OnDisable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent -= recievePlayerSwap;
    }

    public void recievePlayerSwap(bool isPlayer1DrunkDriver)
    {
        if (isPlayer1DrunkDriver)
        {
            player1indicatorText.text = "Player 1 - Drunk Driver";
            player2indicatorText.text = "Player 2 - Cop";
        }
        else
        {
            player1indicatorText.text = "Player 1 - Cop";
            player2indicatorText.text = "Player 2 - Drunk Driver";
        }
    }
}