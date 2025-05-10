using System.Globalization;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{

    public GameObject[] tileSections;
    public int tileSpawnPassedCount;
    public int tileDeletePassedCount;
    public int tileSpawnAmount;
    public int tileDeleteAmount;

    private int tileSpawnPassedCounter = 0;    
    private int tileDeletePassedCounter = 0;

    public void crossed() {
        // Once we cross a section trigger, we increment the counter
        tileSpawnPassedCounter++;
        tileDeletePassedCounter++;

        // Use the counter to check if we need to spawn the tileSpawnAmount of tiles
        if (tileSpawnPassedCounter >= tileSpawnPassedCount) {
            tileSpawnPassedCounter = 0;
            
            // Grab the most recent tile section (usually the last child of the tile manager)
            GameObject lastTileSection = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;
            GameObject chosenTileSection = tileSections[Random.Range(0, tileSections.Length)];
            GameObject newSection = Instantiate(chosenTileSection, lastTileSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.position, lastTileSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.rotation, gameObject.transform);
            newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
            for (int i = 0; i < tileSpawnAmount - 1; i++) {
                // Utilize the object's forward vector to make tile spawning invariant to the rotation it's facing.
                chosenTileSection = tileSections[Random.Range(0, tileSections.Length)];
                newSection = Instantiate(chosenTileSection, newSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.position, newSection.GetComponentInChildren<SectionTrigger>().tileEndPoint.transform.rotation, gameObject.transform);
                newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
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
