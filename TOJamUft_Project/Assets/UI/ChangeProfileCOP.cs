using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PortraitSwitcherCOP : MonoBehaviour
{
    [Header("Portrait sprites")]
    public Sprite defaultSprite;
    public Sprite angrySprite;
    public Sprite happySprite;

    public GameObject CopPlayer;  // should have lassoFire.cs
    private lassoFire lasso; // reference to the lassoFire component

    public GameObject Bottle;  // to get isShaking, which implies DD visited bar which makes Cop angry
    private shakeBottle shakeBottle; 
    
    private Image _img;                 // cached reference
    private PortraitState _current;     // tracks what we last showed

    private enum PortraitState { Default, Angry, Happy }

    void Awake()
    {
        _img = GetComponent<Image>();
        _img.sprite = defaultSprite;    // initial value
        _current = PortraitState.Default;
        
        // get component for lassoFire's GetIsPulling()
        if (CopPlayer != null)
        {
            lasso = CopPlayer.GetComponent<lassoFire>();
        }

        if (Bottle != null)
        {
            shakeBottle = Bottle.GetComponent<shakeBottle>();
        }
    }

    void Update()
    {
        // priority ladder: happy > angry > default
        // happy is when DD is lassoed
        // angry is for a second when DD visits a bar i.e pinged
        // default is when DD is not lassoed or pinged
        PortraitState desired =
            lasso.GetIsPulling() ? PortraitState.Happy :
            shakeBottle.GetIsShaking() ? PortraitState.Angry :
            PortraitState.Default;

        // Avoid redundant sprite assignments every frame
        if (desired == _current) return;

        switch (desired)
        {
            case PortraitState.Happy: _img.sprite = happySprite; break;
            case PortraitState.Angry: _img.sprite = angrySprite; break;
            default:                  _img.sprite = defaultSprite; break;
        }

        _current = desired;
    }
}