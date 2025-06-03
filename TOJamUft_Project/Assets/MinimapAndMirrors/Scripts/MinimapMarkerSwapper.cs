using UnityEngine;
using UnityEngine.UI;

public class MinimapMarkerSwapper : MonoBehaviour
{
    public GameObject Player1MinimapMarkerImage;
    public GameObject Player2MinimapMarkerImage;

    public PlayerSwapEventSender swapSender;

    private RawImage _p1Image;
    private RawImage _p2Image;

    public void Start()
    {
        if (Player1MinimapMarkerImage != null)
            _p1Image = Player1MinimapMarkerImage.GetComponent<RawImage>();
        if (Player2MinimapMarkerImage != null)
            _p2Image = Player2MinimapMarkerImage.GetComponent<RawImage>();
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
            Color temp = _p1Image.color;
            _p1Image.color = _p2Image.color;
            _p2Image.color = temp;
        }
    }
}
