using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


// RoundManager will be utilizing the event system to manage game rounds. It will have methods to start, end, and manage states of the round.
// We define 1 game as 2 rounds, each round will end with a player swap once the player that is driving the cop car catches the other player.
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    public int numberOfGames;
    public float scoreBoardShowDelay;


    private int currentRound = 0;
    private int currentGame = 0;
    private List<float> p1Scores = new List<float>();
    private List<float> p2Scores = new List<float>();
    private bool isP1Driving = true; // This will track which player is currently driving the drunk driver car.

    public UnityEvent SwapEvent = new UnityEvent();
    public UnityEvent NewRoundEvent = new UnityEvent();
    public UnityEvent showScoreBoardEvent = new UnityEvent();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Checkout the scores
        Debug.Log($"RoundManager initialized. Current round: {currentRound}, Current game: {currentGame}");
        Debug.Log($"Player {(isP1Driving ? '1' : '2')} is currently driving.");
    }

    public void startRound()
    {
        // Check which player is currently driving and swap controls.
        SwapEvent.Invoke();
        NewRoundEvent.Invoke();
        Debug.Log($"Cleaning up, new round will start soon...");
        
    }

    // This method will be called when a round ends.
    public void endRound(float score)
    {
        Debug.Log($"Round ended with score: {score}. Current round: {currentRound}, Current game: {currentGame}, Next Round: {currentRound + 1}");
        currentRound++;
        if (currentRound % 2 == 0)
        {
            p2Scores.Add(score);

            if (p2Scores[currentGame] > p1Scores[currentGame])
            {
                Debug.Log("Player 2 wins this game!");
            }
            else if (p2Scores[currentGame] < p1Scores[currentGame])
            {
                Debug.Log("Player 1 wins this game!");
            }
            else
            {
                Debug.Log("It's a tie for this game!");
            }

            currentGame++;
        }
        else
        {
            p1Scores.Add(score);
            isP1Driving = !isP1Driving;

            // TODO: You can add logic here to make some UI show up that explains the round is over and the players are switching roles.
            // Or for testing, don't execute the next set of lines for like 5 seconds.
            

            showScoreBoardEvent.Invoke();
            Invoke(nameof(startRound), scoreBoardShowDelay);            
        }

    }

}
