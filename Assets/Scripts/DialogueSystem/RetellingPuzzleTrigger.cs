using UnityEngine;
using System.Collections.Generic;

public class RetellingPuzzleTrigger : MonoBehaviour
{
    [Header("Puzzle Data")]
    [TextArea(2, 5)]
    public List<string> phrases = new List<string>
    {
        "Chullachaqui vivía en el bosque",
        "Protegía a los animales",
        "Pero los humanos llegaron y cortaron árboles", 
        "Así que se enojó y los engañó",
        "Ahora protege el bosque para siempre"
    };
    
    [TextArea(2, 5)]
    public List<string> englishPhrases = new List<string>
    {
        "Chullachaqui lived in the forest",
        "He protected the animals",
        "But humans came and cut down trees", 
        "So he became angry and tricked them",
        "Now he guards the forest forever"
    };

    public void TriggerPuzzle()
    {
        var manager = FindFirstObjectByType<RetellingPuzzleManager>();
        if (manager != null)
        {
            // Ensure both lists have the same count
            if (phrases.Count != englishPhrases.Count)
            {
                Debug.LogError("Spanish and English phrases must have the same count!");
                return;
            }
            
            // Set the correct order first (unshuffled)
            manager.SetCorrectOrder(phrases, englishPhrases);
            
            // Create a shuffled copy of the phrases
            List<string> shuffledPhrases = new List<string>(phrases);
            List<string> shuffledEnglishPhrases = new List<string>(englishPhrases);
            
            // Shuffle both lists in the same way
            for (int i = 0; i < shuffledPhrases.Count; i++)
            {
                string temp = shuffledPhrases[i];
                string tempEnglish = shuffledEnglishPhrases[i];
                int randomIndex = Random.Range(i, shuffledPhrases.Count);
                shuffledPhrases[i] = shuffledPhrases[randomIndex];
                shuffledEnglishPhrases[i] = shuffledEnglishPhrases[randomIndex];
                shuffledPhrases[randomIndex] = temp;
                shuffledEnglishPhrases[randomIndex] = tempEnglish;
            }
            
            manager.ShowPuzzle(shuffledPhrases, shuffledEnglishPhrases);
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