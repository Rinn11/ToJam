using System;
using System.ComponentModel;
using UnityEngine;

public class FineManagerBehavior : MonoBehaviour
{
    public float fine;
    public AlcoholManager am;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(am.GetAlcoholMultiplier());
        fine += Time.deltaTime * am.GetAlcoholMultiplier();
        fine = (float) Math.Round(fine, 2);

        Debug.Log(fine);
    }
}
