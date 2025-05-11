using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        // Destroy itself once the audio is done playing.
        if (!audioSource.isPlaying) {
            Destroy(this.gameObject);
        }
    }
}
