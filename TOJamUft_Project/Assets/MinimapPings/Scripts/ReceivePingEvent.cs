using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class PingManager : MonoBehaviour
{
    public GameObject notificationPrefab;
    public Transform notificationParent;

    public float notificationHeight = 205f;

    public AlertCopOfDDLocationEventSender AlertCopOfDDLocationEventSender;


    private void OnEnable()
    {
        AlertCopOfDDLocationEventSender.OnLocationEvent += HandleLocationEvent;
    }

    private void OnDisable()
    {
        AlertCopOfDDLocationEventSender.OnLocationEvent -= HandleLocationEvent;
    }

    public void SpawnMinimapPing(Vector2 worldPosition, float fadeDuration, Color color)
    {
        Vector3 spawnPos = new Vector3(worldPosition.x, notificationHeight, worldPosition.y);
        GameObject pingobj = Instantiate(notificationPrefab, spawnPos, notificationPrefab.transform.rotation, notificationParent);
        MinimapNotification script = pingobj.GetComponent<MinimapNotification>();
        script.Override(fadeDuration, color);
    }

    private void HandleLocationEvent(Vector2 location)
    {
        Debug.Log("Received DD location: " + location);
        SpawnMinimapPing(location, 3f, Color.orange);
    }
}
