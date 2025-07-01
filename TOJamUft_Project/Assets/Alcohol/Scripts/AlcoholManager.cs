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
  public int capacityRectangleMaxHeight = 150; // the maximum height of the rectangle in pixels
  
  //UI text
  public TMP_Text alcoholCountUI;

  public int initialAlcoholCount;
  public int initialAlcoholSupply;

  [SerializeField] private PlayerInput playerInput;

  [SerializeField]
  private FineManagerBehavior fineManager;

  private int alcoholCount;                 // The number of alcohol bottles 
  private int alcoholSupply;              // The number of alcohol bottles available
  
  
  private bool withdrawalSymptom = false;  // If true, the player is experiencing withdrawal symptoms and can't drink alcohol
  private float withdrawalTimer = 0.0f;  // when this reaches a threshold, the player will experience withdrawal symptoms
  public int withdrawalThreshold = 20; // how many seconds of not drinking alcohol before withdrawal symptoms kick in
  
  private CanvasGroup blackoutCanvasGroup;  // A reference to control every object in the same canvas as the blackout panel

  private AudioSource[] audioSources;

  // public float bottlex, bottley, bottlez;   // Controls the angle the bottle is tilted to during the drinking animations

  private InputAction abilityAction;

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
    Shader.SetGlobalInt("GlobalAlcoholCount", initialAlcoholCount);

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
  }

  public int GetAlcoholCount()
  {
    return alcoholCount;
  }
  public void increaseAlcoholCount(int amount = 1)
  {
    alcoholCount += amount;
    fineManager.increaseFine(100);
    Shader.SetGlobalInt("GlobalAlcoholCount", alcoholCount);

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
  public int GetAlcoholSupply()
  {
    return alcoholSupply;
  }

  public void changeAlcoholSupply(int amount = 1) // use -1 to decrease
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
  }


  // Update is called once per frame
  void Update()
  {
    withdrawalTimer += Time.deltaTime;  // increase withdrawal timer by the time since last frame
    //Debug.Log("Withdrawal timer: " + withdrawalTimer);
    if (withdrawalTimer >= withdrawalThreshold)
    {
      withdrawalSymptom = true;  // player is experiencing withdrawal symptoms
      //Debug.Log("Withdrawal symptoms are kicking in!");
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
    if (abilityAction.WasPressedThisFrame())
    {
      if (alcoholSupply > 0 && canDrink)
      {
        StartCoroutine(DrinkAlcohol());
      }
      else if (alcoholSupply <= 0)
      {
        Debug.Log("No alcohol supply left!");
      }
      else if (!canDrink)
      {
        Debug.Log("Can't drink while blacking out!");
      }

    }
  }

  // function to drink alcohol
  private IEnumerator DrinkAlcohol()
  {
    canDrink = false;
    // reset withdrawal timer
    withdrawalTimer = 0;
    withdrawalSymptom = false;  // player is not experiencing withdrawal symptoms
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
