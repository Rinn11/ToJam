using UnityEngine;
using UnityEngine.UI;

public class PanelSetup : MonoBehaviour
{

    private RectTransform rectTransform;
    private Image image;
    public PlayerSwapEventSender swapSender;

    void Start()
    {
        
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (Display.displays.Length == 1)
        {
            if (rectTransform != null)
            {
                // Set anchors to cover the top half of the screen
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }

            if (image != null)
            {
                // Ensure the panel has a solid background
                image.sprite = null; // Remove any sprite
                image.color = Color.black; // Set a solid color
            }
            else
            {
                Debug.LogError("Image component not found!");
            }
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
        if (Display.displays.Length == 1)
        { 
            if (isPlayer1DrunkDriver)
            {
                // top half of the screen
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 1);
            }
            else
            {
                // bottom half of the screen
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
            }

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}