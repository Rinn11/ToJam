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
    
    public void increaseCollisionFine(int amount, bool alert = true) 
    { 
        if (alert) 
        { 
            alertManager?.RecieveAlert(1.0f, true); 
        } 
        roundManager?.CollisionFine(amount); // notify the round manager that a collision fine has occurred
        Debug.Log("Collision fine increased!");
    }
}
