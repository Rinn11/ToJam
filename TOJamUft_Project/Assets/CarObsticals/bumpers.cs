using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class bumpers : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            StartCoroutine(slowTime());
        }
        //slow players speed with Ienumer to max
        

        //other.gameObject.GetComponent<Material>().SetColor("_Color", Color.blue);
    }

    private IEnumerator slowTime()
    {
        //SpawnCarsRandomlyAcrossTiles(3);
        // rantom time to spawn

        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;

    }
}
