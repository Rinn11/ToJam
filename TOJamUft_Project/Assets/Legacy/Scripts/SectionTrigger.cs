/*
 * This script causes its object to ignore collisions with the player
 */

// TODO: Determine if we still use this

using Unity.VisualScripting;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public TileSpawner tileManager;
    public float sectionSpawnOffset;
    public GameObject tileEndPoint;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // Alert the Tile Manager
            tileManager.crossed();
            this.GetComponent<Collider>().enabled = false;
            
        }
    }


}
