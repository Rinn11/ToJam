using UnityEngine;

public class SplitScreenUI : MonoBehaviour
{
    public RectTransform textGroup;
    public bool useBottomHalf = false;

    public PlayerSwapEventSender swapSender;

    void Start()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        float screenHeight = ((RectTransform)textGroup.parent).rect.height;
        // get width and height of textgroup
        float width = textGroup.rect.width;
        float height = textGroup.rect.height;
        if (useBottomHalf)
        {
            // Move the anchored group down by half the screen
            textGroup.anchoredPosition = new Vector2(-(width / 2f), -(screenHeight) + (height / 2f));
        }
        else
        {
            // Top half position (default)
            textGroup.anchoredPosition = new Vector2(-(width / 2f), -(height / 2f));
        }
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
        // Update the position based on whether Player 1 is driving
        useBottomHalf = !isPlayer1DrunkDriver;
        UpdatePosition();
        Debug.Log($"SplitScreenUI updated: useBottomHalf = {useBottomHalf}");
    }
}