using System;
using System.Collections;
using UnityEngine;

// TODO: cooldown timer for swigs of drink. UI icon should be a bottle of alcohol refilling to show cooldown restoring


public class AlcoholManager : MonoBehaviour
{
    public GameObject blackoutPanel;
    public GameObject blurryPanel;
    private int alcoholCount;
    // blackout gui panel gameobject
    //private GameObject blackoutPanel;
    private GameObject alcoholCounterUI;
    private CanvasGroup blackoutCanvasGroup;
    private CanvasGroup blurryCanvasGroup;
    
    private AudioSource[] audioSources;

    
    private bool canDrink = true;  // when blacking out, you can't drink
    
    // public function that returns alcoholMultiplier and alcoholCount
    public int GetAlcoholCount()
    {
        return alcoholCount;
    }
    public float GetAlcoholMultiplier()
    {
        // alcohol multiplier is ((alcoholCount - 1) / 10) + 1
        return ((alcoholCount - 1) / 10f) + 1;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alcoholCount = 1;
        //blackoutPanel = GameObject.Find("blackoutPanel");
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
        
        
        alcoholCounterUI = GameObject.Find("AlcoholCounter");
        audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            Debug.LogError("Add at least two AudioSource components to this GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // press space to initiate drink alcohol routine
        if (canDrink && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DrinkAlcohol());
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
        
        yield return StartCoroutine(PlayAndWaitForSoundToFinish(audioSources[0]));
        
        // if alcoholCount is less than 10, increase alcoholCount by 1
        alcoholCount++;
        // do other stuff like animate drinking, play sound, increase ui text 
        
        // increase blurriness
        CanvasGroup panel = blurryPanel.GetComponent<CanvasGroup>();
        panel.alpha = MathF.Min(0.8f, (GetAlcoholMultiplier() - 1) / 10);
        
        if (alcoholCounterUI != null)
        {
            // Assuming the alcoholCounterUI has a Text component
            var textComponent = alcoholCounterUI.GetComponent<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                textComponent.text = "Alcohol Count: " + alcoholCount;
            }
        }
        
        
        // chance to black out for a split second
        
        if (alcoholCount > 5 && UnityEngine.Random.Range(0, 100) < 40 + (Math.Pow(2, GetAlcoholMultiplier())))
        {
            TriggerBlackout();
        }
        else
        {
            if (UnityEngine.Random.Range(0, 100) < 25)
            {
                yield return StartCoroutine(PlayAndWaitForSoundToFinish(audioSources[1]));
            }
            canDrink = true;
        }
       
    }
    
    private IEnumerator PlayAndWaitForSoundToFinish(AudioSource audioSource)
    {   
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is null!");
            yield break;
        } else 
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
        } else 
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