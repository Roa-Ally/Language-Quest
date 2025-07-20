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

    public void ShowPuzzle(List<string> phrases)
    {
        RetellingActive = true;
        
        // Stop the player immediately when puzzle starts
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
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
            SetCorrectOrder(phrases);
        }
        
        ClearPhrases();
        CreatePhrases(phrases);
    }

    public void SetCorrectOrder(List<string> phrases)
    {
        correctOrder.Clear();
        correctOrder.AddRange(phrases);
        Debug.Log($"Set correct order with {correctOrder.Count} phrases");
    }

    private void ClearPhrases()
    {
        foreach (var go in currentPhrases)
            Destroy(go);
        currentPhrases.Clear();
    }

    private void CreatePhrases(List<string> phrases)
    {
        foreach (var phrase in phrases)
        {
            GameObject go = Instantiate(phrasePrefab, phraseContainer);
            Debug.Log($"Created phrase object: {go.name}");
            
            // Try multiple ways to find the text component
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            if (text == null)
                text = go.GetComponent<TextMeshProUGUI>();
            
            if (text != null)
            {
                text.text = phrase;
                Debug.Log($"Set text for phrase: {phrase}");
            }
            else
            {
                Debug.LogError($"No TextMeshProUGUI found on phrase prefab for: {phrase}");
                Debug.LogError($"Prefab name: {phrasePrefab.name}");
                Debug.LogError($"Created object components: {string.Join(", ", go.GetComponents<Component>().Select(c => c.GetType().Name))}");
                
                // Try to add a TextMeshProUGUI component if missing
                text = go.AddComponent<TextMeshProUGUI>();
                text.text = phrase;
                text.fontSize = 16;
                text.color = Color.black;
                Debug.Log($"Added TextMeshProUGUI component and set text: {phrase}");
            }
            
            // Make sure the text is visible and properly sized
            if (text != null)
            {
                var rectTransform = text.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                }
            }
            
            var drag = go.AddComponent<PhraseDraggable>();
            drag.manager = this;
            currentPhrases.Add(go);
        }
    }

    public void ShufflePhrases()
    {
        Debug.Log("Shuffle button clicked!");
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
        Debug.Log("Submit button clicked!");
        if (correctOrder.Count == 0)
        {
            ShowFeedback("No correct order set! Please set the correctOrder list in the inspector.", false);
            return;
        }
        
        bool correct = true;
        for (int i = 0; i < correctOrder.Count && i < currentPhrases.Count; i++)
        {
            var text = currentPhrases[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text == null || text.text != correctOrder[i])
            {
                correct = false;
                Debug.Log($"Incorrect at position {i}: Expected '{correctOrder[i]}', got '{text?.text}'");
                break;
            }
        }
        
        if (correct)
        {
            ShowFeedback("Correct order! Story rebuilt!", true);
            // Close the panel after a short delay
            StartCoroutine(ClosePanelAfterDelay(2f));
        }
        else
        {
            ShowFeedback("Incorrect order. Try again!", false);
        }
    }

    private void ShowFeedback(string message, bool isSuccess)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = isSuccess ? Color.green : Color.red;
            feedbackText.gameObject.SetActive(true);
        }
        Debug.Log(message);
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
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
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
        rectTransform.anchoredPosition += eventData.delta / manager.GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
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