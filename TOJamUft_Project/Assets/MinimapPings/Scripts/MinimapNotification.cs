using UnityEngine;
using UnityEngine.UI;

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

    public void Override(float newFadeDuration, Color newStartColor)
    {
        fadeDuration = newFadeDuration;
        startColor = newStartColor;

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