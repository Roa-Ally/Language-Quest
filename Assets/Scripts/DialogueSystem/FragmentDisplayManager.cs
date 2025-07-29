using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class FragmentDisplayManager : MonoBehaviour
{
    public static bool FragmentActive = false;

    public GameObject fragmentPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI fragmentText;
    public Button addToJournalButton;
    public TextMeshProUGUI buttonText;

    private Action onAddToJournal;
    private string currentFragmentText = "";
    private string currentEnglishFragmentText = "";

    void Start()
    {
        if (fragmentPanel != null)
            fragmentPanel.SetActive(false);
        if (addToJournalButton != null)
            addToJournalButton.onClick.AddListener(OnAddToJournalClicked);
    }

    public void ShowFragment(string fragment, Action onAdd = null)
    {
        FragmentActive = true;
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.StopMovement();
        if (fragmentPanel != null)
            fragmentPanel.SetActive(true);
        
        // Show language button
        SimpleLanguageButton.ShowLanguageButton();
        
        // Store both texts
        currentFragmentText = fragment;
        currentEnglishFragmentText = fragment; // This will be set separately
        
        // Show/hide text components based on language
        UpdateFragmentText();
        
        onAddToJournal = onAdd;
    }
    
    public void SetEnglishFragmentText(string englishFragment)
    {
        currentEnglishFragmentText = englishFragment;
        UpdateFragmentText();
    }
    
    public void UpdateFragmentText()
    {
        if (fragmentText != null)
        {
            if (SimpleLanguageButton.isEnglish)
            {
                fragmentText.text = currentEnglishFragmentText;
            }
            else
            {
                fragmentText.text = currentFragmentText;
            }
        }
        else
        {
            Debug.LogError("fragmentText component is null! Assign it in the inspector.");
        }
    }

    private void OnAddToJournalClicked()
    {
        if (fragmentPanel != null)
            fragmentPanel.SetActive(false);
        FragmentActive = false;
        
        // Hide language button
        SimpleLanguageButton.HideLanguageButton();
        
        // Don't add to inventory here since fragments are already added in FragmentTrigger
        
        if (onAddToJournal != null)
            onAddToJournal.Invoke();
        
        // Resume player movement when fragment is closed
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
            player.ResumeMovement();
    }
} 