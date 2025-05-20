/*
 * Handles starting the game. when the player presses space:
 * - Unpauses the game
 * - Hides the title screen
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class StartGame : MonoBehaviour
{
    public GameObject titleScreen;
    public AudioSource[] audioSources;
        
    void Start()
    {
        Time.timeScale = 0;
        
        titleScreen = GameObject.Find("TitleScreenUI");
        titleScreen.SetActive(true);
        
        audioSources = GetComponents<AudioSource>();
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1;
                audioSources[0].Play();         // TODO: audioSources doesn't seem to be used?
                titleScreen.SetActive(false);
            }
        }
    }
}