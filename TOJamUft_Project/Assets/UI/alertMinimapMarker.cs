using UnityEngine;

public class alertMinimapMarker : MonoBehaviour
{
    private float alertTime = 0.0f;
    public GameObject minimapIcon;
    
    
    // Update is called once per frame
    void Update()
    {
        if (alertTime > 0.0f)
        {
            alertTime -= Time.deltaTime; // Decrease the alert time
            if (alertTime <= 0.0f)
            {
                alertTime = 0.0f; // Ensure it does not go negative
                minimapIcon.SetActive(false); // Deactivate the minimap marker when alert time is over
            }
        }
        else
        {
            minimapIcon.SetActive(false); // Ensure the marker is inactive when not alerting
        }
    }

    public void RecieveAlert(float duration, bool _override = false) // note this always  overrides current alert time, so it may reduce time
    {
        if (!_override && alertTime > 0.0f)
        {
            return; // If not overriding and already alerting, do nothing
        }
        alertTime = duration; // Set the alert time to the specified duration
        minimapIcon.SetActive(true); // Activate the minimap marker
    }
}
