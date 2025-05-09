using UnityEngine;

public class Heads : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // add magnitude check

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
