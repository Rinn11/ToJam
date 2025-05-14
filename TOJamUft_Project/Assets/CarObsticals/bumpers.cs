using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class bumpers : MonoBehaviour
{
    public AudioSource slowPlay;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            slowPlay.Play();
            StartCoroutine(slowTime());
        }
        //slow players speed with Ienumer to max
        

        //other.gameObject.GetComponent<Material>().SetColor("_Color", Color.blue);
    }



    private IEnumerator slowTime()
    {
        //SpawnCarsRandomlyAcrossTiles(3);
        // rantom time to spawn

        Time.timeScale = 0.1f;
        Time.fixedDeltaTime /= 10f;
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 1;
        Time.fixedDeltaTime *= 10f;

    }
}
