/*
 * Handles starting the game. when the player presses space:
 * - Unpauses the game
 * - Hides the title screen
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject ingameUI;
    public AudioSource[] audioSources;

    public RawImage player1Img, player2Img;
    private bool player1Active, player2Active;

    [Header("Title Screen Camera Settings")]
    public GameObject titleScreenCamera;
    public float rotationSpeed;
    private Vector3 rotationVector;

    [Header("Display2 Settings")]
    public GameObject titleScreenUI2;
    public GameObject titleScreenCamera2;
    public RawImage player1Img2, player2Img2;
    private bool isDualMonitor = false;

    [Header("Events")]
    public UnityEvent StartGameEvent;

    private bool canMove;
    public GameObject copCam;
    public GameObject ddCam;

    void Start()
    {
        copCam.SetActive(false); ddCam.SetActive(false);

        ingameUI.SetActive(false);
        titleScreen.SetActive(true);

        audioSources = GetComponents<AudioSource>();

        // Generate a random vector for camera rotations
        rotationVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rotationVector.Normalize();

        int displayCount = Display.displays.Length;

        // Activate all displays
        for (int i = 1; i < displayCount; i++)
            Display.displays[i].Activate();
        isDualMonitor = displayCount >= 2;

        canMove = false;

        if (isDualMonitor)
        {
            titleScreenUI2.SetActive(true);
            titleScreenCamera2.SetActive(true);
        }
    }


    void Update()
    {
        //if (Time.timeScale == 0)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        // {
        //        Time.timeScale = 1;
        //        audioSources[0].Play();         // TODO: audioSources doesn't seem to be used?
        //        titleScreen.SetActive(false);
        //        ingameUI.SetActive(true);
        //    }
        //}

        // Add perturbation to the rotation vector
        rotationVector += new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
        rotationVector.Normalize(); // Normalize to keep the direction consistent

        // Rotate the title screen camera in the direction of the random vector
        titleScreenCamera.transform.Rotate(rotationVector, rotationSpeed * Time.deltaTime);
        if (isDualMonitor)
        {
            // Rotate the second camera in the same way
            titleScreenCamera2.transform.Rotate(rotationVector, rotationSpeed * Time.deltaTime);
        }
    }

    public void ReadyGame()
    {
        if (!player1Active)
        {
            player1Active = true;
            player1Img.color = Color.green;
            player1Img2.color = Color.green;
        }
        else if (!player2Active)
        {
            player2Active = true;
            player2Img.color = Color.green;
            player2Img2.color = Color.green;
        }

        if (player1Active && player2Active)
        {
            Time.timeScale = 1;
            audioSources[0].Play();         // TODO: audioSources doesn't seem to be used?
            titleScreen.SetActive(false);
            ingameUI.SetActive(true);
            StartGameEvent.Invoke();

            copCam.SetActive(true); ddCam.SetActive(true);

            canMove = true;
        }
    }

    public bool GetCanMove()
    {

        return canMove;
    }
}