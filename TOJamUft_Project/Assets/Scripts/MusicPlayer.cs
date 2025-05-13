using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip loopClip;

    void Update()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (!audioSource.isPlaying) {
            audioSource.clip = loopClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

}
