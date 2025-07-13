using UnityEngine;

public class alertMinimapMarker : MonoBehaviour
{
    private float alertTime = 0.0f;
    public GameObject minimapIcon;
    public GameObject minimapPingField;
    
    
    // Update is called once per frame
    void Update()
    {
        if (alertTime > 0.0f)
        {
            alertTime -= Time.deltaTime; // Decrease the alert time
            if (alertTime <= 0.0f)
            {
                alertTime = 0.0f; // Ensure it does not go negative
                SetElementsActive(false); // Deactivate the minimap icon and ping field when alert time is zero
            }
        }
        else
        {
            SetElementsActive(false); // Deactivate the minimap icon and ping field if alert time is zero
        }
    }

    public void RecieveAlert(float duration, bool _override = false) // note this always  overrides current alert time, so it may reduce time
    {
        if (!_override && alertTime > 0.0f)
        {
            return; // If not overriding and already alerting, do nothing
        }
        alertTime = duration; // Set the alert time to the specified duration
        SetElementsActive(true); // Activate the minimap icon and ping field
    }
    
    internal void SetElementsActive(bool active)
    {
        minimapIcon.SetActive(active);
        minimapPingField.SetActive(active);
    }
}
