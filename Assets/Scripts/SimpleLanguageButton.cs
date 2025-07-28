using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleLanguageButton : MonoBehaviour
{
    public Button languageButton;
    public TextMeshProUGUI buttonText;
    
    public static bool isEnglish = false; // Default to Spanish
    
    void Start()
    {
        if (languageButton != null)
        {
            languageButton.onClick.AddListener(ToggleLanguage);
        }
        else
        {
            Debug.LogError("SimpleLanguageButton: languageButton is null! Assign the button in the inspector.");
        }
        UpdateButtonText();
    }
    
    public void ToggleLanguage()
    {
        isEnglish = !isEnglish;
        UpdateButtonText();
        
        // Refresh fragment if active
        var fragmentManager = FindFirstObjectByType<FragmentDisplayManager>();
        if (fragmentManager != null && FragmentDisplayManager.FragmentActive)
        {
            fragmentManager.UpdateFragmentText();
        }
        
        // Refresh dialogue if active
        var dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null && DialogueManager.DialogueActive)
        {
            dialogueManager.RefreshCurrentDialogue();
        }
        
        // Refresh retelling puzzle if active
        var retellingManager = FindFirstObjectByType<RetellingPuzzleManager>();
        if (retellingManager != null && RetellingPuzzleManager.RetellingActive)
        {
            retellingManager.RefreshPuzzle();
        }
    }
    
    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isEnglish ? "Change to Spanish" : "Change to English";
        }
        else
        {
            Debug.LogError("SimpleLanguageButton: buttonText is null! Assign the text component in the inspector.");
        }
    }
} 