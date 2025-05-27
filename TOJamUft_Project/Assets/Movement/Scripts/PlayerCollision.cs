/*
 * Handles collision between the player and and anything except the cop car (e.g. terrain).
 * On collision with the cop, checks their relative magnitudes. If full damage is applied, shows the end screen and other end effects.
 */

// TODO: Maybe this should use the game ending functions in EndScreenBehaviour?

using UnityEngine;
using UnityEngine.Events;

public class PlayerCollision : MonoBehaviour
{

  public AudioSource crashSource;
  public AudioSource damageSource;
  public GameObject endScreenUI;
  public GameObject textUI;
  public UnityEvent roundOverEvent = new UnityEvent();

  float damage = 100f;
  float timeSinceLastCollision = 0f, lastCollisionExitTime = 0f;

    void Start()
    {
        roundOverEvent.AddListener(GameObject.FindGameObjectWithTag("FineManager").GetComponent<FineManagerBehavior>().sendScoreInvoker);
    }

    private void OnCollisionEnter(Collision collision)
  {
    if (lastCollisionExitTime != 0f)
    {
      timeSinceLastCollision = Time.time - lastCollisionExitTime;
    }
    // TODO: make damage only apply if player is hitting a car? do we want wall damage?
    // if (!collision.gameObject.CompareTag("CopCar") && timeSinceLastCollision > 0.05)
    // {
    //   float relativeSpeed = new Vector2(collision.relativeVelocity.x, collision.relativeVelocity.z).magnitude;
    //   damage = Mathf.Max(0f, damage - relativeSpeed);
    //   Debug.Log(damage);
    //   damageSource.Play();
    // }

    if (damage == 0f || collision.gameObject.CompareTag("CopCar"))
    {
      crashSource.Play();
      endScreenUI.SetActive(true);
      textUI.SetActive(false);
      roundOverEvent.Invoke();

      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      // collision.gameObject.SetActive(false);
    }
  }

  void OnCollisionExit(Collision collision)
  {
    lastCollisionExitTime = Time.time;
  }
}
