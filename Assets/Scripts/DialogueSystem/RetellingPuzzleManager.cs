using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;

public class RetellingPuzzleManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject retellingPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI instructionText;
    public Transform phraseContainer;
    public GameObject phrasePrefab; // Use a button or panel with TMP text
    public Button shuffleButton;
    public Button submitButton;
    public TextMeshProUGUI feedbackText; // Add this for UI feedback

    [Header("Puzzle Data")]
    [TextArea(2, 5)]
    public List<string> correctOrder = new List<string>();
    [TextArea(2, 5)]
    public List<string> englishCorrectOrder = new List<string>();
    private List<GameObject> currentPhrases = new List<GameObject>();

    public static bool RetellingActive = false;

    private void Start()
    {
        if (retellingPanel != null)
            retellingPanel.SetActive(false);
        if (shuffleButton != null)
            shuffleButton.onClick.AddListener(ShufflePhrases);
        if (submitButton != null)
            submitButton.onClick.AddListener(CheckSolution);
    }

    public void ShowPuzzle(List<string> phrases, List<string> englishPhrases = null)
    {
        RetellingActive = true;
        
        // Stop the player immediately when puzzle starts
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.StopMovement();
        
        if (retellingPanel != null)
            retellingPanel.SetActive(true);
        if (titleText != null)
            titleText.text = "Rebuild Chullachaqui's Story";
        if (instructionText != null)
            instructionText.text = "Drag the phrases into the correct order:";
        
        // Clear any previous feedback
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
        
        // Set the correct order if not already set
        if (correctOrder.Count == 0)
        {
            SetCorrectOrder(phrases, englishPhrases);
        }
        
        ClearPhrases();
        CreatePhrases(phrases, englishPhrases);
    }

    public void SetCorrectOrder(List<string> phrases, List<string> englishPhrases = null)
    {
        correctOrder.Clear();
        englishCorrectOrder.Clear();
        correctOrder.AddRange(phrases);
        if (englishPhrases != null)
        {
            englishCorrectOrder.AddRange(englishPhrases);
        }
        else
        {
            // Use Spanish phrases as fallback for English
            englishCorrectOrder.AddRange(phrases);
        }
    }

    private void ClearPhrases()
    {
        foreach (var go in currentPhrases)
            Destroy(go);
        currentPhrases.Clear();
    }

    private void CreatePhrases(List<string> phrases, List<string> englishPhrases = null)
    {
        // Use English or Spanish phrases based on language setting
        List<string> phrasesToUse = SimpleLanguageButton.isEnglish ? englishPhrases : phrases;
        
        // Fallback to Spanish if English is empty
        if (SimpleLanguageButton.isEnglish && (englishPhrases == null || englishPhrases.Count == 0))
        {
            phrasesToUse = phrases;
        }
        
        foreach (var phrase in phrasesToUse)
        {
            GameObject go = Instantiate(phrasePrefab, phraseContainer);
            
            // Try multiple ways to find the text component
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            if (text == null)
                text = go.GetComponent<TextMeshProUGUI>();
            
            if (text != null)
            {
                text.text = phrase;
            }
            else
            {
                Debug.LogError($"No TextMeshProUGUI found on phrase prefab for: {phrase}");
                Debug.LogError($"Prefab name: {phrasePrefab.name}");
                Debug.LogError($"Created object components: {string.Join(", ", go.GetComponents<Component>().Select(c => c.GetType().Name))}");
            }
            
            // Add the draggable component
            var draggable = go.GetComponent<PhraseDraggable>();
            if (draggable == null)
                draggable = go.AddComponent<PhraseDraggable>();
            
            draggable.manager = this;
            currentPhrases.Add(go);
        }
    }

    public void ShufflePhrases()
    {
        List<string> phrases = new List<string>();
        foreach (var go in currentPhrases)
        {
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                phrases.Add(text.text);
        }
        for (int i = 0; i < phrases.Count; i++)
        {
            string temp = phrases[i];
            int randomIndex = Random.Range(i, phrases.Count);
            phrases[i] = phrases[randomIndex];
            phrases[randomIndex] = temp;
        }
        ClearPhrases();
        CreatePhrases(phrases);
    }

    public void OnPhraseDropped(GameObject phrase, int newIndex)
    {
        // Remove and insert at new index
        currentPhrases.Remove(phrase);
        currentPhrases.Insert(newIndex, phrase);
        // Reorder in hierarchy
        for (int i = 0; i < currentPhrases.Count; i++)
            currentPhrases[i].transform.SetSiblingIndex(i);
    }

    public void CheckSolution()
    {
        // Get the current phrases in order
        var currentOrder = new List<string>();
        for (int i = 0; i < phraseContainer.childCount; i++)
        {
            var child = phraseContainer.GetChild(i);
            var text = child.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                currentOrder.Add(text.text);
            }
        }
        
        // Use English or Spanish correct order based on language setting
        List<string> correctOrderToCheck = SimpleLanguageButton.isEnglish ? englishCorrectOrder : correctOrder;
        
        // Fallback to Spanish if English is empty
        if (SimpleLanguageButton.isEnglish && (englishCorrectOrder == null || englishCorrectOrder.Count == 0))
        {
            correctOrderToCheck = correctOrder;
        }
        
        // Check if the order is correct
        bool isCorrect = currentOrder.SequenceEqual(correctOrderToCheck);
        
        if (isCorrect)
        {
            ShowFeedback("¡Perfecto! You've rebuilt the story correctly.", true);
        }
        else
        {
            ShowFeedback("Not quite right. Try again!", false);
        }
    }

    public void RefreshPuzzle()
    {
        if (RetellingActive && correctOrder.Count > 0)
        {
            // Restart the puzzle with new language
            ClearPhrases();
            CreatePhrases(correctOrder, englishCorrectOrder);
        }
    }

    private void ShowFeedback(string message, bool isSuccess)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = isSuccess ? new Color(0.1f, 0.6f, 0.1f) : Color.red; // Dark green instead of lime green
            feedbackText.gameObject.SetActive(true);
        }
    }

    private System.Collections.IEnumerator ClosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (retellingPanel != null)
            retellingPanel.SetActive(false);
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
        RetellingActive = false;
        
        // Resume player movement when puzzle ends
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.ResumeMovement();
    }
}

// Drag-and-drop logic for phrases
public class PhraseDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public RetellingPuzzleManager manager;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private int originalIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (manager == null)
        {
            Debug.LogError("Manager is null in PhraseDraggable.OnDrag!");
            return;
        }
        
        // Find the Canvas that contains this draggable object
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in PhraseDraggable.OnDrag! Looking for Canvas in parent hierarchy of: " + gameObject.name);
            return;
        }
        
        // Calculate the new position
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;
        
        // Apply the new position
        rectTransform.anchoredPosition = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        
        if (manager == null)
        {
            Debug.LogError("Manager is null in PhraseDraggable.OnEndDrag!");
            return;
        }
        
        // Find closest sibling index
        int newIndex = originalIndex;
        float minDist = float.MaxValue;
        for (int i = 0; i < manager.phraseContainer.childCount; i++)
        {
            if (manager.phraseContainer.GetChild(i) == transform) continue;
            float dist = Vector2.Distance(rectTransform.position, manager.phraseContainer.GetChild(i).position);
            if (dist < minDist)
            {
                minDist = dist;
                newIndex = i;
            }
        }
        manager.OnPhraseDropped(gameObject, newIndex);
    }
} 