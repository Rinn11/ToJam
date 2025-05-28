/*
 * Handles the collision between the player and the cop. On collision:
 * - Plays a crash sound
 * - Shows the end screen UI
 * - Stops the alcohol UI
 * - Unlocks and Shows the cursor
 * - Deactivates the other car
 */

// TODO: Maybe this should use the game ending functions in EndScreenBehaviour?

using UnityEngine;
using UnityEngine.Events;

public class CopCarCollision : MonoBehaviour
{

    public AudioSource crashSource;
    public GameObject endScreenUI;
    public GameObject alcoholUI;
    public UnityEvent roundOverEvent = new UnityEvent();

    void Start()
    {
        // Register the event to send the score when the round ends
        roundOverEvent.AddListener(GameObject.FindGameObjectWithTag("FineManager").GetComponent<FineManagerBehavior>().sendScoreInvoker);
    }

    private void OnTriggerEnter(Collider other)
    {
        // add magnitude check

    if (other.gameObject.CompareTag("Player"))
    {
      // This is cursed af lmao
      crashSource
        .Play();
      // endScreenUI.SetActive(true);
      // alcoholUI.SetActive(false);
      roundOverEvent.Invoke();

      // Cursor.lockState = CursorLockMode.None;
      // Cursor.visible = true;
      // other.gameObject.SetActive(false);
    }
  }
}
