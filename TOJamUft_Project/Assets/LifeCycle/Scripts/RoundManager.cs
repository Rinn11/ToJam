using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public class WinnerEvent : UnityEvent<int> { };
[Serializable]
public class GameDataEvent: UnityEvent<float, float, float, float> { };


// RoundManager will be utilizing the event system to manage game rounds. It will have methods to start, end, and manage states of the round.
// We define 1 game as 2 rounds, each round will end with a player swap once the player that is driving the cop car catches the other player.
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    public int numberOfGames;
    public float scoreBoardShowDelay;

    private int currentRound = 0;
    private int currentGame = 0;

    // These lists will hold the scores for each player in each game.
    private List<float> p1DrunkDriverScores = new List<float>();
    private List<float> p2DrunkDriverScores = new List<float>();

    // These lists will hold the times for each player took to catch the drunk driver.
    private List<float> p1PoliceCarScores = new List<float>();
    private List<float> p2PoliceCarScores = new List<float>();

    private bool isP1Driving = true; // This will track which player is currently driving the drunk driver car.
    public PlayerSwapEventSender playerSwapEvent;

    // Let's opt for this set up so that we can easily access the both score metrics instead of using events all the time.
    public FineManagerBehavior fineManager;
    public RoundTimer roundTimer;

    public bool toggleIsP1Driving()
    {
        isP1Driving = !isP1Driving;
        Debug.Log($"Toggled driving status. Player 1 driving: {isP1Driving}");
        return isP1Driving;
    }

    [Header("Events")]
    public UnityEvent NewRoundEvent = new UnityEvent();
    public UnityEvent showScoreBoardEvent = new UnityEvent();
    public UnityEvent hideScoreBoardEvent = new UnityEvent();
    public UnityEvent showEndScreenEvent = new UnityEvent();
    public WinnerEvent winnerEvent;
    public GameDataEvent sendGameDataEvent;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Checkout the scores
        Debug.Log($"RoundManager initialized. Current round: {currentRound}, Current game: {currentGame}");
        Debug.Log($"Player {(isP1Driving ? '1' : '2')} is currently driving.");
    }

    public void startRound()
    {
        // Check which player is currently driving and swap controls and camera positions & UI by sending this event
        playerSwapEvent.Trigger(isP1Driving);
        FindFirstObjectByType<PlayerJoinManager>().SwapPlayers();

        NewRoundEvent.Invoke();
        Debug.Log($"Cleaning up, new round will start soon...");

    }

    // This method will be called when a round ends.
    public IEnumerator EndRound()
    {
        float score = fineManager.fine; // Get the score from the FineManager
        float time = roundTimer.elapsedTime; // Get the time from the RoundTimer
        
        fineManager.sendScoreInvoker();
        roundTimer.sendTimeInvoker();

        currentRound++;
        if (currentRound % 2 == 0)
        {
            p1PoliceCarScores.Add(time);
            p2DrunkDriverScores.Add(score);

            if (p2DrunkDriverScores[currentGame] > p1DrunkDriverScores[currentGame])
            {
                Debug.Log("Player 2 wins this game!");
                winnerEvent.Invoke(1); // Invoke the winner event with Player 2 as the winner
            }
            else if (p2DrunkDriverScores[currentGame] < p1DrunkDriverScores[currentGame])
            {
                Debug.Log("Player 1 wins this game!");
                winnerEvent.Invoke(0); // Invoke the winner event with Player 1 as the winner
            }
            else
            {
                Debug.Log("It's a tie for this game!");
                winnerEvent.Invoke(2); // Invoke the winner event with 2 for a tie I guess?
            }

            // Send the game data to the UI or any other component that needs it.
            sendGameDataEvent.Invoke(
                p1DrunkDriverScores[currentGame],
                p2DrunkDriverScores[currentGame],
                p1PoliceCarScores[currentGame],
                p2PoliceCarScores[currentGame]
            );

            // Show the end screen UI after the round ends.
            showEndScreenEvent.Invoke();

            // Then stop time so players can see the board.
            Time.timeScale = 0f;
            currentGame++;
        }
        else
        {
            p1DrunkDriverScores.Add(score);
            p2PoliceCarScores.Add(time);
            toggleIsP1Driving();

            // TODO: Then you can add logic here to make some UI show up that explains the round is over and the players are switching roles.
            // Or for testing, don't execute the next set of lines for like 5 seconds.
            showScoreBoardEvent.Invoke();

            // TODO: Add logic here to stop the game. Whether that's reloading the scene, or setting the time scale to 0. but there are a few issues with either approach.
            //  - Reloading the scene works, but using DontDestroyOnLoad on this RoundManager object will not show up in the DontDestroyOnLoad portion of the scene.
            //  - Setting the time scale to 0 will stop the game, but it will not allow delays to work properly, so the event system will not work as expected.
            Time.timeScale = 0f; // Stop the game time, so that the player cannot move.

            yield return new WaitForSecondsRealtime(scoreBoardShowDelay);
            Time.timeScale = 1f; // Resume the game time.
            hideScoreBoardEvent.Invoke();
            startRound();
        }
        Debug.Log("Coroutine done, round ended.");
    }

    public void runEndRoundCoroutine()
    {
        // This method is a placeholder for running a coroutine to end the round.
        // It can be used to delay the end of the round or perform any other actions before ending the round.
        Debug.Log("Running end round coroutine...");
        StartCoroutine(EndRound());
    }

}
