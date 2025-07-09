using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class TravelPoint : MonoBehaviour
{
    public List<TravelPoint> nextPoints;
    [HideInInspector]
    public bool triggered = false; // Flag to check if there is something already in the trigger

    private string pointName; // Name or ID of the travel point

    private void Start()
    {
        // Initialize the point name with the name of the GameObject
        pointName = gameObject.name;
    }

    public TravelPoint getSuccessor()
    {
        if (nextPoints.Count == 0) return null;

        // Randomly select a successor from the list of next points
        int randomIndex = Random.Range(0, nextPoints.Count);
        return nextPoints[randomIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        triggered = true; // Set the flag to true when an object enters the trigger

        // Check if the other object is a car agent
        CarAgent carAgent = other.GetComponent<CarAgent>();
        if (carAgent != null && carAgent.destination.pointName == pointName)
        {

            // Set the car's destination to this travel point's successor
            TravelPoint successor = getSuccessor();
            if (successor != null)
            {
                carAgent.destination = successor;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Reset the flag when the object exits the trigger
        triggered = false;
    }
}
