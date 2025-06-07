using UnityEngine;

public class MinimapSwapper : MonoBehaviour
{
    public GameObject Player1MinimapCanvas;
    public GameObject Player2MinimapCanvas;

    public PlayerSwapEventSender swapSender;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent += recievePlayerSwapMinimap;
    }

    private void OnDisable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent -= recievePlayerSwapMinimap;
    }

    public void recievePlayerSwapMinimap(bool isPlayer1DrunkDriver)
    {
        if (Display.displays.Length == 1)
        {
            var rt1 = Player1MinimapCanvas.GetComponent<RectTransform>();
            var rt2 = Player2MinimapCanvas.GetComponent<RectTransform>();

            // Swap anchors
            Vector2 aMin1 = rt1.anchorMin;
            Vector2 aMax1 = rt1.anchorMax;
            Vector2 pivot1 = rt1.pivot;

            Vector2 aMin2 = rt2.anchorMin;
            Vector2 aMax2 = rt2.anchorMax;
            Vector2 pivot2 = rt2.pivot;

            rt1.anchorMin = aMin2;
            rt1.anchorMax = aMax2;
            rt1.pivot = pivot2;

            rt2.anchorMin = aMin1;
            rt2.anchorMax = aMax1;
            rt2.pivot = pivot1;
            Debug.Log("swapminimap");
        }
    }
}
