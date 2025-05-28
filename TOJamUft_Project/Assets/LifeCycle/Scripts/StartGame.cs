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
    public GameObject ingameUI;
    public AudioSource[] audioSources;
        
    void Start()
    {
        Time.timeScale = 0;
        
        ingameUI.SetActive(false);
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
                ingameUI.SetActive(true);
            }
        }
    }
}