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
        Debug.Log($"SimpleLanguageButton: Start called. Instance set to: {instance != null}, LanguageButton: {languageButton != null}");
        
        if (languageButton != null)
        {
            languageButton.onClick.AddListener(ToggleLanguage);
            Debug.Log("SimpleLanguageButton: Language button listener added");
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
            Debug.Log("SimpleLanguageButton: Language button started hidden");
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
        
        // Refresh inventory if open
        var inventoryManager = InventoryManager.Instance;
        if (inventoryManager != null)
        {
            inventoryManager.RefreshInventoryDisplay();
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
        Debug.Log($"SimpleLanguageButton: ShowLanguageButton called. Instance: {instance != null}, LanguageButton: {instance?.languageButton != null}");
        if (instance != null && instance.languageButton != null)
        {
            instance.languageButton.gameObject.SetActive(true);
            Debug.Log("SimpleLanguageButton: Language button activated successfully");
        }
        else
        {
            Debug.LogError("SimpleLanguageButton: Cannot show language button - instance or languageButton is null");
        }
    }
    
    public static void HideLanguageButton()
    {
        Debug.Log($"SimpleLanguageButton: HideLanguageButton called. Instance: {instance != null}, LanguageButton: {instance?.languageButton != null}");
        if (instance != null && instance.languageButton != null)
        {
            instance.languageButton.gameObject.SetActive(false);
            Debug.Log("SimpleLanguageButton: Language button deactivated successfully");
        }
        else
        {
            Debug.LogError("SimpleLanguageButton: Cannot hide language button - instance or languageButton is null");
        }
    }
} 