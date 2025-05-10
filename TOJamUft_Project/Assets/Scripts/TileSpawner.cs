using System.Globalization;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    // public SectionTrigger something;

    public GameObject tileSection;
    public int tilePassedCount;
    public int tileSpawnAmount;

    private int tilePassedCounter = 0;

    public void crossed(SectionTrigger sec) {
        // Once we cross a section trigger, we increment the counter
        tilePassedCounter++;

        // Use the counter to check if we need to spawn the tileSpawnAmount of tiles
        if (tilePassedCounter >= tilePassedCount) {
            tilePassedCounter = 0;
            // Grab the most recent tile section (usually the last child of the tile manager)
            GameObject lastTileSection = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;
            GameObject newSection = Instantiate(tileSection, lastTileSection.GetComponentInChildren<SectionTrigger>().tileSpawnPoint.transform.position, lastTileSection.transform.rotation, gameObject.transform);
            newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
            for (int i = 0; i < tileSpawnAmount - 1; i++) {
                // Utilize the object's forward vector to make tile spawning invariant to the rotation it's facing.
                newSection = Instantiate(tileSection, newSection.GetComponentInChildren<SectionTrigger>().tileSpawnPoint.transform.position, newSection.transform.rotation, gameObject.transform);
                newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
            }
        } 
    }
}
