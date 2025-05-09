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
        // Once we cross a section trigger, we increment the coutner
        tilePassedCounter++;

        // Use the counter to check if we need to spawn the tileSpawnAmount of tiles
        if (tilePassedCounter >= tilePassedCount) {
            tilePassedCounter = 0;
            GameObject newSection = Instantiate(tileSection, sec.tileSpawnPoint.transform.position, sec.transform.rotation, gameObject.transform);
            newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
            for (int i = 0; i < tileSpawnAmount - 1; i++) {
                // Utilize the object's forward vector to make tile spawning invariant to the rotation it's facing.
                newSection = Instantiate(tileSection, newSection.GetComponentInChildren<SectionTrigger>().tileSpawnPoint.transform.position, sec.transform.rotation, gameObject.transform);
                newSection.GetComponentInChildren<SectionTrigger>().tileManager = this;
            }
        } 
    }
}
