using UnityEngine;

public enum TrafficNodeType { None, StopSign, TrafficLight }

public class TrafficNode : MonoBehaviour
{
    public TrafficNodeType nodeType = TrafficNodeType.None;
    public float waitTime = 2f; // Default stop time
    public bool greenLight = true; // For traffic light control

    public bool ShouldStop()
    {
        if (nodeType == TrafficNodeType.StopSign) return true;
        if (nodeType == TrafficNodeType.TrafficLight) return !greenLight;
        return false;
    }
}
