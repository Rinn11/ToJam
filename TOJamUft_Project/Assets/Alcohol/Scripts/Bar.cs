using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class Bar : MonoBehaviour
{
    private GameObject openModel;
    private GameObject closedModel;

    public ParticleSystem greenEffect;

    public RawImage mapIcon;

    public bool IsOpen { get; private set; }
    private bool isVisited = false;
    
    public GameObject player;
    private Transform playerTransform;
    public float activationRadius = 70.0f;
    private float activationRadiusSqr;
    
    private float visitTime = 0.0f;
    public float visitTimeLimit = 2.0f; // seconds to get the fill up at bar 
    private float timeTillOpen = 0.0f; // time till bar opens after being closed
    
    internal BarManager Manager { get; set; }

    void Awake()
    {   
        openModel = transform.Find("open_bar")?.gameObject;
        closedModel = transform.Find("closed_bar")?.gameObject;

        if (openModel == null || closedModel == null) {
            Debug.LogError("Missing open_bar or closed_bar in children!");
        }
        
        playerTransform = player?.transform;
        
        activationRadiusSqr = Mathf.Pow(activationRadius, 2);
        greenEffect.startSize = activationRadiusSqr * 4.3f;
    }

    public void FindDrunkPlayer()
    {
        openModel = transform.Find("OpenModel")?.gameObject;
        closedModel = transform.Find("ClosedModel")?.gameObject;

        if (openModel == null || closedModel == null)
        {
            Debug.LogError("Missing OpenModel or ClosedModel in children!");
        }

        //rend = GetComponent<Renderer>();
        if (player == null)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found;
        }

        var size = GetComponent<Collider>().bounds.size;
        activationRadiusSqr = Mathf.Pow(activationRadius, 2);
    }

    void Update()
    {
        // get x and z distance only
        Vector3 diff = playerTransform.position - mapIcon.transform.position;
        float dist = Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
        if (IsOpen && !isVisited && player != null && dist < activationRadius)
        {
            isVisited = true;
            Manager?.NotifyBarBeginVisit(this); // notify the manager that the bar visit has begun
        }
        if (isVisited && IsOpen)
        {
            visitTime += Time.deltaTime;
            if (visitTime >= visitTimeLimit)
            {
                exitedBar(true);
            }
            else if (dist > Mathf.Sqrt(activationRadiusSqr))
            {  // if you exit before the time reaches full
                exitedBar(false);
            }
        }

        if (!IsOpen && timeTillOpen > 0.0f && !isVisited)
        {
            timeTillOpen -= Time.deltaTime;
            if (timeTillOpen <= 0.0f)
            {
                timeTillOpen = 0.0f;
                Manager?.NotifyBarReopen(this);
            }
        }
    }

    internal void SetOpen()
    {
        IsOpen = true;
        isVisited = false;
        openModel.SetActive(true);
        closedModel.SetActive(false);
        mapIcon.color = Color.yellowNice;
    }

    internal void SetClosed(float closingDuration)
    {
        IsOpen = false;
        isVisited = false;
        openModel.SetActive(false);
        closedModel.SetActive(true);
        mapIcon.color = Color.grey;
        timeTillOpen = closingDuration;
    }

    internal void exitedBar(bool completed)
    {
        visitTime = 0.0f; // reset visit time
        isVisited = false; // reset visited state
        Manager?.NotifyBarDoneVisit(this, completed); // notify the manager that the bar visit is done
    }
}