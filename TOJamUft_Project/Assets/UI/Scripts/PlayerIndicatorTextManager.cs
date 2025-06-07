using TMPro;
using UnityEngine;

public class PlayerIndicatorTextManager : MonoBehaviour
{
    public PlayerSwapEventSender swapSender;

    public TextMeshProUGUI player1indicatorText;
    public TextMeshProUGUI player2indicatorText;

    void Start()
    {
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
        if (Display.displays.Length == 1)
        {
            // Swap text
            string tmpText = player1indicatorText.text;
            player1indicatorText.text = player2indicatorText.text;
            player2indicatorText.text = tmpText;

            // Swap colors
            Color tmpColor = player1indicatorText.color;
            player1indicatorText.color = player2indicatorText.color;
            player2indicatorText.color = tmpColor;
        }
    }
}