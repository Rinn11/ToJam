using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ReceivePingEvent : MonoBehaviour
{
    public GameObject notificationPrefab;
    public Transform notificationParent;

    public float notificationHeight = 205f;

    public AlertCopOfDDLocationEventSender AlertCopOfDDLocationEventSender;
    public AlertDDOfCopLocationEventSender AlertDDOfCopLocationEventSender;

    private void OnEnable()
    {
        AlertCopOfDDLocationEventSender.OnLocationEvent += HandleDDPingEvent;
        AlertDDOfCopLocationEventSender.OnLocationEvent += HandleCopPingEvent;
    }

    private void OnDisable()
    {
        AlertCopOfDDLocationEventSender.OnLocationEvent -= HandleDDPingEvent;
        AlertDDOfCopLocationEventSender.OnLocationEvent -= HandleCopPingEvent;
    }

    public void SpawnMinimapPing(Vector2 worldPosition, float fadeDuration, Color color, string layerName)
    {
        Vector3 spawnPos = new Vector3(worldPosition.x, notificationHeight, worldPosition.y);
        GameObject pingobj = Instantiate(notificationPrefab, spawnPos, notificationPrefab.transform.rotation, notificationParent);
        MinimapNotification script = pingobj.GetComponent<MinimapNotification>();
        script.Override(fadeDuration, color, layerName);
    }

    private void HandleDDPingEvent(Vector2 location)
    {
        Debug.Log("Received DD location: " + location);
        SpawnMinimapPing(location, 3f, Color.orange, "CopOnly"); // Drunk driver ping
    }

    private void HandleCopPingEvent(Vector2 location)
    {
        Debug.Log("Received DD location: " + location);
        SpawnMinimapPing(location, 3f, Color.blue, "DrunkOnly"); // Cop ping
    }
}
