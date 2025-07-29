using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

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

    [Header("Completion Events")]
    [SerializeField] private bool triggerAct4Part2Events = false;

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
            
            // Set up completion callback if this trigger should trigger Act 4 Part 2 events
            if (triggerAct4Part2Events)
            {
                StartCoroutine(WaitForPuzzleCompletion());
            }
        }
        else
        {
            Debug.LogError("RetellingPuzzleManager not found in scene!");
        }
    }
    
    private System.Collections.IEnumerator WaitForPuzzleCompletion()
    {
        var manager = FindFirstObjectByType<RetellingPuzzleManager>();
        if (manager == null) yield break;
        
        // Wait until the puzzle is no longer active
        while (RetellingPuzzleManager.RetellingActive)
        {
            yield return null;
        }
        
        // Check if the puzzle was completed successfully using the manager's success flag
        if (manager.WasPuzzleCompletedSuccessfully())
        {
            TriggerAct4Part2Events();
        }
    }
    
    private void TriggerAct4Part2Events()
    {
        // Enable SwapManager triggerSwap
        var swapManager = FindFirstObjectByType<SpriteSwapController>();
        if (swapManager != null)
        {
            // Use reflection to access the private triggerSwap field
            var triggerSwapField = typeof(SpriteSwapController).GetField("triggerSwap", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (triggerSwapField != null)
            {
                triggerSwapField.SetValue(swapManager, true);
                Debug.Log("Enabled SwapManager triggerSwap");
            }
        }
        
        // Enable Tree_0000_0 animator PassedTest parameter
        var treeObjects = FindObjectsOfType<Animator>();
        foreach (var animator in treeObjects)
        {
            if (animator.gameObject.name == "Tree_0000_0")
            {
                animator.SetBool("PassedTest", true);
                Debug.Log("Set Tree_0000_0 PassedTest to true");
                break;
            }
        }
        
        // Start the delay before transitioning to end scene
        StartCoroutine(DelayToEndScene());
    }
    
    private System.Collections.IEnumerator DelayToEndScene()
    {
        Debug.Log("Starting 12 second delay before end scene transition...");
        
        // Wait 12 seconds
        yield return new WaitForSeconds(12f);
        
        Debug.Log("Delay complete, transitioning to End Scene...");
        
        // Check if scene exists in build settings
        bool sceneExists = false;
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == "End Scene")
            {
                sceneExists = true;
                Debug.Log($"Found End Scene at build index {i}");
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.LogError("End Scene not found in build settings! Please add it to Build Settings > Scenes in Build.");
        }
        else
        {
            // Transition to end scene
            SimpleSceneTransition.Instance.TransitionToScene("End Scene");
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