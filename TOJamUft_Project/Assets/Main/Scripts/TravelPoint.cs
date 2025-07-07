using System.Collections.Generic;
using UnityEngine;

public class TravelPoint : MonoBehaviour
{
    public List<TravelPoint> nextPoints;

    public TravelPoint getSuccessor()
    {
        if (nextPoints.Count == 0) return null;

        // Randomly select a successor from the list of next points
        int randomIndex = Random.Range(0, nextPoints.Count);
        return nextPoints[randomIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object is a car agent
        CarAgent carAgent = other.GetComponent<CarAgent>();
        if (carAgent != null)
        {
            // Set the car's destination to this travel point's successor
            TravelPoint successor = getSuccessor();
            if (successor != null)
            {
                carAgent.destination = successor;
            }
        }  
    }
}
