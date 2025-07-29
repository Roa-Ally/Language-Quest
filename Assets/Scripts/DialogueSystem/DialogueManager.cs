using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialogueBox;
    public Image speakerPortrait;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choicePrefab;
    public TextMeshProUGUI dialogueContinueIndicator;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public Color debugButtonColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    private Queue<DialogueLine> lines;
    private Dialogue currentDialogue;
    private bool showingChoices = false;
    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private string currentLineText = "";
    private bool shouldShowContinueIndicator = true;
    private int currentLineIndex = -1; // Track which line we're currently on

    public static bool DialogueActive = false;

    void Start()
    {
        lines = new Queue<DialogueLine>();
        dialogueBox.SetActive(false);
        
        // Debug check for required components
        if (choicesContainer == null)
            Debug.LogError("Choices Container is not assigned in DialogueManager!");
        if (choicePrefab == null)
            Debug.LogError("Choice Prefab is not assigned in DialogueManager!");
    }

    void Update()
    {
        if (!dialogueBox.activeInHierarchy)
            return;

        if (!showingChoices)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    if (typewriterCoroutine != null)
                        StopCoroutine(typewriterCoroutine);
                    dialogueText.text = currentLineText;
                    isTyping = false;
                    if (dialogueContinueIndicator != null)
                        dialogueContinueIndicator.gameObject.SetActive(true);
                }
                else
                {
                    // Hide continue indicator when advancing to next line
                    if (dialogueContinueIndicator != null)
                        dialogueContinueIndicator.gameObject.SetActive(false);
                    DisplayNextLine();
                }
            }
        }
    }

    [ContextMenu("Test Show Choices")]
    public void TestShowChoices()
    {
        if (currentDialogue != null)
        {
            ShowChoices();
        }
        else
        {
            Debug.LogError("No current dialogue to test!");
        }
    }

    [ContextMenu("Display Next Line")]
    public void DisplayNextLineManual()
    {
        DisplayNextLine();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        DialogueActive = true;
        
        // Stop the player immediately when dialogue starts
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.StopMovement();
        
        if (dialogue == null)
        {
            Debug.LogError("Trying to start a null dialogue!");
            return;
        }

        currentDialogue = dialogue;
        lines.Clear();
        dialogueBox.SetActive(true);
        
        // Show language button
        SimpleLanguageButton.ShowLanguageButton();

        foreach (DialogueLine line in dialogue.lines)
        {
            lines.Enqueue(line);
        }

        currentLineIndex = 0; // Start at first line
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            if (currentDialogue.choices.Length > 0)
            {
                ShowChoices();
                return;
            }
            else
            {
                EndDialogue();
                return;
            }
        }

        DialogueLine currentLine = lines.Dequeue();
        currentLineIndex++; // Increment line index
        
        speakerNameText.text = currentLine.speaker;
        
        // Use English text if language is English, otherwise use Spanish
        string displayText = SimpleLanguageButton.isEnglish ? currentLine.englishText : currentLine.sentence;
        
        // Fallback to Spanish if English is empty
        if (string.IsNullOrEmpty(displayText))
        {
            displayText = currentLine.sentence;
        }
        
        currentLineText = displayText;
        
        // Stop the old coroutine and set flag to prevent it from showing continue indicator
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            shouldShowContinueIndicator = false;
        }
        
        // Hide continue indicator before starting new text
        if (dialogueContinueIndicator != null)
            dialogueContinueIndicator.gameObject.SetActive(false);
        
        typewriterCoroutine = StartCoroutine(TypeText(displayText));

        if (currentLine.portrait != null)
        {
            speakerPortrait.sprite = currentLine.portrait;
            speakerPortrait.enabled = true;
        }
        else
        {
            speakerPortrait.enabled = false;
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        shouldShowContinueIndicator = true;
        
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        
        isTyping = false;
        
        // Only show continue indicator if we're not showing choices and the flag is still true
        if (dialogueContinueIndicator != null && shouldShowContinueIndicator && !showingChoices)
        {
            dialogueContinueIndicator.gameObject.SetActive(true);
        }
    }

    private void ShowChoices()
    {
        if (currentDialogue == null)
        {
            Debug.LogError("Current dialogue is null when trying to show choices!");
            return;
        }

        showingChoices = true;
        
        // Hide continue indicator when showing choices
        if (dialogueContinueIndicator != null)
            dialogueContinueIndicator.gameObject.SetActive(false);
        
        if (currentDialogue.choices.Length > 0)
        {
            // Clear existing choices
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new choice buttons
            for (int i = 0; i < currentDialogue.choices.Length; i++)
            {
                var choice = currentDialogue.choices[i];
                if (string.IsNullOrEmpty(choice.choiceText))
                {
                    Debug.LogError($"Choice {i + 1} has no text!");
                    continue;
                }

                if (choicePrefab == null)
                {
                    Debug.LogError("Choice prefab is null!");
                    continue;
                }

                // Create the button - use the proper instantiation method
                GameObject buttonInstance = Instantiate(choicePrefab, choicesContainer);
                if (buttonInstance == null)
                {
                    Debug.LogError("Failed to instantiate choice button!");
                    continue;
                }

                // Force proper setup
                buttonInstance.transform.localScale = Vector3.one;
                buttonInstance.transform.localPosition = Vector3.zero;
                buttonInstance.SetActive(true);
                buttonInstance.name = $"ChoiceButton_{i + 1}";
                
                // Get the TMP_Text component (this is the key fix)
                TMP_Text choiceText = buttonInstance.GetComponentInChildren<TMP_Text>();
                if (choiceText == null)
                {
                    Debug.LogError($"TMP_Text component missing on choice {i + 1}!");
                    continue;
                }

                // Set the text
                string choiceDisplayText = SimpleLanguageButton.isEnglish ? choice.englishChoiceText : choice.choiceText;
                
                // Fallback to Spanish if English is empty
                if (string.IsNullOrEmpty(choiceDisplayText))
                {
                    choiceDisplayText = choice.choiceText;
                }
                
                choiceText.text = choiceDisplayText;
                choiceText.color = Color.white;
                choiceText.fontSize = 20;
                choiceText.textWrappingMode = TextWrappingModes.Normal;
                choiceText.overflowMode = TextOverflowModes.Ellipsis;
                
                // Set up the button behavior
                Button button = buttonInstance.GetComponent<Button>();
                if (button == null)
                {
                    Debug.LogError($"Button component missing on choice {i + 1}!");
                    continue;
                }

                button.interactable = true;
                int choiceIndex = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    OnChoiceSelected(choice);
                });

                // Set up the button appearance
                Image buttonImage = buttonInstance.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = new Color32(155, 110, 62, 255); // #9B6E3E
                }
            }



            // Force layout update multiple times
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(choicesContainer as RectTransform);
            
            // Force another update after a frame
            StartCoroutine(ForceLayoutUpdate());
        }
        else
        {
            EndDialogue();
        }
    }

    private System.Collections.IEnumerator ForceLayoutUpdate()
    {
        yield return null; // Wait one frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(choicesContainer as RectTransform);
        Canvas.ForceUpdateCanvases();
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        showingChoices = false;
        
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        if (choice.conversation != null)
        {
            StartDialogue(choice.conversation);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        DialogueActive = false;
        dialogueBox.SetActive(false);
        
        // Hide language button
        SimpleLanguageButton.HideLanguageButton();
        
        // Resume player movement when dialogue ends
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.ResumeMovement();
    }

    public void RefreshCurrentDialogue()
    {
        if (currentDialogue != null && DialogueActive)
        {
            // Always try to update dialogue text first (if we have a current line)
            if (dialogueText != null && currentDialogue.lines != null && currentLineIndex >= 0 && currentLineIndex < currentDialogue.lines.Length)
            {
                // Get the current line directly by index
                DialogueLine currentLine = currentDialogue.lines[currentLineIndex];
                
                // Get the text in the new language
                string newText = SimpleLanguageButton.isEnglish ? currentLine.englishText : currentLine.sentence;
                
                // Fallback to Spanish if English is empty
                if (string.IsNullOrEmpty(newText))
                {
                    newText = currentLine.sentence;
                }
                
                // Update the text
                currentLineText = newText;
                dialogueText.text = newText;
                
                // If we were typing, stop it and show full text
                if (isTyping)
                {
                    if (typewriterCoroutine != null)
                    {
                        StopCoroutine(typewriterCoroutine);
                    }
                    isTyping = false;
                    
                    // Show continue indicator if it should be shown
                    if (dialogueContinueIndicator != null && shouldShowContinueIndicator)
                    {
                        dialogueContinueIndicator.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                // Try to use the last valid line index
                if (currentDialogue.lines != null && currentDialogue.lines.Length > 0)
                {
                    int validIndex = Mathf.Clamp(currentLineIndex, 0, currentDialogue.lines.Length - 1);
                    
                    DialogueLine currentLine = currentDialogue.lines[validIndex];
                    string newText = SimpleLanguageButton.isEnglish ? currentLine.englishText : currentLine.sentence;
                    
                    if (string.IsNullOrEmpty(newText))
                    {
                        newText = currentLine.sentence;
                    }
                    
                    currentLineText = newText;
                    dialogueText.text = newText;
                }
            }
            
            // If we're showing choices, also refresh the choice text
            if (showingChoices)
            {
                ShowChoices(); // This will refresh the choice text with new language
            }
        }
    }
} 