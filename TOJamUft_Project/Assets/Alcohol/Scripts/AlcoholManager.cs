/*
 * Manages the effects of alcohol:
 * - Applies a blackout effect
 * - Applies a blurring effect (via shader texture)
 * - Updates an alcohol counter component
 * Also animates the bottle being drank.
 */

/*
 * Implements IMovementModifier, which specifies the statistics of the car.
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class AlcoholManager : MonoBehaviour, IMovementModifier
{
  public GameObject blackoutPanel;          // The panel used to simulate a blackout

  public RawImage capacityRectangle; // this is the rectangle that fills up with alcohol supply
  public int capacityRectangleMaxHeight = 146; // the maximum height of the rectangle in pixels
  public RawImage bottle;
  public GameObject DrinkToolTip;
  
  //UI text
  public TMP_Text alcoholCountUI;

  public int initialAlcoholCount;
  public int initialAlcoholSupply;

  [SerializeField] private PlayerInput playerInput;

  [SerializeField]
  private RoundManager roundManager;

  private float alcoholCount;                 // The number of alcohol bottles 
  private float alcoholSupply;              // The number of alcohol bottles available
  
  private bool withdrawalSymptom = false;  // If true, the player is experiencing withdrawal symptoms and can't drink alcohol
  private float withdrawalTimer = 0.0f;  // when this reaches a threshold, the player will experience withdrawal symptoms
  public int withdrawalThreshold = 20; // how many seconds of not drinking alcohol before withdrawal symptoms kick in
  
  private CanvasGroup blackoutCanvasGroup;  // A reference to control every object in the same canvas as the blackout panel

  private AudioSource[] audioSources;

  // public float bottlex, bottley, bottlez;   // Controls the angle the bottle is tilted to during the drinking animations

  private InputAction abilityAction;

  public GameObject DrunkDriverPLayer;
  private UFOMovement playerMovement; // reference to the player's movement script

  private bool wasDrinking = false;

  private bool canDrink = true;  // when blacking out, you can't drink

  // Implement interface functions to set movement modifiers
  public float GetAccelerationMultiplier() => GetAlcoholMultiplier() * 2f;
  public float GetReverseMultiplier() => GetAlcoholMultiplier() * 0.7f;
  public float GetBrakeMultiplier() => GetAlcoholMultiplier();
  public float GetTurnMultiplier() => (withdrawalSymptom ? Mathf.Pow(1.6f, GetAlcoholMultiplier()) * (GetAlcoholMultiplier() / 2f) : 1.0f);
  public float GetMaxSpeedMultiplier() => GetAlcoholMultiplier() * 2f;


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    alcoholSupply = initialAlcoholSupply;
    alcoholCount = initialAlcoholCount;
    Shader.SetGlobalFloat("GlobalAlcoholCount", initialAlcoholCount);
    
    DrinkToolTip.SetActive(false); // hide the drink tooltip at start

    if (blackoutPanel != null)
    {
      Debug.Log("blackoutPanel found.");
      blackoutCanvasGroup = blackoutPanel.GetComponent<CanvasGroup>();
      if (blackoutCanvasGroup == null)
      {
        Debug.Log("CanvasGroup not found, adding one.");
        blackoutCanvasGroup = blackoutPanel.AddComponent<CanvasGroup>();
      }

      blackoutCanvasGroup.alpha = 0;
    }
    else
    {
      Debug.LogError("blackoutPanel not found in the scene!");
    }

    audioSources = GetComponents<AudioSource>();
    if (audioSources.Length < 2)
    {
      Debug.LogError("Add at least two AudioSource components to this GameObject.");
    }

    if (alcoholSupply > 0 && canDrink)  // force first drink.
    {
      StartCoroutine(DrinkAlcohol());
    }

    // get UFOMovement component from the player
    if (DrunkDriverPLayer != null)
    {
      playerMovement = DrunkDriverPLayer.GetComponent<UFOMovement>();
      if (playerMovement == null)
      {
        Debug.LogError("UFOMovement component not found on the DrunkDriverPLayer GameObject!");
      }
    }
    else
    {
      Debug.LogError("DrunkDriverPLayer GameObject not assigned in the AlcoholManager!");
    }
  }

  public float GetAlcoholCount()
  {
    return alcoholCount;
  }
  
  public bool GetIsDrinking() // returns true if the player is currently drinking alcohol
  {
    return !canDrink;
  }
  
  public void increaseAlcoholCount(float amount = 1)
  {
    alcoholCount += amount;
    // if (alcoholCount > 1)
    // { 
    //   roundManager.increaseAlcoholFine();  
    // }
    
    Shader.SetGlobalFloat("GlobalAlcoholCount", alcoholCount);

    if (alcoholCountUI != null)
    {
      // turn count into blood alc percent. E.g. level 1 is 0.01%, level 10 is 0.1%, level 20 is 0.2%, etc. level 101 is 1.01 
      alcoholCountUI.text = (alcoholCount / 100f).ToString("F2") + "%";
    }
    else
    {
      Debug.LogError("AlcoholCounterUI not found!");
    }
  }

  public float GetAlcoholMultiplier()
  {
    // alcohol multiplier is ((alcoholCount - 1) / 10) + 1
    return ((alcoholCount - 1) / 10f) + 1;
  }
  public float GetAlcoholSupply()
  {
    return alcoholSupply;
  }

  public void changeAlcoholSupply(float amount = 1) // use -1 to decrease
  {
    alcoholSupply += amount;

    if (alcoholSupply < 0)
    {
      alcoholSupply = 0;
    }

    // update text
    if (capacityRectangle != null)
    {
      // should follow an asymptote so that the bottle never fulls - this gives infinite capacity but good indicator of supply
      // the first 10 drinks should take up about the first 80% of the hight
      float height = Mathf.Clamp(capacityRectangleMaxHeight * (1 - (Mathf.Exp(-alcoholSupply / 10f))), 0,
        capacityRectangleMaxHeight);
      capacityRectangle.rectTransform.sizeDelta = new Vector2(capacityRectangle.rectTransform.sizeDelta.x, height);
    }
    
    if (amount > 0) // if we are increasing the alcohol supply, shake the bottle
    {
      bottle.GetComponent<shakeBottle>().setShakeTimer();
    }
  }


  // Update is called once per frame
  void Update()
  {
    withdrawalTimer += Time.deltaTime;  // increase withdrawal timer by the time since last frame
    //Debug.Log("Withdrawal timer: " + withdrawalTimer);
    if (withdrawalTimer >= withdrawalThreshold && !wasDrinking)
    {
      withdrawalSymptom = true;  // player is experiencing withdrawal symptoms
      //Debug.Log("Withdrawal symptoms are kicking in!");
      if (alcoholSupply > 0)
      {
        DrinkToolTip.SetActive(true); // show the drink tooltip
      }
      // change colour of alcohol capacity rectangle to red
      if (capacityRectangle != null)
      {
        capacityRectangle.color = Color.red;
      }
      else
      {
        //Debug.LogError("CapacityRectangle not found!");
      }
      
      //Debug.Log("Withdrawal symptoms are kicking in!");
    }
    
    
    if (playerInput == null) return;
    abilityAction = playerInput.actions["Ability"];
    if (abilityAction == null) return;

    // press space to initiate drink alcohol routine
    // if (abilityAction.WasPressedThisFrame())
    // {
    //   if (alcoholSupply > 0 && canDrink)
    //   {
    //     StartCoroutine(DrinkAlcohol());
    //   }
    //   else if (alcoholSupply <= 0)
    //   {
    //     Debug.Log("No alcohol supply left!");
    //   }
    //   else if (!canDrink)
    //   {
    //     Debug.Log("Can't drink while blacking out!");
    //   }
    //
    // }
    if (alcoholSupply > 0  && abilityAction.IsPressed())
    {
      if (!wasDrinking)
      {
        withdrawalTimer = 0;
        withdrawalSymptom = false;  // player is not experiencing withdrawal symptoms
        DrinkToolTip.SetActive(false); // hide the drink tooltip
        capacityRectangle.color = new Color(0, 0, 200);
      }
      wasDrinking = true;
      // change colour of alcohol capacity rectangle to blue
     
      // apply large force to the player (speed boost)
      playerMovement.speedBoost(1000 * GetAlcoholMultiplier());
      // so, alcohol bottle fills up with a logarithmic function with x being the alcohol supply
      // we want to deplete the alcohol supply so that the bottle will decrease linearlu, i.e. reverse log for supply depletion, but we still must track supply decreasing so we know to take a 1 to 1 increase of alcohol blood percentage per supply depleted
      increaseAlcoholCount(0.25f);
      changeAlcoholSupply(-0.25f); // decrease alcohol supply by 0.25

      if (!audioSources[0].isPlaying)
      {
        audioSources[0].Play();
      }
    }
    else if (wasDrinking && !abilityAction.WasReleasedThisFrame())
    {
      wasDrinking = false;
      // blackout
      if (alcoholCount >= 3 && UnityEngine.Random.Range(0, 100) < 40 + (Math.Pow(2, GetAlcoholMultiplier())))
      {
        TriggerBlackout();
      }

      if (alcoholSupply < 1.0f)
      {
        changeAlcoholSupply(-1.0f * alcoholSupply); // draing alcohol supply by the remaining amount
      }
      capacityRectangle.color = new Color(0.529f, 0.337f, 0.325f); // brownish color
    }
    else
    {
      
    }
  }

  // function to drink alcohol
  private IEnumerator DrinkAlcohol()
  {
    canDrink = false;
    // reset withdrawal timer
    withdrawalTimer = 0;
    withdrawalSymptom = false;  // player is not experiencing withdrawal symptoms
    DrinkToolTip.SetActive(false); // hide the drink tooltip
    // fix colour of alcohol capacity rectangle
    if (capacityRectangle != null)
    {
      capacityRectangle.color = new Color(0.529f, 0.337f, 0.325f); // brownish color
    }
    else
    {
      Debug.LogError("CapacityRectangle not found!");
    }
    
    yield return StartCoroutine(PlayAndWaitForSoundToFinish(audioSources[0]));

    // drink increase same as supply decrease
    increaseAlcoholCount(1);
    changeAlcoholSupply(-1);

    // chance to black out for a split second
    if (alcoholCount >= 3 && UnityEngine.Random.Range(0, 100) < 40 + (Math.Pow(2, GetAlcoholMultiplier())))
    {
      TriggerBlackout();
    }
    else
    {
      if (UnityEngine.Random.Range(0, 100) < 25)
      {
        yield return StartCoroutine(PlayAndWaitForSoundToFinish(audioSources[1]));
      }
    }
    canDrink = true;
  }
  



  private IEnumerator PlayAndWaitForSoundToFinish(AudioSource audioSource)
  {
    if (audioSource == null)
    {
      Debug.LogError("AudioSource is null!");
      yield break;
    }
    else
    {
      audioSource.Play();
    }
    // Wait for the sound to finish
    while (audioSource.isPlaying)
    {
      yield return null;
    }
  }

  public void TriggerBlackout()
  {
    if (blackoutCanvasGroup != null)
    {
      StartCoroutine(BlackoutRoutine());
    }
    else
    {
      Debug.LogError("BlackoutCanvasGroup is null!");
    }
  }

  private IEnumerator BlackoutRoutine()
  {
    canDrink = false;

    // Phase 1: Increase alpha from 0 to 1 over 0.1 seconds
    float duration = 0.1f;
    for (float t = 0; t < duration; t += Time.deltaTime)
    {
      blackoutPanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, t / duration);
      yield return null;
    }
    blackoutPanel.GetComponent<CanvasGroup>().alpha = 1;

    // Phase 2: Hold alpha at 1 for 0.3 seconds
    yield return new WaitForSeconds(0.3f);

    // Phase 3: Decrease alpha from 1 to 0 over 0.1 seconds
    duration = 0.1f;
    for (float t = 0; t < duration; t += Time.deltaTime)
    {
      blackoutPanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, t / duration);
      yield return null;
    }
    blackoutPanel.GetComponent<CanvasGroup>().alpha = 0;  // for safety

    // Deactivate the panel after the blackout effect
    canDrink = true;
  }

  public void RefreshAlcoholManager()  // invoked by round manager's reset scene event
  {
    alcoholSupply = initialAlcoholSupply;
    alcoholCount = initialAlcoholCount;
    Shader.SetGlobalInt("GlobalAlcoholCount", initialAlcoholCount);
    if (alcoholCountUI != null)
    {
      // turn count into blood alc percent. E.g. level 1 is 0.01%, level 10 is 0.1%, level 20 is 0.2%, etc. level 101 is 1.01 
      alcoholCountUI.text = (alcoholCount / 100f).ToString("F2") + "%";
    }
    else
    {
      Debug.LogError("AlcoholCounterUI not found!");
    }
    if (capacityRectangle != null)
    {
      // reset rectangle to height 2
      capacityRectangle.rectTransform.sizeDelta = new Vector2(capacityRectangle.rectTransform.sizeDelta.x, 2);
    }
    else
    {
      Debug.LogError("CapacityRectangle not found!");
    }

    if (alcoholSupply > 0 && canDrink)  // force first drink.
    {
      StartCoroutine(DrinkAlcohol());
    }
  }
}


// def f(x):
// ...     return 100 * (1.6 ** x) * (x / 2)
