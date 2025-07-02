/*
 * Handles collision between the player and and anything except the cop car (e.g. terrain).
 * On collision with the cop, checks their relative magnitudes. If full damage is applied, shows the end screen and other end effects.
 */

// TODO: Maybe this should use the game ending functions in EndScreenBehaviour?

using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollision : MonoBehaviour
{
    public GameObject objectToBlink;

  [Header("UI Related Settings")]
  public GameObject textUI;

  [Header("Collision Settings")]
  [SerializeField]
  private int numberOfCollisions;
  private int currentCollisions = 0;
  private bool acceptCollisions = true; // Flag to control whether collisions are accepted for certain behaviors to trigger

  [Header("IFrame Settings")]
  [SerializeField]
  private float iframeDuration; // Duration of invincibility frames (in seconds)
  [SerializeField]
  private int numberOfIframeFlashes; // Number of times to flash the player during iFrames
  [SerializeField]
  private float iframeOpacity; // Amount to change the opacity during iFrames

  [Header("Audio Settings")]
  public AudioSource crashSource;
  public AudioSource damageSource;

  float timeSinceLastCollision = 0f, lastCollisionExitTime = 0f;

  [Header("Events")]
  public UnityEvent roundOverEvent;

  // Thanks to this video for the help for the iframe implementation even though it's for 2D Unity: https://www.youtube.com/watch?v=YSzmCf_L2cE
  /* previous good code, due to the current object having several meshes I changed it to do this per object, but we should go back too the below if 
   * we ever do optimizeation
    private IEnumerator iFrameCoroutine()
  {
    acceptCollisions = false; // Disable further collisions and more behaviors during iframes

        // Grab the mesh renderer of the player to flash it
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        
        Color materialColor = renderer.material.color;

        for (int i = 0; i < numberOfIframeFlashes; i++)
        {
          materialColor.a = iframeOpacity; // Set the material color to semi-transparent
          renderer.material.color = materialColor; // Apply the color change
          yield return new WaitForSeconds(iframeDuration / (numberOfIframeFlashes * 2));
          materialColor.a = 1f; // Reset to full opacity
          renderer.material.color = materialColor; // Apply the color change
          yield return new WaitForSeconds(iframeDuration / (numberOfIframeFlashes * 2));
        }

    acceptCollisions = true; // Re-enable collisions after the iFrame duration
  }*/

    // temp i frame, check each object needed render (see above)
    private IEnumerator iFrameCoroutine()
    {
        acceptCollisions = false; // Disable further collisions and more behaviors during iframes

        for (int i = 0; i < numberOfIframeFlashes; i++)
        {
            objectToBlink.SetActive(false);
            yield return new WaitForSeconds(iframeDuration / (numberOfIframeFlashes * 2));

            objectToBlink.SetActive(true);
            yield return new WaitForSeconds(iframeDuration / (numberOfIframeFlashes * 2));
        }

        acceptCollisions = true; // Re-enable collisions after the iFrame duration
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

    if (collision.gameObject.CompareTag("CopCar") && acceptCollisions)
    {
      currentCollisions++;
      Debug.Log("Collision with cop car detected. Current collisions: " + currentCollisions);

      // Just crashing won't do. you need feedback. add visual and audio feedback here.
      crashSource.Play();

      if (currentCollisions >= numberOfCollisions)
      {
        roundOverEvent.Invoke();
        return;
      }

      // Use an iFrame period to give the player a chance to recover.
      StartCoroutine(iFrameCoroutine());
    }

  }

  void OnCollisionExit(Collision collision)
  {
    lastCollisionExitTime = Time.time;
  }

  public void resetCollisions()
  {
    currentCollisions = 0; // Reset the collision count
    acceptCollisions = true; // Re-enable collisions
  }
}
