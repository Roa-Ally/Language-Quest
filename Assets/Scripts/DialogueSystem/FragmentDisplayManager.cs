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
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.StopMovement();
        if (fragmentPanel != null)
            fragmentPanel.SetActive(true);
        if (titleText != null)
            titleText.text = "📜 Story Fragment Unlocked!";
        if (fragmentText != null)
            fragmentText.text = fragment;
        if (buttonText != null)
            buttonText.text = "📖 Add to Journal";
        onAddToJournal = onAdd;
    }

    private void OnAddToJournalClicked()
    {
        if (fragmentPanel != null)
            fragmentPanel.SetActive(false);
        FragmentActive = false;
        if (onAddToJournal != null)
            onAddToJournal.Invoke();
        
        // Resume player movement when fragment is closed
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.ResumeMovement();
    }
} 