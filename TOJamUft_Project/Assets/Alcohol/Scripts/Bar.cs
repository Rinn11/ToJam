using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class Bar : MonoBehaviour
{
    private GameObject openModel;
    private GameObject closedModel;

    public RawImage mapIcon;

    public bool IsOpen { get; private set; }
    private bool isVisited = false;
    
    public GameObject player;
    private Transform playerTransform;
    public float activationRadius = 70.0f;
    private float activationRadiusSqr;
    
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
    }

    void Update()
    {
        // get x and z distance only
        Vector3 diff = playerTransform.position - mapIcon.transform.position;
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
        mapIcon.color = Color.yellowNice;
    }

    internal void SetClosed()
    {
        Debug.Log($"Bar {gameObject.name} set closed");
        IsOpen = false;
        isVisited = false;
        openModel.SetActive(false);
        closedModel.SetActive(true);
        mapIcon.color = Color.grey;
    }
}