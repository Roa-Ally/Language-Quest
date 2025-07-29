using UnityEngine;
using System.Collections.Generic;

public class FragmentTrigger : MonoBehaviour
{
    [Header("Fragment Lines")]
    [TextArea(2, 5)]
    public List<string> fragmentLines = new List<string>();
    [TextArea(2, 5)]
    public List<string> englishFragmentLines = new List<string>();

    [Header("Visual Settings")]
    [SerializeField] private float spriteScale = 3.0f;
    [SerializeField] private bool hideSpriteWhenTriggered = true;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the sprite renderer and set its scale
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            transform.localScale = Vector3.one * spriteScale;
        }
    }

    public void TriggerFragment()
    {
        var manager = FindFirstObjectByType<FragmentDisplayManager>();
        if (manager != null && fragmentLines.Count > 0)
        {
            // Process each non-empty element as a separate fragment
            for (int i = 0; i < fragmentLines.Count; i++)
            {
                string fragmentText = fragmentLines[i];
                string englishFragmentText = "";
                
                // Get corresponding English text if available
                if (englishFragmentLines != null && i < englishFragmentLines.Count)
                {
                    englishFragmentText = englishFragmentLines[i];
                }
                
                // Only process non-empty fragments
                if (!string.IsNullOrEmpty(fragmentText) && !string.IsNullOrWhiteSpace(fragmentText))
                {
                    // Add to inventory directly (no need to show UI for each one)
                    var inventoryManager = InventoryManager.Instance;
                    if (inventoryManager != null)
                    {
                        inventoryManager.AddFragment(fragmentText, englishFragmentText);
                    }
                }
            }
            
            // Show combined text of all non-empty fragments in the UI
            List<string> nonEmptyFragments = new List<string>();
            List<string> nonEmptyEnglishFragments = new List<string>();
            
            for (int i = 0; i < fragmentLines.Count; i++)
            {
                string fragmentText = fragmentLines[i];
                if (!string.IsNullOrEmpty(fragmentText) && !string.IsNullOrWhiteSpace(fragmentText))
                {
                    nonEmptyFragments.Add(fragmentText);
                    
                    // Get corresponding English text if available
                    string englishFragmentText = "";
                    if (englishFragmentLines != null && i < englishFragmentLines.Count)
                    {
                        englishFragmentText = englishFragmentLines[i];
                    }
                    nonEmptyEnglishFragments.Add(englishFragmentText);
                }
            }
            
            // Show combined fragments in UI
            if (nonEmptyFragments.Count > 0)
            {
                string combinedFragmentText = string.Join("\n\n", nonEmptyFragments);
                string combinedEnglishText = string.Join("\n\n", nonEmptyEnglishFragments);
                
                manager.ShowFragment(combinedFragmentText, () => {
                    // Don't add to inventory here since we already added all fragments above
                });
                
                manager.SetEnglishFragmentText(combinedEnglishText);
            }
            
            // Hide the sprite if enabled
            if (hideSpriteWhenTriggered && spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerFragment();
            
            // Hide the sprite if enabled
            if (hideSpriteWhenTriggered && spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }
            
            GetComponent<Collider2D>().enabled = false;
        }
    }
} 