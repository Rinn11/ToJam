using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public Vector3 player1StartPosition;
    public Vector3 player2StartPosition;
    
    // angle of the players when they are reset
    public float player1StartAngle = 0f;
    public float player2StartAngle = 0f;

    public void ResetPlayerPositions()  // can be invoked by RoundManagerBehavior.cs or any other script that needs to reset player positions
    {
        Debug.Log("Resetting player positions...");
        if (player1 != null)
        {
            // set velocity and angular velocity to zero
            Rigidbody rb1 = player1.GetComponent<Rigidbody>();
            rb1.MovePosition(player1StartPosition);
            rb1.MoveRotation(Quaternion.Euler(0, player1StartAngle, 0));
            
            if (rb1 != null)
            {
                rb1.linearVelocity = Vector3.zero;
                rb1.angularVelocity = Vector3.zero;
            }
            else
            {
                Debug.LogError("Rigidbody component not found on Player 1 GameObject. Please add one.");
            }
        }
        else
        {
            Debug.LogError("Player 1 GameObject is not assigned.");
        }

        if (player2 != null)
        {
            // set velocity and angular velocity to zero
            Rigidbody rb2 = player2.GetComponent<Rigidbody>();
            rb2.MovePosition(player2StartPosition);
            rb2.MoveRotation(Quaternion.Euler(0, player2StartAngle, 0));
            if (rb2 != null)
            {
                rb2.linearVelocity = Vector3.zero;
                rb2.angularVelocity = Vector3.zero;
            }
            else
            {
                Debug.LogError("Rigidbody component not found on Player 2 GameObject. Please add one.");
            }
        }
        else
        {
            Debug.LogError("Player 2 GameObject is not assigned.");
        }
    }
}