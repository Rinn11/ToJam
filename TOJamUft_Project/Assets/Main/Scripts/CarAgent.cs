using System.Collections;
using UnityEngine;

public class CarAgent : MonoBehaviour
{
    public TravelPoint destination; // The destination point the car should move towards
    public float speed; // Speed of the car

    [Header("IFrame Settings")]
    public GameObject visibleBody; // The full body of the car
    public GameObject invisibleBody; // The transparent body of the car used for iframes
    public float iframeDuration; // Duration of invincibility frames (in seconds)
    public int numberOfIframeFlashes; // Number of times to flash the car during iFrames

    private bool acceptCollisions = true;

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

    private IEnumerator iFrameCoroutine()
    {
        for (int i = 0; i < numberOfIframeFlashes; i++)
        {
            visibleBody.SetActive(false);
            invisibleBody.SetActive(true);
            yield return new WaitForSeconds(iframeDuration / (numberOfIframeFlashes * 2));

            visibleBody.SetActive(true);
            invisibleBody.SetActive(false);
            yield return new WaitForSeconds(iframeDuration / (numberOfIframeFlashes * 2));
        }
        Destroy(gameObject); // Destroy the car after the iFrame duration
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check the tag of the collided object
        if (acceptCollisions && (collision.gameObject.CompareTag("CopCar") || collision.gameObject.CompareTag("Player")))
        {
            acceptCollisions = false; // Disable further collisions to prevent multiple triggers

            // If the car collides with either players set the destination to null.
            destination = null;

            // Then you can now do whatever you want, as the car is no longer moving and is like a prop.
            StartCoroutine(iFrameCoroutine());
        }
    }
}
