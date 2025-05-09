using System.Globalization;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public SectionTrigger something;

    public GameObject tileSection;
    public int tilePassedCount;
    public int tileSpawnAmount;

    private int tilePassedCounter = 0;

    public void crossed(SectionTrigger sec) {
        // Once we cross a section trigger, we increment the coutner
        tilePassedCounter++;

        // Use the counter to check if we need to spawn the tileSpawnAmount of tiles
        if (tilePassedCounter >= tilePassedCount) {
            GameObject newSection = Instantiate(tileSection, sec.gameObject.transform.position + sec.gameObject.transform.forward * sec.sectionSpawnOffset, Quaternion.identity, transform);
            
            for (int i = 0; i < tileSpawnAmount - 1; i++) {
                // Utilize the object's forward vector to make tile spawning invariant to the rotation it's facing.
                newSection = Instantiate(tileSection, newSection.gameObject.transform.position + sec.gameObject.transform.forward * sec.sectionSpawnOffset, Quaternion.identity, transform);
            }
        } 
    }
}
