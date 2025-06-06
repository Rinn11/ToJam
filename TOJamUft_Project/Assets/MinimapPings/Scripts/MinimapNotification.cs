using UnityEngine;
using UnityEngine.UI;

// This file defines the behavior of a map ping after spawning. This makes it fade, but allows us to set some parameters too

public class MinimapNotification : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    public Color startColor;

    private RawImage rawImage;
    private float timer = 0f;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    public void Override(float newFadeDuration, Color newStartColor, string layerName)
    {
        fadeDuration = newFadeDuration;
        startColor = newStartColor;
        gameObject.layer = LayerMask.NameToLayer(layerName);

        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float alpha = 1f - (timer / fadeDuration);
        alpha = Mathf.Clamp01(alpha);

        rawImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}