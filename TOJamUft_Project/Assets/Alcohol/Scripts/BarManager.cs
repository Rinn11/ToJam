// BarManager.cs
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour
{
    [Tooltip("How many bars may be open simultaneously. " +
             "If <= 0, defaults to half the total (rounded up).")]
    public float simultaneousOpenPercent = 50.0f;

    private readonly List<Bar> bars = new();
    private readonly List<Bar> closed = new();
    private readonly List<Bar> open = new();

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
        
        CollectBars();
        OpenInitialSet();
    }

    /* Called by a Bar that has just been visited. */
    public void NotifyBarVisited(Bar bar)
    {
        // Defensive check in case the bar wasn’t tracked.
        if (!open.Contains(bar)) return;

        bar.SetClosed();
        open.Remove(bar);
        
        //play sound (use audio source)
        collectSound.Play();
        
        alcoholManager.changeAlcoholSupply(1);  // increase alcohol supply by 1
        alertCopOfDDLocationEventSender.Trigger(new Vector2(bar.transform.position.x, bar.transform.position.z));

        AssignReplacementBar();
        closed.Add(bar);
    }

    /* ---------- internal helpers ---------- */

    private void CollectBars()
    {
        bars.Clear();
        closed.Clear();
        open.Clear();

        GetComponentsInChildren(true, bars);          // cheap hierarchy scan
        
        //print to debug console
        Debug.Log($"Found {bars.Count} bars in the scene.");
        foreach (var b in bars)
        {
            b.Manager = this;                         // back-reference
            b.SetClosed();
            closed.Add(b);
        }
    }

    private void OpenInitialSet()
    {
        var targetOpen = simultaneousOpenPercent > 0
            ? Mathf.CeilToInt(bars.Count * (simultaneousOpenPercent / 100f))
            : Mathf.CeilToInt(bars.Count / 2f);  // default to half

        for (var i = 0; i < targetOpen && closed.Count > 0; i++)
        {
            PromoteRandomClosedBar();
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
            bar.SetClosed();
        }
        open.Clear();
        closed.Clear();
        CollectBars();
        OpenInitialSet();
        Debug.Log("Reassigned all bars.");
    }
}