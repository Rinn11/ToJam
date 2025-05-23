/*
 * Manages the current fine of the player.
 * Increases the fine based on a rate from the alcohol multiplier and updates the relevant UI element.
 */

using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class FineManagerBehavior : MonoBehaviour
{
    public float fine;
    public AlcoholManager am;
    private AlcoholManager alcoholManager;
    public Text fineUI;

    void Start()
    {
        fine = 0.0f;
        alcoholManager = am.GetComponent<AlcoholManager>();
    }
    
    public void increaseFine(int amount)
    {
        fine += amount;
        fine = (float) Math.Round(fine, 2);

        if (fineUI != null)
        {
            fineUI.text = $"Fine: ${fine}";
        }
    }
    
    // // Update is called once per frame
    // void Update()
    // {
    //
    // }
}
