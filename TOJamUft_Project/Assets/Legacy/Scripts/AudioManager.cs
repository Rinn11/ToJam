// TODO: Determine if this is redundant (AudioManager, AudioPlayer, MusicPlayer)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject audioPlayer;
    public float masterVolume;

    public void playClip(AudioClip clip, float volume, GameObject requestedObject, bool loop = false) { 
        // Create a new audio player object, which will contain a new AudioSource.
        GameObject apCopy = Instantiate(audioPlayer, requestedObject.transform.position, requestedObject.transform.rotation, gameObject.transform);
        AudioSource apSource = apCopy.GetComponent<AudioSource>();

        // Place the clip in the audio player's source, and play it.
        masterVolume = Mathf.Clamp(masterVolume, 0.0f, 1.0f);
        apSource.clip = clip;
        apSource.volume = volume * masterVolume;
        apSource.loop = loop;
        apSource.Play();
    }

    public GameObject createAudioPlayer(GameObject requestedObject) {
        // If needed, create a new audio player object and return it to the requested object for custom use or behavior.
        return Instantiate(audioPlayer, requestedObject.transform.position, requestedObject.transform.rotation, gameObject.transform);
    }
}
