using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleLanguageButton : MonoBehaviour
{
    public Button languageButton;
    public TextMeshProUGUI buttonText;
    
    public static bool isEnglish = false; // Default to Spanish
    private static SimpleLanguageButton instance;
    
    void Start()
    {
        instance = this;
        
        if (languageButton != null)
        {
            languageButton.onClick.AddListener(ToggleLanguage);
        }
        else
        {
            Debug.LogError("SimpleLanguageButton: languageButton is null! Assign the button in the inspector.");
        }
        UpdateButtonText();
        
        // Start hidden
        if (languageButton != null)
        {
            languageButton.gameObject.SetActive(false);
        }
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
    
    // Static method to show/hide the language button
    public static void ShowLanguageButton()
    {
        if (instance != null && instance.languageButton != null)
        {
            instance.languageButton.gameObject.SetActive(true);
        }
    }
    
    public static void HideLanguageButton()
    {
        if (instance != null && instance.languageButton != null)
        {
            instance.languageButton.gameObject.SetActive(false);
        }
    }
} 