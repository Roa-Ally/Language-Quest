using UnityEngine;
using System.Collections.Generic;

public class RetellingPuzzleTrigger : MonoBehaviour
{
    [Header("Puzzle Data")]
    [TextArea(2, 5)]
    public List<string> phrases = new List<string>
    {
        "Chullachaqui lived in the forest",
        "He protected the animals",
        "But humans came and cut down trees", 
        "So he became angry and tricked them",
        "Now he guards the forest forever"
    };

    public void TriggerPuzzle()
    {
        var manager = FindObjectOfType<RetellingPuzzleManager>();
        if (manager != null)
        {
            // Set the correct order first (unshuffled)
            manager.SetCorrectOrder(phrases);
            
            // Create a shuffled copy of the phrases
            List<string> shuffledPhrases = new List<string>(phrases);
            for (int i = 0; i < shuffledPhrases.Count; i++)
            {
                string temp = shuffledPhrases[i];
                int randomIndex = Random.Range(i, shuffledPhrases.Count);
                shuffledPhrases[i] = shuffledPhrases[randomIndex];
                shuffledPhrases[randomIndex] = temp;
            }
            
            manager.ShowPuzzle(shuffledPhrases);
        }
        else
        {
            Debug.LogError("RetellingPuzzleManager not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerPuzzle();
            // Optionally disable this trigger after use:
            GetComponent<Collider2D>().enabled = false;
        }
    }
} 