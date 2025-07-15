// BarManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarManager : MonoBehaviour
{
    [Tooltip("How many bars may be open simultaneously. " +
             "If <= 0, defaults to half the total (rounded up).")]
    public float simultaneousOpenPercent = 100.0f;

    private readonly List<Bar> bars = new();
    private readonly List<Bar> closed = new();
    private readonly List<Bar> open = new();
    
    private bool isVisitingBar = false;
    private float visitTimeLimit = 2.0f;
    public Image barVisitProgress; // UI element to show bar visit progress
    private float timeSpentVisiting = 0.0f; // Time spent visiting the bar
    public TMP_Text visitTooltip;

    public GameObject alertMinimapIcon;
    private alertMinimapMarker alertManager;
    
    public float closingDuration = 10.0f; // seconds till bar closes after being visited


    public GameObject AlcoholManager;
    private AlcoholManager alcoholManager;

    public AlertCopOfDDLocationEventSender alertCopOfDDLocationEventSender;

    public AudioSource collectSound;

    void Start()
    {
        if (AlcoholManager == null)
        {
            Debug.LogError("AlcoholManager not found!");
        }
        alcoholManager = AlcoholManager.GetComponent<AlcoholManager>();

        if (alertMinimapIcon == null)
        {
            Debug.LogError("alertMinimapIcon not found!");
        }

        alertManager = alertMinimapIcon.GetComponent<alertMinimapMarker>();
        
        CollectBars();
        OpenInitialSet(true);

        visitTooltip.text = "";
        if (barVisitProgress != null)
        {
            barVisitProgress.fillAmount = 0.0f; // Initialize the progress bar
        }
        else
        {
            Debug.LogWarning("Bar visit progress UI element is not assigned.");
        }
    }

    // /* Called by a Bar that has just been visited. */
    // public void NotifyBarVisited(Bar bar)
    // {
    //     // Defensive check in case the bar wasn’t tracked.
    //     if (!open.Contains(bar)) return;
    //
    //     bar.SetClosed();
    //     open.Remove(bar);
    //     
    //     //play sound (use audio source)
    //     collectSound.Play();
    //     
    //     alcoholManager.changeAlcoholSupply(1);  // increase alcohol supply by 1
    //     alertCopOfDDLocationEventSender.Trigger(new Vector2(bar.transform.position.x, bar.transform.position.z)); // Alert cop of drunk driver
    //
    //     AssignReplacementBar();
    //     closed.Add(bar);
    // }

    public void NotifyBarDoneVisit(Bar bar, bool completed)
    {
        isVisitingBar = false;
        if (completed)
        {
            open.Remove(bar);
            closed.Add(bar);
            bar.SetClosed(closingDuration);
            collectSound.Play();
            alcoholManager.changeAlcoholSupply(1);
        }
        alertManager?.RecieveAlert(1.0f, true);
        barVisitProgress.fillAmount = 0.0f; // reset the progress bar
        timeSpentVisiting = 0.0f; // reset the time spent visiting
        visitTooltip.text = ""; // clear the tooltip text
    }
    
    public void NotifyBarBeginVisit(Bar bar)
    {
        if (isVisitingBar)
        {
            Debug.LogWarning("Already visiting a bar. Cannot start a new visit.");
            return;
        }
        isVisitingBar = true;
        alertManager?.RecieveAlert(bar.visitTimeLimit, true);
        barVisitProgress.fillAmount = 0.0f; // reset the progress bar
        timeSpentVisiting = 0.0f; // reset the time spent visiting
        visitTimeLimit = bar.visitTimeLimit; // set the visit time limit from the bar
        visitTooltip.text = $"Visiting Bar...";
    }

    public void NotifyBarReopen(Bar bar)
    {
        bar.SetOpen();
        open.Add(bar);
        closed.Remove(bar);
    }
    
    void Update()
    {
        if (isVisitingBar)
        {
            timeSpentVisiting += Time.deltaTime;
            barVisitProgress.fillAmount = Mathf.Min(1.0f, timeSpentVisiting / visitTimeLimit);
        }
    }

    /* ---------- internal helpers ---------- */

    private void CollectBars()
    {
        bars.Clear();
        closed.Clear();
        open.Clear();

        GetComponentsInChildren(true, bars);          // cheap hierarchy scan
        
        //print to debug console
        foreach (var b in bars)
        {
            b.Manager = this;                         // back-reference
            b.SetClosed(0.0f); // set all bars to closed initially
            closed.Add(b);
        }
    }

    private void OpenInitialSet(bool openAll)
    {
        if (openAll)
        {
            foreach (var bar in closed)
            {
                bar.SetOpen();
                open.Add(bar);
            }
            closed.Clear();
        }
        else
        {
            var targetOpen = simultaneousOpenPercent > 0
                ? Mathf.CeilToInt(bars.Count * (simultaneousOpenPercent / 100f))
                : Mathf.CeilToInt(bars.Count / 2f);  // default to half

            for (var i = 0; i < targetOpen && closed.Count > 0; i++)
            {
                PromoteRandomClosedBar();
            }
        }
        
    }

    private void AssignReplacementBar()
    {
        if (closed.Count == 0) return;                // everything is already open
        PromoteRandomClosedBar();
    }

    private void PromoteRandomClosedBar()
    {
        var idx = Random.Range(0, closed.Count);
        var b   = closed[idx];
        closed.RemoveAt(idx);

        b.SetOpen();
        open.Add(b);
    }
    
    public void ReassignAllBars()  // invoked by round manager's reset scene event
    {
        // set all bars to closed then open 2 bars
        foreach (var bar in bars)
        {
            bar.SetClosed(0.0f);
        }
        open.Clear();
        closed.Clear();
        CollectBars();
        OpenInitialSet(true);
    }

    public void FindAllUFOonReset()
    {
        foreach (var bar in bars)
        {
            bar.FindDrunkPlayer();
        }
    }
}