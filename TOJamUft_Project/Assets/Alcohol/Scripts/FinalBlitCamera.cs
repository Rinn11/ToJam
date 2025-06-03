using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FinalBlitCamera : MonoBehaviour
{
    private RenderTexture blurredSource;
    public Material blurMaterial;
    public Camera drunkDriverCamera;
    public Camera copPlayerCamera;
    public PlayerSwapEventSender swapSender;
    
    
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

        if (isDualMonitor)
        {
            Debug.Log("Multi monitor setup detected. Initializing cameras for dual monitor mode.");
            UpdateCameraViewportsDualMonitor();
        }
        else
        {
            Debug.Log("Single monitor setup detected. Initializing cameras for splitscreen mode.");

            // Create a new RenderTexture with the correct dimensions
            blurredSource = new RenderTexture(Screen.width, Screen.height / 2, 0);
            blurredSource.Create();

            UpdateCameraViewportsSplitscreen();
        }
    }

    void UpdateCameraViewportsSplitscreen()
    {
        // Ensure drunk driver camera always occupies the full screen
        if (drunkDriverCamera != null)
        {
            drunkDriverCamera.rect = new Rect(0, 0, 1, 1);
        }

        // Set cop player camera position based on who is the drunk driver
        if (copPlayerCamera != null)
        {
            if (IsPlayer1DrunkDriver)
            {
                copPlayerCamera.rect = new Rect(0, 0, 1, 0.5f); // Bottom half
            }
            else
            {
                copPlayerCamera.rect = new Rect(0, 0.5f, 1, 0.5f); // Top half
            }
        }
        // Apply blur effect to the drunk driver camera
        if (drunkDriverCamera != null)
        {
            drunkDriverCamera.targetTexture = blurredSource;
            drunkDriverCamera.depth = 0; // Render after copPlayerCamera
        }

        // Ensure cop player camera renders normally
        if (copPlayerCamera != null)
        {
            copPlayerCamera.depth = -1; // Render first
        }
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
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blurredSource == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (isDualMonitor)
        {
            // Fullscreen blur (dual monitor)
            Graphics.Blit(blurredSource, destination, blurMaterial);
        }
        else
        { 
            // Render the blurred texture for the drunk driver camera
            RenderTexture.active = destination;

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);

            // Draw the blurred texture on the drunk driver camera's viewport
            if (IsPlayer1DrunkDriver)
            {
                Graphics.DrawTexture(
                    new Rect(0, 0, Screen.width, Screen.height / 2),
                    blurredSource,
                    blurMaterial
                );
            }
            else
            {
                Graphics.DrawTexture(
                    new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2),
                    blurredSource,
                    blurMaterial
                );
            }

            GL.PopMatrix();
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
            // Recreate the RenderTexture
            if (blurredSource != null)
            {
                blurredSource.Release();
            }
            blurredSource = new RenderTexture(Screen.width, Screen.height / 2, 0);
            blurredSource.Create();

            // Update the blur material with the new RenderTexture
            if (blurMaterial != null)
            {
                blurMaterial.SetTexture("_MainTex", blurredSource);
            }

            // Update camera settings
            if (drunkDriverCamera != null)
            {
                drunkDriverCamera.targetTexture = blurredSource;
                drunkDriverCamera.rect = new Rect(0, 0, 1, 1); // Full screen
                drunkDriverCamera.depth = 0; // Render after copPlayerCamera
            }

            if (copPlayerCamera != null)
            {
                copPlayerCamera.targetTexture = null; // Render directly to the screen
                copPlayerCamera.depth = -1; // Render first

                // Adjust viewport based on the drunk driver
                copPlayerCamera.rect = IsPlayer1DrunkDriver
                    ? new Rect(0, 0, 1, 0.5f) // Bottom half
                    : new Rect(0, 0.5f, 1, 0.5f); // Top half
            }

            Debug.Log($"Player swap received. Swapping camera positions");
        }
    }
}