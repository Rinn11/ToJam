using UnityEngine;
using UnityEngine.UI;

public class MinimapMarkerSwapper : MonoBehaviour
{
    public GameObject Player1MinimapMarkerImage;
    public GameObject Player2MinimapMarkerImage;

    public PlayerSwapEventSender swapSender;

    private Image _p1Image;
    private Image _p2Image;

    public void Awake()
    {
        if (Player1MinimapMarkerImage != null)
            _p1Image = Player1MinimapMarkerImage.GetComponent<Image>();
        if (Player2MinimapMarkerImage != null)
            _p2Image = Player2MinimapMarkerImage.GetComponent<Image>();
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
        Color temp = _p1Image.color;
        _p1Image.color = _p2Image.color;
        _p2Image.color = temp;
    }
}
