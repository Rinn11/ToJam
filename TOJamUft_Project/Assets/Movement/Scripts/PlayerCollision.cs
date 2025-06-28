/*
 * Handles collision between the player and and anything except the cop car (e.g. terrain).
 * On collision with the cop, checks their relative magnitudes. If full damage is applied, shows the end screen and other end effects.
 */

// TODO: Maybe this should use the game ending functions in EndScreenBehaviour?

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollision : MonoBehaviour
{
  [Header("UI Related Settings")]
  public GameObject textUI;

  [Header("Collision Settings")]
  [SerializeField]
  private int numberOfCollisions;
  private int currentCollisions = 0;

  [Header("IFrame Settings")]
  [SerializeField]
  private float iframeDuration; // Duration of invincibility frames (in seconds)
  [SerializeField]
  private int numberOfIframeFlashes; // Number of times to flash the player during iFrames

  [Header("Audio Settings")]
  public AudioSource crashSource;
  public AudioSource damageSource;

  float timeSinceLastCollision = 0f, lastCollisionExitTime = 0f;

  [Header("Events")]
  public UnityEvent roundOverEvent;

  // Thanks to this video for the help for the iframe implementation even though it's for 2D Unity: https://www.youtube.com/watch?v=YSzmCf_L2cE
  private IEnumerator iFrameCoroutine()
  {
    return null;
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

    if (collision.gameObject.CompareTag("CopCar"))
    {
      currentCollisions++;
      Debug.Log("Collision with cop car detected. Current collisions: " + currentCollisions);

      // Just crashing won't do. you need feedback. add visual and audio feedback here.
      crashSource.Play();

      // Use an iFrame period to give the player a chance to recover.
      StartCoroutine(iFrameCoroutine());
    }

    if (currentCollisions >= numberOfCollisions)
    {
      roundOverEvent.Invoke();
    }
  }

  void OnCollisionExit(Collision collision)
  {
    lastCollisionExitTime = Time.time;
  }
}
