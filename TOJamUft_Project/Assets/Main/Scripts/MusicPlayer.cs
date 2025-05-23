/*
 * Plays the AudioClip loopClip, on Audiosource audioSource.
 * Change audioSource to change how it is played.
*/

// TODO: Determine if this is redundant (AudioManager, AudioPlayer, MusicPlayer)

using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip loopClip;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = loopClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
