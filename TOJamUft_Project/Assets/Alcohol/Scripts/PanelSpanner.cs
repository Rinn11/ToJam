using UnityEngine;
using UnityEngine.UI;

public class PanelSetup : MonoBehaviour
{
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Image image = GetComponent<Image>();

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