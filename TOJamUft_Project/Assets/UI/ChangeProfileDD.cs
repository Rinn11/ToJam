using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PortraitSwitcherDD : MonoBehaviour
{
    [Header("Portrait sprites")]
    public Sprite defaultSprite;
    public Sprite drinkingSprite;
    public Sprite dizzySprite;
    
    public AlcoholManager alcoholManager; // Reference to the AlcoholManager to check drinking state
    private bool isLassoed = false;

    private Image _img;                 // cached reference
    private PortraitState _current;     // tracks what we last showed

    private enum PortraitState { Default, Drinking, Dizzy }

    void Awake()
    {
        _img = GetComponent<Image>();
        _img.sprite = defaultSprite;    // initial value
        _current = PortraitState.Default;
    }

    void Update()
    {
        // A simple priority ladder: drinking > lassoed > default
        PortraitState desired =
            (alcoholManager != null && alcoholManager.GetIsDrinking()) ? PortraitState.Drinking :
            isLassoed  ? PortraitState.Dizzy     :
            PortraitState.Default;

        // Avoid redundant sprite assignments every frame
        if (desired == _current) return;

        switch (desired)
        {
            case PortraitState.Drinking: _img.sprite = drinkingSprite; break;
            case PortraitState.Dizzy:    _img.sprite = dizzySprite;    break;
            default:                     _img.sprite = defaultSprite;  break;
        }

        _current = desired;
    }
}