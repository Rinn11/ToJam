/*
 * Manages the effects of alcohol:
 * - Applies a blackout effect
 * - Applies a blurring effect
 * - Updates an alcohol counter component
 * Also animates the bottle being drank.
 */

/*
 * Implements IMovementModifier, which specifies the statistics of the car.
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class AlcoholManager : MonoBehaviour, IMovementModifier
{
    public GameObject blackoutPanel;          // The panel used to simulate a blackout
    public GameObject blurryPanel;            // The panel used to simulate a blurring effect
    public GameObject bottle;                 // The alcohol bottle

    //UI text
    public Text alcoholCountUI;
    public Text alcoholSupplyUI;

    public int initialAlcoholCount;
    public int initialAlcoholSupply;

    public GameObject FineManager;
    private FineManagerBehavior fineManager;

    private int alcoholCount;                 // The number of alcohol bottles 
    private int alcoholSupply;              // The number of alcohol bottles available

    private CanvasGroup blackoutCanvasGroup;  // A reference to control every object in the same canvas as the blackout panel
    private CanvasGroup blurryCanvasGroup;    // A reference to control every object in the same canvas as the blurry panel

    private AudioSource[] audioSources;

    public float bottlex, bottley, bottlez;   // Controls the angle the bottle is tilted to during the drinking animations


    private bool canDrink = true;  // when blacking out, you can't drink

    // Implement interface functions to set movement modifiers
    public float GetAccelerationMultiplier() => GetAlcoholMultiplier() * 2f;
    public float GetReverseMultiplier() => GetAlcoholMultiplier() * 0.7f;
    public float GetBrakeMultiplier() => GetAlcoholMultiplier();
    public float GetTurnMultiplier() => Mathf.Pow(1.6f, GetAlcoholMultiplier()) * (GetAlcoholMultiplier() / 2f);
    public float GetMaxSpeedMultiplier() => GetAlcoholMultiplier() * 2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alcoholSupply = initialAlcoholSupply;
        alcoholCount = initialAlcoholCount;

        if (FineManager == null)
        {
            Debug.LogError("FineManager not found!");
        }
        fineManager = FineManager.GetComponent<FineManagerBehavior>();

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

        if (blurryPanel != null)
        {
            Debug.Log("blurryPanel found.");
            blurryCanvasGroup = blurryPanel.GetComponent<CanvasGroup>();
            if (blurryCanvasGroup == null)
            {
                Debug.Log("CanvasGroup not found, adding one.");
                blurryCanvasGroup = blurryPanel.AddComponent<CanvasGroup>();
            }

            blurryCanvasGroup.alpha = 0;
        }
        else
        {
            Debug.LogError("blackoutPanel not found in the scene!");
        }

        bottlex = 140f;
        bottley = -15f;
        bottlez = 0f;

        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            Debug.LogError("Add at least two AudioSource components to this GameObject.");
        }
    }

    public int GetAlcoholCount()
    {
        return alcoholCount;
    }
    public void increaseAlcoholCount(int amount = 1)
    {
        alcoholCount += amount;
        fineManager.increaseFine(100);  // 100 dollar fine per bottle - TODO: fix this from being "magic num"

        if (alcoholCountUI != null)
        {
            alcoholCountUI.text = "Alcohol Count: " + alcoholCount;
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
        if (alcoholSupplyUI != null)
        {
            alcoholSupplyUI.text = "Alcohol Supply: " + alcoholSupply;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // press space to initiate drink alcohol routine
        if (alcoholSupply > 0 && canDrink && Input.GetKeyDown(KeyCode.Space))
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

    private void FixedUpdate()
    {

    }

    // function to drink alcohol
    private IEnumerator DrinkAlcohol()
    {
        canDrink = false;

        // grab bottle animation, drink animation + gulp sfx, then increase alcoholCount
        Transform bottleTransform = bottle.transform;

        // // Record bottle's original position and rotation
        Vector3 originalLocalPos = bottleTransform.localPosition;
        Quaternion originalLocalRot = bottleTransform.localRotation;
        yield return StartCoroutine(AlcoholMove());
        yield return StartCoroutine(PlayAndWaitForSoundToFinish(audioSources[0]));

        // drink increase same as supply decrease
        increaseAlcoholCount(1);
        changeAlcoholSupply(-1);

        // increase blurriness
        CanvasGroup panel = blurryPanel.GetComponent<CanvasGroup>();
        panel.alpha = MathF.Min(0.8f, (GetAlcoholMultiplier() - 1) / 10);

        // chance to black out for a split second
        if (alcoholCount > 5 && UnityEngine.Random.Range(0, 100) < 40 + (Math.Pow(2, GetAlcoholMultiplier())))
        {
            TriggerBlackout();
            yield return StartCoroutine(AlcoholReturnLocal(originalLocalPos, originalLocalRot));
            canDrink = true;
        }
        else
        {
            if (UnityEngine.Random.Range(0, 100) < 25)
            {
                yield return StartCoroutine(PlayAndWaitForSoundToFinish(audioSources[1]));
            }
            yield return StartCoroutine(AlcoholReturnLocal(originalLocalPos, originalLocalRot));
            canDrink = true;
        }
    }

    private IEnumerator AlcoholMove()
    {
        Transform bottleTransform = bottle.transform;
        Transform cameraTransform = Camera.main.transform;

        Vector3 startLocalPos = bottleTransform.localPosition;
        Quaternion startLocalRot = bottleTransform.localRotation;

        Vector3 bottleTopOffset = bottleTransform.up * (-0.2f); // adjust 0.2f based on bottle size


        // Compute local target position relative to bottle's parent (e.g., car)
        Vector3 targetWorldPos = cameraTransform.position + cameraTransform.forward * 0.5f - bottleTopOffset;
        Vector3 targetLocalPos = bottleTransform.parent.InverseTransformPoint(targetWorldPos);
        // Quaternion targetLocalRot = Quaternion.Inverse(bottleTransform.parent.rotation) * cameraTransform.rotation;

        // Base rotation facing the camera
        Quaternion cameraRot = cameraTransform.rotation;

        // Apply a tilt so the bottle isn't upside down
        Quaternion bottleTilt = Quaternion.Euler(bottlex, bottley, bottlez); // adjust X angle as needed

        // Final rotation is camera-facing plus tilt (in world space)
        Quaternion targetWorldRot = cameraRot * bottleTilt;

        // Convert to local space
        Quaternion targetLocalRot = Quaternion.Inverse(bottleTransform.parent.rotation) * targetWorldRot;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            bottleTransform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            bottleTransform.localRotation = Quaternion.Slerp(startLocalRot, targetLocalRot, t);

            yield return null;
        }

        bottleTransform.localPosition = targetLocalPos;
        bottleTransform.localRotation = targetLocalRot;
    }


    private IEnumerator AlcoholReturnLocal(Vector3 originalLocalPos, Quaternion originalLocalRot)
    {
        Transform bottleTransform = bottle.transform;

        Vector3 startLocalPos = bottleTransform.localPosition;
        Quaternion startLocalRot = bottleTransform.localRotation;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            bottleTransform.localPosition = Vector3.Lerp(startLocalPos, originalLocalPos, t);
            bottleTransform.localRotation = Quaternion.Slerp(startLocalRot, originalLocalRot, t);

            yield return null;
        }

        bottleTransform.localPosition = originalLocalPos;
        bottleTransform.localRotation = originalLocalRot;
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
}


// def f(x):
// ...     return 100 * (1.6 ** x) * (x / 2)