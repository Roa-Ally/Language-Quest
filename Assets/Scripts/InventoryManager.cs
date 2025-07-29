using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public TextMeshProUGUI fragmentCountText;
    public TextMeshProUGUI puzzleCountText;
    public TextMeshProUGUI fragmentListText; // New: to show fragment sentences
    public Button closeButton;
    public Button previousButton; // New: navigate to previous fragment
    public Button nextButton; // New: navigate to next fragment
    
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.E;
    
    [Header("Total Counts")]
    public int totalFragments = 4; // Update this based on your game
    public int totalRetellingPuzzles = 3;   // Update this based on your game
    
    private static InventoryManager instance;
    private int collectedFragments = 0;
    private int completedRetellingPuzzles = 0;
    private bool isInventoryOpen = false;
    private System.Collections.Generic.List<string> collectedFragmentTexts = new System.Collections.Generic.List<string>();
    private System.Collections.Generic.List<string> collectedEnglishFragmentTexts = new System.Collections.Generic.List<string>();
    private int currentFragmentIndex = 0; // Track which fragment is being displayed
    
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Look for existing instance first
                instance = FindFirstObjectByType<InventoryManager>();
                
                // If no instance exists, we need to create one with UI
                if (instance == null)
                {
                    Debug.LogWarning("No InventoryManager found! Make sure to add InventoryManager to your first scene.");
                }
            }
            return instance;
        }
    }
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make it persist across scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Always initialize UI references, even if this is a duplicate
        // This ensures the UI works in each scene
        
        // Initialize UI
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        
        // Set up close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
        
        // Set up navigation buttons
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(ShowPreviousFragment);
        }
        else
        {
            Debug.LogWarning("InventoryManager: Previous button is null!");
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ShowNextFragment);
        }
        else
        {
            Debug.LogWarning("InventoryManager: Next button is null!");
        }
        

        
        UpdateInventoryDisplay();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
        
        // Debug: Check if we're the persistent instance
        if (instance == this)
        {
            // This is the persistent instance
        }
    }
    
    // Called when scene changes to ensure player movement is restored
    void OnEnable()
    {
        // Force resume player movement when scene loads
        ResumePlayerMovement();
    }
    
    public void ToggleInventory()
    {
        if (isInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }
    
    public void OpenInventory()
    {
        
        // Try to find UI elements if they're null
        if (inventoryPanel == null)
        {
            FindUIElements();
        }
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            isInventoryOpen = true;
            
            // Update display immediately to show current data
            UpdateInventoryDisplay();
            
            // Show language button when inventory is open
            SimpleLanguageButton.ShowLanguageButton();
            
            // Pause player movement when inventory is open
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                player.StopMovement();
            }
        }
        else
        {
            Debug.LogError("InventoryManager: inventoryPanel is null! Check UI references.");
        }
    }
    
    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isInventoryOpen = false;
            
            // Hide language button when inventory is closed
            SimpleLanguageButton.HideLanguageButton();
            
            // Resume player movement when inventory is closed
            ResumePlayerMovement();
        }
    }
    
    // Method to force resume player movement
    public void ResumePlayerMovement()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            player.ResumeMovement();
        }
    }
    
    public void AddFragment(string fragmentText = "", string englishFragmentText = "")
    {
        // Only add non-empty fragments
        if (!string.IsNullOrEmpty(fragmentText) && !string.IsNullOrWhiteSpace(fragmentText))
        {
            // Check if this fragment already exists to prevent duplicates
            if (!collectedFragmentTexts.Contains(fragmentText))
            {
                collectedFragments++;
                collectedFragmentTexts.Add(fragmentText);
                collectedEnglishFragmentTexts.Add(englishFragmentText);
            }
        }
        UpdateInventoryDisplay();
    }
    
    public void AddPuzzle()
    {
        completedRetellingPuzzles++;
        UpdateInventoryDisplay();
    }
    
    private void UpdateInventoryDisplay()
    {
        if (fragmentCountText != null)
        {
            fragmentCountText.text = $"Fragments: {collectedFragments}/{totalFragments}";
        }
        
        if (puzzleCountText != null)
        {
            puzzleCountText.text = $"Retelling Puzzles: {completedRetellingPuzzles}/{totalRetellingPuzzles}";
        }
        
        if (fragmentListText != null)
        {
            if (collectedFragmentTexts.Count > 0)
            {
                // Show current fragment with navigation info
                string fragmentText = collectedFragmentTexts[currentFragmentIndex];
                string englishFragmentText = collectedEnglishFragmentTexts[currentFragmentIndex];
                
                // Use language setting to choose text
                string displayText = SimpleLanguageButton.isEnglish ? englishFragmentText : fragmentText;
                string fullDisplayText = $"Fragment {currentFragmentIndex + 1} of {collectedFragmentTexts.Count}:\n\n{displayText}";
                fragmentListText.text = fullDisplayText;
                
                // Update navigation buttons
                UpdateNavigationButtons();
            }
            else
            {
                fragmentListText.text = "No fragments collected yet.";
                UpdateNavigationButtons();
            }
        }
    }
    
    // Public getters for other scripts to check progress
    public int GetCollectedFragments() => collectedFragments;
    public int GetCompletedRetellingPuzzles() => completedRetellingPuzzles;
    public int GetTotalFragments() => totalFragments;
    public int GetTotalRetellingPuzzles() => totalRetellingPuzzles;
    
    // Navigation methods
    public void ShowPreviousFragment()
    {
        if (collectedFragmentTexts.Count > 0)
        {
            currentFragmentIndex--;
            if (currentFragmentIndex < 0)
            {
                currentFragmentIndex = collectedFragmentTexts.Count - 1; // Wrap to last
            }
            UpdateInventoryDisplay();
        }
    }
    
    public void ShowNextFragment()
    {
        if (collectedFragmentTexts.Count > 0)
        {
            currentFragmentIndex++;
            if (currentFragmentIndex >= collectedFragmentTexts.Count)
            {
                currentFragmentIndex = 0; // Wrap to first
            }
            UpdateInventoryDisplay();
        }
    }
    
    private void UpdateNavigationButtons()
    {
        if (previousButton != null)
        {
            previousButton.interactable = collectedFragmentTexts.Count > 1;
        }
        
        if (nextButton != null)
        {
            nextButton.interactable = collectedFragmentTexts.Count > 1;
        }
    }
    
    // Method to refresh display when language changes
    public void RefreshInventoryDisplay()
    {
        UpdateInventoryDisplay();
    }
    
    // Method to find UI elements in the current scene
    private void FindUIElements()
    {
        
        // Find inventory panel - search more thoroughly
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name == "Inventory Panel")
            {
                inventoryPanel = obj;
                break;
            }
        }
        
        // If not found, try searching in children of Canvas objects
        if (inventoryPanel == null)
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.name.Contains("Canvas"))
                {
                    Transform[] allChildren = obj.GetComponentsInChildren<Transform>(true);
                    foreach (Transform child in allChildren)
                    {
                        if (child.name == "Inventory Panel")
                        {
                            inventoryPanel = child.gameObject;
                            break;
                        }
                    }
                }
            }
        }
        
        // Find text elements - search more thoroughly
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in allTexts)
        {
            if (text.name == "Fragments Count")
            {
                fragmentCountText = text;
            }
            else if (text.name == "Retelling Count")
            {
                puzzleCountText = text;
            }
            else if (text.name == "Fragments Text")
            {
                fragmentListText = text;
            }
        }
        
        // If text elements not found, search in Canvas children
        if (fragmentCountText == null || puzzleCountText == null || fragmentListText == null)
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.name.Contains("Canvas"))
                {
                    TextMeshProUGUI[] canvasTexts = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI text in canvasTexts)
                    {
                        if (text.name == "Fragments Count" && fragmentCountText == null)
                        {
                            fragmentCountText = text;
                        }
                        else if (text.name == "Retelling Count" && puzzleCountText == null)
                        {
                            puzzleCountText = text;
                        }
                        else if (text.name == "Fragments Text" && fragmentListText == null)
                        {
                            fragmentListText = text;
                        }
                    }
                }
            }
        }
        
        // Find buttons - search more thoroughly
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            if (button.name == "Close Button")
            {
                closeButton = button;
            }
            else if (button.name == "PreviousButton")
            {
                previousButton = button;
            }
            else if (button.name == "NextButton")
            {
                nextButton = button;
            }
        }
        
        // If buttons not found, search in Canvas children
        if (closeButton == null || previousButton == null || nextButton == null)
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.name.Contains("Canvas"))
                {
                    Button[] canvasButtons = obj.GetComponentsInChildren<Button>(true);
                    foreach (Button button in canvasButtons)
                    {
                        if (button.name == "Close Button" && closeButton == null)
                        {
                            closeButton = button;
                        }
                        else if (button.name == "PreviousButton" && previousButton == null)
                        {
                            previousButton = button;
                        }
                        else if (button.name == "NextButton" && nextButton == null)
                        {
                            nextButton = button;
                        }
                    }
                }
            }
        }
        
        // Re-setup button listeners if buttons were found
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseInventory);
        }
        
        if (previousButton != null)
        {
            previousButton.onClick.RemoveAllListeners();
            previousButton.onClick.AddListener(ShowPreviousFragment);
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowNextFragment);
        }
    }
} 