/*
 * This plays sounds for the player off events generated from sources such as the car's controls.
 */

using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource engineAudio;
    public AudioSource brakeAudio;
    public AudioSource idleAudio;

    public void PlayEngine()
    {
        if (!engineAudio.isPlaying)
        {
            engineAudio.Play();
        }
    }

    public void PlayBrake()
    {
        if (!brakeAudio.isPlaying)
        {
            brakeAudio.Play();
        }
    }

    public void PlayIdle()
    {
        if (!idleAudio.isPlaying)
        {
            idleAudio.Play();
        }
    }
}
