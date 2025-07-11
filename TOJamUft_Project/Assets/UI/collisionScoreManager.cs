using UnityEngine;

public class collisionScoreManager : MonoBehaviour
{   
    public GameObject alertMinimapIcon;
    private alertMinimapMarker alertManager;
    public RoundManager roundManager;
    
    void Awake()
    {
       //
       if (alertMinimapIcon == null)
       {
           Debug.LogError("alertMinimapIcon not found!");
       }
       else
       {
           alertManager = alertMinimapIcon.GetComponent<alertMinimapMarker>();
       }
    }
    
    public void increaseCollisionFine()
    { 
        alertManager?.RecieveAlert(1.0f, false);
        roundManager?.CollisionFine(); // notify the round manager that a collision fine has occurred
        Debug.Log("Collision fine increased!");
    }
}
