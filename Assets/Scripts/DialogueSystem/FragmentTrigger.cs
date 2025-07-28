using UnityEngine;
using System.Collections.Generic;

public class FragmentTrigger : MonoBehaviour
{
    [Header("Fragment Lines")]
    [TextArea(2, 5)]
    public List<string> fragmentLines = new List<string>();

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
            string fragmentText = string.Join("\n", fragmentLines);
            manager.ShowFragment(fragmentText, () => {
                // Fragment added to journal
            });
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