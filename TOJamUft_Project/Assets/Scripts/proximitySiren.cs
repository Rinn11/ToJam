using UnityEngine;

public class proximitySiren : MonoBehaviour
{
    public AudioSource sirenSource;
    public GameObject player;
    public GameObject cop;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get distance between cop and player
        float distance = Vector3.Distance(player.transform.position, cop.transform.position);
        // IF distance is less than 10, play siren
        if (distance < 100 && Time.timeScale != 0)
        {
            if (!sirenSource.isPlaying)
            {
                sirenSource.Play();
            }
        }
        else
        {
            if (sirenSource.isPlaying)
            {
                sirenSource.Stop();
            }
        }

        if (sirenSource.isPlaying)
        {
            // adjust volume based on distance
            float volume = Mathf.Clamp01(1 - (distance / 25));
            sirenSource.volume = volume;
        }
    }
}
