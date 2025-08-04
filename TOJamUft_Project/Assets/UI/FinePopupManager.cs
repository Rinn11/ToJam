using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinePopupManager : MonoBehaviour
{
    public GameObject finePopupPrefab;
    public RectTransform popupContainer;

    private List<FinePopup> currentPopups = new List<FinePopup>();

    public float verticalSpacing = 40f; // distance between popups
    public float moveSpeed = 8f; // how fast they move

    public void ShowFine(int amount)
    {
        GameObject newObj = Instantiate(finePopupPrefab, popupContainer);
        FinePopup newPopup = newObj.GetComponent<FinePopup>();
        newPopup.Init(amount);

        currentPopups.Insert(0, newPopup);

        if (currentPopups.Count > 5)
        {
            Destroy(currentPopups[currentPopups.Count - 1].gameObject);
            currentPopups.RemoveAt(currentPopups.Count - 1);
        }

        StartCoroutine(RepositionPopups());
    }

    private IEnumerator RepositionPopups()
    {
        // Smoothly move each popup to its new position
        float timer = 0f;
        Vector2[] startPositions = new Vector2[currentPopups.Count];
        Vector2[] targetPositions = new Vector2[currentPopups.Count];

        for (int i = 0; i < currentPopups.Count; i++)
        {
            var popup = currentPopups[i];
            startPositions[i] = popup.rectTransform.anchoredPosition;
            targetPositions[i] = new Vector2(0f, -i * verticalSpacing);
        }

        while (timer < 1f)
        {
            timer += Time.deltaTime * moveSpeed;

            for (int i = 0; i < currentPopups.Count; i++)
            {
                if (currentPopups[i] != null)
                {
                    currentPopups[i].rectTransform.anchoredPosition = Vector2.Lerp(
                        startPositions[i],
                        targetPositions[i],
                        Mathf.SmoothStep(0, 1, timer)
                    );

                    // Apply fade for 4th and 5th
                    if (i == 3) currentPopups[i].canvasGroup.alpha = 0.6f;
                    else if (i == 4) currentPopups[i].canvasGroup.alpha = 0.3f;
                    else currentPopups[i].canvasGroup.alpha = 1f;
                }
            }

            yield return null;
        }
    }
}
