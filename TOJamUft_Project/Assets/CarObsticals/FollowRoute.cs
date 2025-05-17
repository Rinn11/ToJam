using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowRoute : MonoBehaviour
{
    [SerializeField] private float carSpeed = 10f;

    public List<Transform> totalRoads = new List<Transform>();

    private int route;
    private Transform targetPosition;
    
    private int currentRoadPosition=0;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void setTarget()
    {
        if (route < 2)
        {
            currentRoadPosition++;
        }
        else
        {
            currentRoadPosition--;
        }

        if (currentRoadPosition >= totalRoads.Count || currentRoadPosition <= 0) { Destroy(this.gameObject); }

        targetPosition = totalRoads[currentRoadPosition].GetChild(route);

        transform.LookAt(targetPosition);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(this.gameObject.transform.position, targetPosition.position, Time.fixedDeltaTime * carSpeed));
    }

    public void setRoute(int currentRoute, Transform roads)
    {
        route = currentRoute;

        for (int i = 0; i < roads.childCount; i++)
        {
            Debug.Log(totalRoads);
            totalRoads.Add(roads.GetChild(i));
        }

        if (route < 2)
        {
            currentRoadPosition = 0;
        }
        else
        {
            currentRoadPosition = roads.childCount;
        }

        setTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ControlPoint"))
        {
            setTarget();

        }
    }
}
