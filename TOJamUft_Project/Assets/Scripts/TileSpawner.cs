using System;
using System.Globalization;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{

    public GameObject[] tileSections;
    public int initialTileAmount;
    public int tileSpawnPassedCount;
    public int tileDeletePassedCount;
    public int tileSpawnAmount;
    public int tileDeleteAmount;
    public float tileScale;
    public float downwardOffset; // This is to offset the next generated tile so that we can elimnate some seams on the floor despite all tiles existing on the value on the y axis.

    private int tileSpawnPassedCounter = 0;    
    private int tileDeletePassedCounter = 0;

    private void Start() {
        initialTileAmount = Math.Max(1, initialTileAmount);
        tileSpawnPassedCount = Math.Max(1, tileSpawnPassedCount);

        // You need a initial child to spawn the rest of the files in the correct position and rotation
        GameObject chosenTileSection = tileSections[UnityEngine.Random.Range(0, tileSections.Length)];
        GameObject newSection = Instantiate(chosenTileSection, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
        newSection.transform.localScale = new Vector3(tileScale, tileScale, tileScale);

        // Spawn the initial amount of tiles
        for (int i = 0; i < initialTileAmount - 1; i++) {
            chosenTileSection = tileSections[UnityEngine.Random.Range(0, tileSections.Length)];
            newSection = Instantiate(chosenTileSection, newSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.position - new Vector3(0, downwardOffset, 0), newSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.rotation, gameObject.transform);
            newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
            newSection.transform.localScale = new Vector3(tileScale, tileScale, tileScale);
        }
    }

    public void crossed() {
        // Once we cross a section trigger, we increment the counter
        tileSpawnPassedCounter++;
        tileDeletePassedCounter++;

        // Use the counter to check if we need to spawn the tileSpawnAmount of tiles
        if (tileSpawnPassedCounter >= tileSpawnPassedCount) {
            tileSpawnPassedCounter = 0;
            
            // Grab the most recent tile section (usually the last child of the tile manager)
            GameObject lastTileSection = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;
            GameObject chosenTileSection = tileSections[UnityEngine.Random.Range(0, tileSections.Length)];
            GameObject newSection = Instantiate(chosenTileSection, lastTileSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.position  - new Vector3(0, downwardOffset, 0), lastTileSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.rotation, gameObject.transform);
            newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
            newSection.transform.localScale = new Vector3(tileScale, tileScale, tileScale);
            for (int i = 0; i < tileSpawnAmount - 1; i++) {
                // Utilize the object's forward vector to make tile spawning invariant to the rotation it's facing.
                chosenTileSection = tileSections[UnityEngine.Random.Range(0, tileSections.Length)];
                newSection = Instantiate(chosenTileSection, newSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.position  - new Vector3(0, downwardOffset, 0), newSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.rotation, gameObject.transform);
                newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
                newSection.transform.localScale = new Vector3(tileScale, tileScale, tileScale);
            }
        } 

        if (tileDeletePassedCounter >= tileDeletePassedCount) {
            tileDeletePassedCounter = 0;
            
            // Delete the tileDeleteAmount of tiles
            for (int i = 0; i < tileDeleteAmount; i++) {
                // Grab the first tile section (usually the first child of the tile manager)
                GameObject firstTileSection = gameObject.transform.GetChild(0).gameObject;
                Destroy(firstTileSection);
            }

        }
    }
}
