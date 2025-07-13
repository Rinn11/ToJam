using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DestructibleObject : MonoBehaviour
{
    [Header("IFrame Settings")]
    public float destructibilityDuration; // Duration of invincibility frames (in seconds)
    public int numberOfIframeFlashes; // Number of times to flash the car during iFrames
    public Material invisibleMaterial;

    [Header("Respawn Settings")]
    public float respawnDelay; // The delay in seconds to spawn back the object/obstacle
    [HideInInspector]
    public bool beingDestroyed = false;

    Vector3 originalPosition;
    Quaternion originalRotation;
    Rigidbody rb;
    bool acceptCollisions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the original position and rotation of the object to make sure we can move the object back.
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // The object should always be in kinematic mode if it's not being collided with
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        acceptCollisions = true;
    }

    public IEnumerator destructionCoroutine()
    {
        rb.isKinematic = false;
        acceptCollisions = false;
        beingDestroyed = true;

        Renderer[] rends = GetComponentsInChildren<Renderer>();
        Material[] oldMaterials = new Material[rends.Length];

        for (int i = 0; i < numberOfIframeFlashes; i++)
        {
            for (int j = 0; j < rends.Length; j++)
            {
                oldMaterials[j] = rends[j].material;
                rends[j].material = invisibleMaterial;
            }
            yield return new WaitForSeconds(destructibilityDuration / (numberOfIframeFlashes * 2));

            for (int j = 0; j < rends.Length; j++)
            {
                rends[j].material = oldMaterials[j];
            }
            yield return new WaitForSeconds(destructibilityDuration / (numberOfIframeFlashes * 2));
        }

        // And reset the position and velocities
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Disable everything excluding this game object's script and do not set the activeness of the game object to false.
        pseudoDisable();

        // Wait for a duration to respawn the object
        yield return new WaitForSeconds(respawnDelay);

        // Re enable everything.
        acceptCollisions = true;
        beingDestroyed = false;
        pseudoEnable();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision contains another destructible object that is currently being destroyed.
        DestructibleObject otherDestructible = collision.gameObject.GetComponent<DestructibleObject>();

        if (acceptCollisions && (collision.gameObject.CompareTag("CopCar") || collision.gameObject.CompareTag("Player") || (otherDestructible != null && otherDestructible.beingDestroyed)))
        {
            StartCoroutine(destructionCoroutine());
        }
    }

    // Coroutines in Unity immediately halt if the object is set to inactive. so we need to disable everything except the script itself.
    void pseudoDisable()
    {
        // Start with disabling all components related to the object.
        Collider objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }

        Renderer meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        // Then deal with any children the object may have. thankfully this one is much easier than the last.
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void pseudoEnable()
    {
        // Re-enable all components related to the object except the script.
        Collider objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }

        Renderer meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        // Then deal with any children the object may have. thankfully this one is much easier than the last.
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
