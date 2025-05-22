using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Bar : MonoBehaviour
{
    public Material openMaterial;
    public Material closedMaterial;
    private Renderer rend;
    public bool IsOpen { get; private set; }
    private bool isVisited = false;
    
    public Transform player;
    public float activationRadius = 100.0f;
    private float activationRadiusSqr;
    
    internal BarManager Manager { get; set; }

    void Awake()
    {
        Debug.Log("Bar Start");
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
        float dist = Vector3.Distance(player.position, transform.position);
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
        if (rend != null && closedMaterial != null)
        {
            rend.material = closedMaterial;
        }
    }
}