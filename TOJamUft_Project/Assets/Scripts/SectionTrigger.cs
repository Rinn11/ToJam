using Unity.VisualScripting;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public TileSpawner tileManager;
    public float sectionSpawnOffset;
    public GameObject tileSpawnPoint;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // Alert the Tile Manager
            tileManager.crossed(this);
            this.GetComponent<Collider>().enabled = false;
            
        }
    }


}
