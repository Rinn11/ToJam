/*
 * Manages the current fine of the player.
 * Increases the fine based on a rate from the alcohol multiplier and updates the relevant UI element.
 */

using System;
using System.ComponentModel;
using UnityEngine;

public class FineManagerBehavior : MonoBehaviour
{
    public float fine;
    public AlcoholManager am;
    private GameObject fineUI;

    void Start()
    {
        fineUI = GameObject.Find("Fine");
    }
    
    // Update is called once per frame
    void Update()
    {
        fine += Time.deltaTime * am.GetAlcoholMultiplier() * 10;
        fine = (float) Math.Round(fine, 2);
        
        if (fineUI != null)
        {
            // Assuming the alcoholCounterUI has a Text component
            var textComponent = fineUI.GetComponent<UnityEngine.UI.Text>(); // TODO: Optimize by caching (bad idea to do this every frame)
            if (textComponent != null)
            {
                textComponent.text = "Fine: $" + fine;
            }
        }
    }
}
