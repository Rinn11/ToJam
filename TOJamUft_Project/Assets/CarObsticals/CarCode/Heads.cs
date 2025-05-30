using UnityEngine;
using UnityEngine.Events;


public class Heads : MonoBehaviour
{

  public AudioSource crashSource;
  public GameObject endScreenUI;
  // public GameObject alcoholUI;

  public UnityEvent roundOverEvent = new UnityEvent();

  void Start()
  {
    roundOverEvent.AddListener(GameObject.FindGameObjectWithTag("FineManager").GetComponent<FineManagerBehavior>().sendScoreInvoker);
  }

  private void OnTriggerEnter(Collider other)
  {
    // add magnitude check

    if (other.gameObject.CompareTag("Player"))
    {
      // Use a signal to end the round
      crashSource.Play();
      endScreenUI.SetActive(true);
      roundOverEvent.Invoke();
      // alcoholUI.SetActive(false);

      // Cursor.lockState = CursorLockMode.None;
      // Cursor.visible = true;
      // other.gameObject.SetActive(false);
    }
  }
}
