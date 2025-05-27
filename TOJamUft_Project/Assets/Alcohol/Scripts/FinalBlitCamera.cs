using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FinalBlitCamera : MonoBehaviour
{
    private RenderTexture blurredSource;
    public Material blurMaterial;
    public Camera player1Camera;
    
    void Start()
    {
        // Create a new RenderTexture with the correct dimensions
        blurredSource = new RenderTexture(Screen.width, Screen.height / 2, 0);
        blurredSource.Create();
        
        // Ensure Player 1's camera renders to the blurredSource
        if (player1Camera != null)
        {
            player1Camera.targetTexture = blurredSource;
        }
    }
    
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {   
        // Render Player 1's camera view into the blurredSource
        if (player1Camera != null)
        {
            player1Camera.Render();
        }
        // Set the destination as the active render target
        RenderTexture.active = destination;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);

        Graphics.DrawTexture(
            new Rect(0, 0, Screen.width, Screen.height / 2),
            blurredSource,
            blurMaterial
        );

        GL.PopMatrix();
    }
    
    
    void OnDestroy()
    {
        // Release the RenderTexture when the object is destroyed
        if (blurredSource != null)
        {
            blurredSource.Release();
        }

        // Reset Player 1's camera target texture
        if (player1Camera != null)
        {
            player1Camera.targetTexture = null;
        }
    }
}