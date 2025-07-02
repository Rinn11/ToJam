/*
 * Handles starting the game. when the player presses space:
 * - Unpauses the game
 * - Hides the title screen
 */

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject ingameUI;
    public AudioSource[] audioSources;

    public Image player1Img, player2Img;
    private bool player1Active, player2Active;
        
    void Start()
    {
        Time.timeScale = 0;
        
        ingameUI.SetActive(false);
        titleScreen.SetActive(true);
        
        audioSources = GetComponents<AudioSource>();
    }

    /*
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
    } */

    public void ReadyGame()
    {
        if (!player1Active)
        {
            player1Active = true;
            player1Img.color = Color.green;
        } else if (!player2Active)
        {
            player2Active = true;
            player2Img.color = Color.green;
        }

        if (player1Active && player2Active)
        {
            Time.timeScale = 1;
            audioSources[0].Play();         // TODO: audioSources doesn't seem to be used?
            titleScreen.SetActive(false);
            ingameUI.SetActive(true);
        }
    }
}