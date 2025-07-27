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
    public TextMeshProUGUI continueIndicator;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public Color debugButtonColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    private Queue<DialogueLine> lines;
    private Dialogue currentDialogue;
    private bool showingChoices = false;
    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private string currentLineText = "";

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
        else
            Debug.Log($"Choice Prefab is assigned: {choicePrefab.name}");
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
                    if (continueIndicator != null)
                        continueIndicator.gameObject.SetActive(true);
                }
                else
                {
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
            Debug.Log("Manually testing ShowChoices");
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
        Debug.Log($"Manual call to DisplayNextLine. Lines remaining: {lines.Count}");
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

        Debug.Log($"Starting dialogue with {dialogue.lines.Length} lines and {dialogue.choices.Length} choices");
        
        currentDialogue = dialogue;
        showingChoices = false;
        dialogueBox.SetActive(true);

        // Make sure the choices container is visible
        if (choicesContainer != null)
        {
            choicesContainer.gameObject.SetActive(true);
            Debug.Log($"Choices container is active: {choicesContainer.gameObject.activeInHierarchy}");
        }

        // Hide continue indicator initially
        if (continueIndicator != null)
            continueIndicator.gameObject.SetActive(false);

        lines.Clear();
        foreach (DialogueLine line in dialogue.lines)
        {
            lines.Enqueue(line);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        Debug.Log($"DisplayNextLine called. Lines remaining: {lines.Count}");
        
        if (isTyping)
        {
            // If still typing, finish instantly
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            dialogueText.text = currentLineText;
            isTyping = false;
            if (continueIndicator != null)
                continueIndicator.gameObject.SetActive(true);
            return;
        }
        
        if (lines.Count == 0)
        {
            Debug.Log("No more lines, showing choices");
            showingChoices = true;
            if (continueIndicator != null)
                continueIndicator.gameObject.SetActive(false);
            ShowChoices();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();
        Debug.Log($"Displaying line: {currentLine.speaker} - {currentLine.sentence}");
        
        speakerNameText.text = currentLine.speaker;
        currentLineText = currentLine.sentence;
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        typewriterCoroutine = StartCoroutine(TypeText(currentLine.sentence));
        if (continueIndicator != null)
            continueIndicator.gameObject.SetActive(false);

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
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
        if (continueIndicator != null)
            continueIndicator.gameObject.SetActive(true);
    }

    private void ShowChoices()
    {
        if (currentDialogue == null)
        {
            Debug.LogError("Current dialogue is null when trying to show choices!");
            return;
        }

        Debug.Log($"Current dialogue has {currentDialogue.choices.Length} choices");
        
        // Debug each choice
        for (int i = 0; i < currentDialogue.choices.Length; i++)
        {
            var choice = currentDialogue.choices[i];
            Debug.Log($"Choice {i + 1}: Text = '{choice.choiceText}', Has Conversation = {choice.conversation != null}");
        }
        
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

                Debug.Log($"Generating choice: {choice.choiceText}");

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
                choiceText.text = choice.choiceText;
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
                    Debug.Log($"Button {choiceIndex + 1} clicked: {choice.choiceText}");
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
            Debug.Log("No choices available, ending dialogue");
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
        Debug.Log($"Choice selected: {choice.choiceText}");
        
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
        
        // Resume player movement when dialogue ends
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.ResumeMovement();
    }
} 