using UnityEngine;
using System.Collections.Generic;

// RoundManager will be utilizing the event system to manage game rounds. It will have methods to start, end, and manage states of the round.
// We define 1 game as 2 rounds, each round will end with a player swap once the player that is driving the cop car catches the other player.
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    public int numberOfGames;
    private int currentRound = 0;
    private int currentGame = 0;
    private List<float> p1Scores = new List<float>();
    private List<float> p2Scores = new List<float>();
    

    // Awake() makes sure the scores kept here persist across rounds after reloading the scene.
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene reloads
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void endRound(float score)
    {
        currentRound++;
        if (currentRound % 2 == 0)
        {
            p2Scores.Add(score);
            currentGame++;
            
        }
        else
        {
            p1Scores.Add(score);
        }

    }

}
