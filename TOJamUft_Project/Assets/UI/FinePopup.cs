using UnityEngine;
using TMPro;
using System.Collections;

public class FinePopup : MonoBehaviour
{
    public TextMeshProUGUI fineText;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    private float showDuration = 2.5f;
    private float fadeDuration = 1.0f;

    private const float minFontSize = 24f;
    private const float maxFontSize = 48f;
    private const float maxFine = 1000f;

    private const float minPunchScale = 1.0f;
    private const float maxPunchScale = 1.3f;

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 1f;
    }

    public void Init(int amount)
    {
        fineText.text = $"+{amount}";

        // Font size by value
        float t = Mathf.Clamp01(amount / maxFine);
        fineText.fontSize = Mathf.Lerp(minFontSize, maxFontSize, t);

        StopAllCoroutines();
        StartCoroutine(AnimateSpawn(t));
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator AnimateSpawn(float scaleT)
    {
        float punchScale = Mathf.Lerp(minPunchScale, maxPunchScale, scaleT);
        float duration = 0.3f;
        float timer = 0f;

        Vector3 start = Vector3.zero;
        Vector3 target = Vector3.one * punchScale;

        // Scale up (punch)
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            rectTransform.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // Snap back to normal scale
        timer = 0f;
        Vector3 final = Vector3.one;
        while (timer < duration * 0.5f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration * 0.5f);
            rectTransform.localScale = Vector3.Lerp(target, final, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        rectTransform.localScale = final;
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(showDuration);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }
}
