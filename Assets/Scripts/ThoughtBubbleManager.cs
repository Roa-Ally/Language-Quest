using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThoughtBubbleManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject thoughtBox;
    public TextMeshProUGUI thoughtText;
    public TextMeshProUGUI thoughtContinueIndicator;

    private Queue<string> thoughts;
    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private string currentThoughtText = "";

    public static bool ThoughtBubbleActive = false;

    void Start()
    {
        thoughts = new Queue<string>();
        thoughtBox.SetActive(false);
    }

    void Update()
    {
        if (!thoughtBox.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // Check if we clicked on a UI element (like the language button)
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return; // Don't advance if clicking on UI
            }
            
            if (isTyping)
            {
                if (typewriterCoroutine != null)
                    StopCoroutine(typewriterCoroutine);
                thoughtText.text = currentThoughtText;
                isTyping = false;
                if (thoughtContinueIndicator != null)
                    thoughtContinueIndicator.gameObject.SetActive(true);
            }
            else
            {
                DisplayNextThought();
            }
        }
    }

    public void StartThoughtSequence(List<string> thoughtList)
    {
        ThoughtBubbleActive = true;
        
        // Stop the player immediately when thoughts start
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.StopMovement();
        
        if (thoughtList == null || thoughtList.Count == 0)
        {
            Debug.LogError("No thoughts provided to ThoughtBubbleManager!");
            return;
        }

        thoughtBox.SetActive(true);

        // Hide continue indicator initially
        if (thoughtContinueIndicator != null)
            thoughtContinueIndicator.gameObject.SetActive(false);

        thoughts.Clear();
        
        foreach (string thought in thoughtList)
        {
            thoughts.Enqueue(thought);
        }

        DisplayNextThought();
    }

    public void DisplayNextThought()
    {
        if (isTyping)
        {
            // If still typing, finish instantly
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            thoughtText.text = currentThoughtText;
            isTyping = false;
            if (thoughtContinueIndicator != null)
                thoughtContinueIndicator.gameObject.SetActive(true);
            return;
        }
        
        if (thoughts.Count == 0)
        {
            EndThoughtSequence();
            return;
        }

        string thought = thoughts.Dequeue();
        
        currentThoughtText = thought;
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        typewriterCoroutine = StartCoroutine(TypeThought(thought));
        if (thoughtContinueIndicator != null)
            thoughtContinueIndicator.gameObject.SetActive(false);
    }

    private IEnumerator TypeThought(string text)
    {
        isTyping = true;
        thoughtText.text = "";
        foreach (char c in text)
        {
            thoughtText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
        if (thoughtContinueIndicator != null)
            thoughtContinueIndicator.gameObject.SetActive(true);
    }

    private void EndThoughtSequence()
    {
        ThoughtBubbleActive = false;
        thoughtBox.SetActive(false);
        
        // Resume player movement when thoughts end
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.ResumeMovement();
    }
} 