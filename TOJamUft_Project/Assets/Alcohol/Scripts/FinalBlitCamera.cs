using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FinalBlitCamera : MonoBehaviour
{
    private RenderTexture blurredSource;
    public Material blurMaterial;
    
    public Camera drunkDriverCamera;
    public Camera drunkDriverUICamera;
    public Camera copPlayerCamera;
    public PlayerSwapEventSender swapSender;

    public Canvas letterboxing;
    
    // Global variable to determine the drunk driver
    public bool IsPlayer1DrunkDriver;

    // Below is dual monitor support attributes that handle camera viewports
    private bool isDualMonitor;
    public Canvas drunkDriverCanvas;
    public Canvas copPlayerCanvas;

    void Start()
    {
        int displayCount = Display.displays.Length;

        // Activate all displays
        for (int i = 1; i < displayCount; i++)
            Display.displays[i].Activate();

        isDualMonitor = displayCount >= 2;
        
        drunkDriverCanvas.renderMode   = RenderMode.ScreenSpaceCamera;
        drunkDriverCanvas.worldCamera  = drunkDriverUICamera;
        drunkDriverCanvas.planeDistance = 1f;          // positive & small
        drunkDriverCanvas.gameObject.layer = LayerMask.NameToLayer("UI");

        if (isDualMonitor)
        {
            Debug.Log("Multi monitor setup detected. Initializing cameras for dual monitor mode.");
            // set letterboxing canv unactive
            if (letterboxing != null)
            {
                letterboxing.gameObject.SetActive(false);
            }
            UpdateCameraViewportsDualMonitor();
        }
        else
        {
            Debug.Log("Single monitor setup detected. Initializing cameras for splitscreen mode.");
            blurredSource = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
            blurredSource.Create();
            // set letterboxing canvas active
            if (letterboxing != null)
            {
                letterboxing.gameObject.SetActive(true);
            }
            
            UpdateCameraViewportsSplitscreen();
        }
    }

    void UpdateCameraViewportsSplitscreen()
    {
        // Calculate the camera viewports based on the player being the drunk driver or not
        float drunkDriverX = IsPlayer1DrunkDriver ? 0 : 0.5f;
        float copCameraX = IsPlayer1DrunkDriver ? 0.5f : 0;
        // Set the camera viewports for splitscreen
        
        // drunk driver
        if (drunkDriverCamera != null)
        {
            drunkDriverCamera.rect = new Rect(drunkDriverX, 0.25f, 0.5f, 0.5f);
            // ui scale
            if (drunkDriverCanvas != null)
            {
                var canvasScaler = drunkDriverCanvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                if (canvasScaler != null)
                {
                    canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);  // should be ratio of 1920x1080
                    canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 0.5f; // Balance width and height scaling
                }
            }
            
            // Create a new RenderTexture for the blurred source
            drunkDriverCamera.targetTexture = blurredSource;
            drunkDriverCamera.depth = 0; // Render after copPlayerCamera
        }
        
        // cop car
        if (copPlayerCamera != null)
        {
            copPlayerCamera.rect = new Rect(copCameraX, 0.25f, 0.5f, 0.5f);
            // ui scale
            if (copPlayerCanvas != null)
            {
                var canvasScaler = copPlayerCanvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                if (canvasScaler != null)
                {
                    canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
                    canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 0.5f; // Balance width and height scaling
                }
            }
            // copPlayerCamera.targetTexture = null; // Render directly to the screen
            copPlayerCamera.depth = 10;
        }
        
        drunkDriverUICamera.rect = drunkDriverCamera.rect;
        // make sure UI depth is higher than both cameras
        drunkDriverUICamera.depth = 20; // Ensure UI camera renders on top
    }

    void UpdateCameraViewportsDualMonitor()
    {
        // Assign cameras to displays
        if (IsPlayer1DrunkDriver)
        {
            drunkDriverCamera.targetDisplay = 0;
            copPlayerCamera.targetDisplay = 1;
        }
        else
        { 
            drunkDriverCamera.targetDisplay = 1;
            copPlayerCamera.targetDisplay = 0;
        }
        

        // Fullscreen render texture for Display 1
        int w = Display.displays[drunkDriverCamera.targetDisplay].systemWidth;
        int h = Display.displays[drunkDriverCamera.targetDisplay].systemHeight;

        // Setup the target displays for the UI canvases of those respective cameras
        drunkDriverCanvas.worldCamera = drunkDriverCamera;
        copPlayerCanvas.worldCamera = copPlayerCamera;
        drunkDriverCanvas.targetDisplay = drunkDriverCamera.targetDisplay;
        copPlayerCanvas.targetDisplay = copPlayerCamera.targetDisplay;        

        blurredSource = new RenderTexture(w, h, 0);
        blurredSource.Create();

        drunkDriverCamera.targetTexture = blurredSource;
        drunkDriverCamera.rect = new Rect(0, 0, 1, 1);
        copPlayerCamera.rect = new Rect(0, 0, 1, 1);

        // Set this camera to overlay on Display 1
        var overlayCam = GetComponent<Camera>();
        overlayCam.targetDisplay = drunkDriverCamera.targetDisplay;
        overlayCam.depth = drunkDriverCamera.depth + 1;
        overlayCam.clearFlags = CameraClearFlags.Nothing;
        
        drunkDriverUICamera.targetDisplay = drunkDriverCamera.targetDisplay;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blurredSource != null)
        {
            Graphics.Blit(blurredSource, destination, blurMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    void OnDestroy()
    {
        // Release the RenderTexture when the object is destroyed
        if (blurredSource != null)
        {
            blurredSource.Release();
        }
    }
    
    private void OnEnable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent += recievePlayerSwap;
    }
  
    private void OnDisable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent -= recievePlayerSwap;
    }
    
    public void recievePlayerSwap(bool isPlayer1DrunkDriver)
    {
        // Update the drunk driver flag
        IsPlayer1DrunkDriver = isPlayer1DrunkDriver;

        if (isDualMonitor)
        {
            blurredSource.Release();
            UpdateCameraViewportsDualMonitor();
            Debug.Log($"Player swap received. Swapping target displays for the cameras for dual monitor setup");
        }
        else
        {
            blurredSource.Release();
            UpdateCameraViewportsSplitscreen();
            Debug.Log($"Player swap received. Swapping camera positions");
        }
    }

}