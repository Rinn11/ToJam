using UnityEngine;

public class Impact : MonoBehaviour
{
    public ParticleSystem crashEffectPrefab;
    [SerializeField]
    private float smallestScale;
    [SerializeField]
    private float largestScale;

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;
        Quaternion contactRotation = Quaternion.LookRotation(collision.contacts[0].normal);
        ParticleSystem crashEffect = Instantiate(crashEffectPrefab, contactPoint, contactRotation);

        // Scale the effect based on the collision force
        float collisionForce = collision.relativeVelocity.magnitude;
        float scale = Mathf.Lerp(smallestScale, largestScale, collisionForce / 10f);
        crashEffect.transform.localScale = Vector3.one * scale;

        crashEffect.Play();
        Destroy(crashEffect.gameObject, crashEffect.main.duration);
    }
}
