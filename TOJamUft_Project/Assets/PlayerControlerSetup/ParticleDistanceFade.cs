using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleXFade : MonoBehaviour
{
    public Transform referenceObject; // The GameObject to fade from
    public float maxDistance = 50f;   // Distance at which alpha becomes 0

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        if (referenceObject == null) return;

        int count = ps.GetParticles(particles);
        //float refX = referenceObject.position.x;
        float distence = Vector3.Distance(referenceObject.position, transform.position);
        Debug.Log(distence + " dis");

        if (distence > maxDistance)
        {
            ps.Stop();
            return;
        }
        else
        {
            ps.Play();
        }
    }
}
