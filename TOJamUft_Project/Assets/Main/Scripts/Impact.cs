using UnityEngine;

public class Impact : MonoBehaviour
{
    public ParticleSystem crashEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;
        Quaternion contactRotation = Quaternion.LookRotation(collision.contacts[0].normal);
        ParticleSystem crashEffect = Instantiate(crashEffectPrefab, contactPoint, contactRotation);
        crashEffect.Play();
        Destroy(crashEffect.gameObject, crashEffect.main.duration);        
    }
}
