using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapRoadCamSetup : MonoBehaviour
{
    // Drag your “Custom/TintWhite” shader in the Inspector.
    [SerializeField] Shader tintShader;

    void Awake()
    {
        var cam = GetComponent<Camera>();
        cam.SetReplacementShader(tintShader, "");   // "" = apply to everything the camera sees
    }
}