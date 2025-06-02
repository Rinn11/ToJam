using UnityEngine;

public class MultiDisplayManager : MonoBehaviour
{
    public Camera drunkDriverCamera;
    public Camera copCarCamera;

    [HideInInspector]
    public bool isSplitScreen = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Check if the system supports at least two displays. Otherwise a single display will be used but with split screen, leave it alone basically...
        if (Display.displays.Length > 1)
        {
            // Activate the second display
            Display.displays[1].Activate();
            // Set the second camera to render to the second display
            copCarCamera.targetDisplay = 1;
            isSplitScreen = false; // Set to false to use the second display as a full screen for the cop car camera
        }
        Debug.Log("MultiDisplayManager started. Number of displays: " + Display.displays.Length);
    }

    // Need to handle swaps between two different displays as needed.
    void swapDisplayPerspectives()
    {
        
    }
    
}
