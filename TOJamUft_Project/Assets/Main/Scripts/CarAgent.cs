using UnityEngine;

public class CarAgent : MonoBehaviour
{
    public TravelPoint destination; // The destination point the car should move towards
    public float speed; // Speed of the car
    public float yOffset; // In case the direction needs to be adjusted.

    private void Update()
    {
        if (destination != null)
        {
            // Move towards the destination point
            Vector3 direction = (destination.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Rotate the car to face the destination
            if (direction != Vector3.zero)
            {
                // For the rotation only change the y-axis
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
            }
        }
    }
}
