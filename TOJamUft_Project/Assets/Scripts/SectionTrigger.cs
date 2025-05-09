using Unity.VisualScripting;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public TileSpawner tileManager;
    public float sectionSpawnOffset;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // Alert the Tile Manager
            tileManager.crossed(this);
        }
    }


}
