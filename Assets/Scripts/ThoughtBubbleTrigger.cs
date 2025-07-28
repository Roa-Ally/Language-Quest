using UnityEngine;
using System.Collections.Generic;

public class ThoughtBubbleTrigger : MonoBehaviour
{
    [Header("Thought Sequence")]
    [TextArea(2, 5)]
    public List<string> thoughts = new List<string>();
    
    [Header("Settings")]
    public bool triggerOnce = true;
    
    private bool hasTriggered = false;
    
    void Start()
    {
        // Make sure this has a trigger collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            TriggerThoughts();
        }
    }
    
    public void TriggerThoughts()
    {
        if (hasTriggered && triggerOnce)
            return;
            
        var manager = FindFirstObjectByType<ThoughtBubbleManager>();
        if (manager != null && thoughts.Count > 0)
        {
            manager.StartThoughtSequence(thoughts);
            hasTriggered = true;
            if (triggerOnce)
            {
                GetComponent<Collider2D>().enabled = false;
            }
        }
        else
        {
            Debug.LogError("ThoughtBubbleManager not found in scene or no thoughts assigned!");
        }
    }
} 