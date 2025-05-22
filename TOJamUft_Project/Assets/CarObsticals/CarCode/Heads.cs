using UnityEngine;

public class Heads : MonoBehaviour
{

  public AudioSource crashSource;
  public GameObject endScreenUI;
  // public GameObject alcoholUI;

  private void OnTriggerEnter(Collider other)
  {
    // add magnitude check

    if (other.gameObject.CompareTag("Player"))
    {
      // This is cursed af lmao
      crashSource.Play();
      endScreenUI.SetActive(true);
      // alcoholUI.SetActive(false);

      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      other.gameObject.SetActive(false);
    }
  }
}
