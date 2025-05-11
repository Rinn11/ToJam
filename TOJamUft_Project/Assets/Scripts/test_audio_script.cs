using UnityEngine;

public class test_audio_script : MonoBehaviour
{
    public AudioManager audioManager;
    public AudioClip testClip;
    public AudioClip testClip2; // the hold clip test

    private GameObject heldAudioPlayer = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Test a audio clip by playing it.
            audioManager.playClip(testClip, 1.0f, gameObject);
        }

        // Test custom behavior of pressing down F to play a clip and keeps playing on repeat, but stops playing if F is let go.
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            if (!heldAudioPlayer) {
                heldAudioPlayer = audioManager.createAudioPlayer(gameObject);
                AudioSource heldAudioSource = heldAudioPlayer.GetComponent<AudioSource>();
                heldAudioSource.clip = testClip2;
                heldAudioSource.volume = 1.0f;
                heldAudioSource.loop = true;
                heldAudioSource.Play();
            }
        } else if (Input.GetKeyUp(KeyCode.Alpha2)) {
            if (heldAudioPlayer) {
                AudioSource heldAudioSource = heldAudioPlayer.GetComponent<AudioSource>();
                heldAudioSource.Stop();
                Destroy(heldAudioPlayer);
                heldAudioPlayer = null;
            }
        }
    }
}
