using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class DropZone : MonoBehaviour, IDropHandler
{
    public TMP_Text feedbackText;
    public string correctAnswer = "Espiritu";
    public Image dropZoneImage;

    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public float fadeDuration = 1.0f;

    private Color originalColor;
    private Coroutine fadeCoroutine;

    public SubgameTrigger2D subgameTrigger;
    void Start()
    {
        if (dropZoneImage != null)
            originalColor = dropZoneImage.color;

        feedbackText.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        TMP_Text draggedText = dropped.GetComponent<TMP_Text>();
        if (draggedText == null) return;

        string draggedWord = draggedText.text;
        Draggable draggable = dropped.GetComponent<Draggable>();
        feedbackText.gameObject.SetActive(true);

        if (draggedWord == correctAnswer)
        {
            feedbackText.text = "Correct!";
            feedbackText.color = Color.green;

            SetDropZoneColor(correctColor);

            // Snap to center of this drop zone
            if (draggable != null)
                draggable.SnapToAnchoredPosition(new Vector2(70f, 49f));

            StartCoroutine(CompleteSubgameAfterDelay());
        }
        else
        {
            feedbackText.text = "Try Again!";
            feedbackText.color = Color.red;

            SetDropZoneColor(incorrectColor, fade: true);

            // Send back to original position
            if (draggable != null)
                draggable.ReturnToOriginalPosition();
        }
    }

    void SetDropZoneColor(Color newColor, bool fade = false)
    {
        if (dropZoneImage == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        dropZoneImage.color = newColor;

        if (fade)
            fadeCoroutine = StartCoroutine(FadeBackToOriginal());
    }

    System.Collections.IEnumerator FadeBackToOriginal()
    {
        float elapsed = 0f;
        Color startColor = dropZoneImage.color;

        while (elapsed < fadeDuration)
        {
            dropZoneImage.color = Color.Lerp(startColor, originalColor, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        dropZoneImage.color = originalColor;
    }

    private IEnumerator CompleteSubgameAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1f);  // Waits even when Time.timeScale = 0
        if (subgameTrigger != null)
            subgameTrigger.OnSubgameComplete();
    }
}
