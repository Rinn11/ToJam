using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FinalBlitCamera : MonoBehaviour
{
    private RenderTexture blurredSource;
    public Material blurMaterial;
    public Camera drunkDriverCamera;
    public Camera copPlayerCamera;

    // Global variable to determine the drunk driver
    public bool IsPlayer1DrunkDriver;

    void Start()
    {
        // Create a new RenderTexture with the correct dimensions
        blurredSource = new RenderTexture(Screen.width, Screen.height / 2, 0);
        blurredSource.Create();

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

        UpdateCameraViewports();
    }

    void UpdateCameraViewports()
    {
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

    void OnRenderImage(RenderTexture source, RenderTexture destination)
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

    void OnDestroy()
    {
        // Release the RenderTexture when the object is destroyed
        if (blurredSource != null)
        {
            blurredSource.Release();
        }
    }
    
    public void recievePlayerSwap(bool isPlayer1DrunkDriver)
    {
        IsPlayer1DrunkDriver = isPlayer1DrunkDriver;
        UpdateCameraViewports();
        Debug.Log($"Player swap received. Swapping camera positions");
    }
}