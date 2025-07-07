using UnityEngine;

public class CarAgent : MonoBehaviour
{
    public TravelPoint destination; // The destination point the car should move towards
    public float speed; // Speed of the car

    private void Update()
    {
        if (destination != null)
        {
            // Move towards the destination point
            Vector3 direction = (destination.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
