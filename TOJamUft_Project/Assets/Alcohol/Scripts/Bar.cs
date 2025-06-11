using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Bar : MonoBehaviour
{
    public Material openMaterial;
    public Material closedMaterial;
    
    private GameObject openModel;
    private GameObject closedModel;
    
    private Renderer rend;
    public bool IsOpen { get; private set; }
    private bool isVisited = false;
    
    public Transform player;
    public float activationRadius = 100.0f;
    private float activationRadiusSqr;
    
    internal BarManager Manager { get; set; }

    void Awake()
    {   
        openModel = transform.Find("OpenModel")?.gameObject;
        closedModel = transform.Find("ClosedModel")?.gameObject;

        if (openModel == null || closedModel == null) {
            Debug.LogError("Missing OpenModel or ClosedModel in children!");
        }
        
        rend = GetComponent<Renderer>();
        if (player == null)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }
        
        var size = GetComponent<Collider>().bounds.size;
        activationRadiusSqr = Mathf.Pow(activationRadius, 2);
    }

    void Update()
    {
        // get x and z distance only
        Vector3 diff = player.position - transform.position;
        float dist = Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
        if (IsOpen && !isVisited && player != null && dist < Mathf.Sqrt(activationRadiusSqr))
        {
            isVisited = true;
            Manager?.NotifyBarVisited(this);
        }
    }

    internal void SetOpen()
    {
        Debug.Log($"Bar {gameObject.name} set open");
        IsOpen = true;
        isVisited = false;
        openModel.SetActive(true);
        closedModel.SetActive(false);
        if (rend != null && openMaterial != null)
        {
            rend.material = openMaterial;
        }
    }

    internal void SetClosed()
    {
        Debug.Log($"Bar {gameObject.name} set closed");
        IsOpen = false;
        isVisited = false;
        openModel.SetActive(false);
        closedModel.SetActive(true);
        if (rend != null && closedMaterial != null)
        {
            rend.material = closedMaterial;
        }
    }
}