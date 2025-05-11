using UnityEngine;

public class Heads : MonoBehaviour
{

    public AudioSource crashSource;
    public AudioSource arrestSource;
    public GameObject endScreenUI;

    private void OnTriggerEnter(Collider other)
    {
        // add magnitude check

        if (other.gameObject.CompareTag("Player"))
        {
            crashSource.Play();
            arrestSource.Play();
            endScreenUI.SetActive(true);
            other.gameObject.SetActive(false);
        }
    }
}
