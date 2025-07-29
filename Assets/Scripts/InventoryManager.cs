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
                Debug.Log($"InventoryManager: Looking for instance, found: {instance != null}");
                
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
            Debug.Log("InventoryManager: Instance created and set to persist");
        }
        else if (instance != this)
        {
            Debug.Log("InventoryManager: Destroying duplicate instance");
            Destroy(gameObject);
            return;
        }
        else
        {
            Debug.Log("InventoryManager: Instance already exists, this is the same one");
        }
        
        // Always initialize UI references, even if this is a duplicate
        // This ensures the UI works in each scene
        
        // Debug current state
        Debug.Log($"InventoryManager: Current state - Fragments: {collectedFragments}, Puzzles: {completedRetellingPuzzles}");
        
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
            Debug.Log("InventoryManager: Previous button listener added");
        }
        else
        {
            Debug.LogWarning("InventoryManager: Previous button is null!");
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ShowNextFragment);
            Debug.Log("InventoryManager: Next button listener added");
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
            Debug.Log($"InventoryManager: E key pressed, instance: {instance != null}");
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
        Debug.Log($"InventoryManager: ToggleInventory called, isInventoryOpen: {isInventoryOpen}, inventoryPanel: {inventoryPanel != null}");
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
        Debug.Log($"InventoryManager: OpenInventory called, inventoryPanel: {inventoryPanel != null}");
        
        // Try to find UI elements if they're null
        if (inventoryPanel == null)
        {
            FindUIElements();
        }
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            isInventoryOpen = true;
            Debug.Log("InventoryManager: Inventory panel activated");
            
            // Update display immediately to show current data
            UpdateInventoryDisplay();
            
            // Show language button when inventory is open
            SimpleLanguageButton.ShowLanguageButton();
            Debug.Log("InventoryManager: Language button shown");
            
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
            Debug.Log("InventoryManager: Language button hidden");
            
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
            Debug.Log("InventoryManager: Player movement resumed");
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
                Debug.Log($"InventoryManager: Added new fragment. Total: {collectedFragments}");
            }
            else
            {
                Debug.Log("InventoryManager: Fragment already exists, skipping duplicate");
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
        Debug.Log("InventoryManager: Searching for UI elements in current scene");
        
        // Debug: List all GameObjects to see what's available
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        Debug.Log($"InventoryManager: Found {allObjects.Length} GameObjects in scene");
        foreach (GameObject obj in allObjects)
        {
            Debug.Log($"InventoryManager: GameObject: {obj.name}");
            if (obj.name.Contains("Inventory") || obj.name.Contains("Panel"))
            {
                Debug.Log($"InventoryManager: Found potential UI object: {obj.name}");
            }
        }
        
        // Find inventory panel - search more thoroughly
        foreach (GameObject obj in allObjects)
        {
            Debug.Log($"InventoryManager: Checking object: {obj.name}");
            if (obj.name == "Inventory Panel")
            {
                inventoryPanel = obj;
                Debug.Log($"InventoryManager: Found inventory panel: {obj.name}");
                break;
            }
        }
        
        // If not found, try searching in children of Canvas objects
        if (inventoryPanel == null)
        {
            Debug.Log("InventoryManager: Inventory Panel not found in root, searching in Canvas children");
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Canvas"))
                {
                    Debug.Log($"InventoryManager: Searching in Canvas: {obj.name}");
                    Transform[] allChildren = obj.GetComponentsInChildren<Transform>(true);
                    foreach (Transform child in allChildren)
                    {
                        Debug.Log($"InventoryManager: Canvas child: {child.name}");
                        if (child.name == "Inventory Panel")
                        {
                            inventoryPanel = child.gameObject;
                            Debug.Log($"InventoryManager: Found inventory panel in Canvas: {child.name}");
                            break;
                        }
                    }
                }
            }
        }
        
        // Find text elements - search more thoroughly
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        Debug.Log($"InventoryManager: Found {allTexts.Length} TextMeshProUGUI objects");
        foreach (TextMeshProUGUI text in allTexts)
        {
            Debug.Log($"InventoryManager: Text object found: {text.name}");
            if (text.name == "Fragments Count")
            {
                fragmentCountText = text;
                Debug.Log($"InventoryManager: Found fragment count text: {text.name}");
            }
            else if (text.name == "Retelling Count")
            {
                puzzleCountText = text;
                Debug.Log($"InventoryManager: Found retelling count text: {text.name}");
            }
            else if (text.name == "Fragments Text")
            {
                fragmentListText = text;
                Debug.Log($"InventoryManager: Found fragment list text: {text.name}");
            }
        }
        
        // If text elements not found, search in Canvas children
        if (fragmentCountText == null || puzzleCountText == null || fragmentListText == null)
        {
            Debug.Log("InventoryManager: Some text elements not found, searching in Canvas children");
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Canvas"))
                {
                    TextMeshProUGUI[] canvasTexts = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI text in canvasTexts)
                    {
                        Debug.Log($"InventoryManager: Canvas text found: {text.name}");
                        if (text.name == "Fragments Count" && fragmentCountText == null)
                        {
                            fragmentCountText = text;
                            Debug.Log($"InventoryManager: Found fragment count text in Canvas: {text.name}");
                        }
                        else if (text.name == "Retelling Count" && puzzleCountText == null)
                        {
                            puzzleCountText = text;
                            Debug.Log($"InventoryManager: Found retelling count text in Canvas: {text.name}");
                        }
                        else if (text.name == "Fragments Text" && fragmentListText == null)
                        {
                            fragmentListText = text;
                            Debug.Log($"InventoryManager: Found fragment list text in Canvas: {text.name}");
                        }
                    }
                }
            }
        }
        
        // Find buttons - search more thoroughly
        Button[] allButtons = FindObjectsOfType<Button>();
        Debug.Log($"InventoryManager: Found {allButtons.Length} Button objects");
        foreach (Button button in allButtons)
        {
            Debug.Log($"InventoryManager: Button object found: {button.name}");
            if (button.name == "Close Button")
            {
                closeButton = button;
                Debug.Log($"InventoryManager: Found close button: {button.name}");
            }
            else if (button.name == "PreviousButton")
            {
                previousButton = button;
                Debug.Log($"InventoryManager: Found previous button: {button.name}");
            }
            else if (button.name == "NextButton")
            {
                nextButton = button;
                Debug.Log($"InventoryManager: Found next button: {button.name}");
            }
        }
        
        // If buttons not found, search in Canvas children
        if (closeButton == null || previousButton == null || nextButton == null)
        {
            Debug.Log("InventoryManager: Some buttons not found, searching in Canvas children");
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Canvas"))
                {
                    Button[] canvasButtons = obj.GetComponentsInChildren<Button>(true);
                    foreach (Button button in canvasButtons)
                    {
                        Debug.Log($"InventoryManager: Canvas button found: {button.name}");
                        if (button.name == "Close Button" && closeButton == null)
                        {
                            closeButton = button;
                            Debug.Log($"InventoryManager: Found close button in Canvas: {button.name}");
                        }
                        else if (button.name == "PreviousButton" && previousButton == null)
                        {
                            previousButton = button;
                            Debug.Log($"InventoryManager: Found previous button in Canvas: {button.name}");
                        }
                        else if (button.name == "NextButton" && nextButton == null)
                        {
                            nextButton = button;
                            Debug.Log($"InventoryManager: Found next button in Canvas: {button.name}");
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
            Debug.Log("InventoryManager: Close button listener re-setup");
        }
        
        if (previousButton != null)
        {
            previousButton.onClick.RemoveAllListeners();
            previousButton.onClick.AddListener(ShowPreviousFragment);
            Debug.Log("InventoryManager: Previous button listener re-setup");
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowNextFragment);
            Debug.Log("InventoryManager: Next button listener re-setup");
        }
    }
} 