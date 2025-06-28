using UnityEngine;

public class Impact : MonoBehaviour
{
    public ParticleSystem crashEffectPrefab;
    [SerializeField]
    private float smallestScale;
    [SerializeField]
    private float largestScale;

    [SerializeField]
    private float smallestCollisionForce;
    [SerializeField]
    private float largestCollisionForce;

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;
        Quaternion contactRotation = Quaternion.LookRotation(collision.contacts[0].normal);
        ParticleSystem crashEffect = Instantiate(crashEffectPrefab, contactPoint, contactRotation);

        // Get the collision force
        float collisionForce = collision.relativeVelocity.magnitude;
        Debug.Log($"Collision Force: {collisionForce}");

        // Normalize the collision force so it works correctly
        float normalizedForce = Mathf.InverseLerp(smallestCollisionForce, largestCollisionForce, collisionForce);
        
        // Clamp the normalized force to ensure it stays within bounds
        normalizedForce = Mathf.Clamp01(normalizedForce);

        // Scale the effect based on the collision force
        float scale = Mathf.Lerp(smallestScale, largestScale, normalizedForce);
        crashEffect.transform.localScale = Vector3.one * scale;        

        crashEffect.Play();
        Destroy(crashEffect.gameObject, crashEffect.main.duration);
    }
}
