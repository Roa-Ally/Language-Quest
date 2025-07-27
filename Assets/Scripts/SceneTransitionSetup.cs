using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool destroyAfterSetup = true;
    
    private static bool hasBeenSetup = false;
    
    private void Start()
    {
        // Only setup if this is the first scene and we haven't set up yet
        if (autoSetupOnStart && !hasBeenSetup && SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetupSceneTransition();
        }
    }
    
    [ContextMenu("Setup Scene Transition")]
    public void SetupSceneTransition()
    {
        // Check if SceneTransitionManager already exists
        if (SceneTransitionManager.Instance != null)
        {
            Debug.Log("SceneTransitionManager already exists in the scene.");
            hasBeenSetup = true;
            if (destroyAfterSetup)
                Destroy(gameObject);
            return;
        }
        
        // Create the SceneTransitionManager
        GameObject managerGO = new GameObject("SceneTransitionManager");
        SceneTransitionManager manager = managerGO.AddComponent<SceneTransitionManager>();
        
        Debug.Log("SceneTransitionManager created successfully!");
        hasBeenSetup = true;
        
        if (destroyAfterSetup)
            Destroy(gameObject);
    }
    
    // Static method to setup from anywhere
    public static void Setup()
    {
        if (SceneTransitionManager.Instance == null && !hasBeenSetup)
        {
            GameObject go = new GameObject("SceneTransitionManager");
            go.AddComponent<SceneTransitionManager>();
            hasBeenSetup = true;
            Debug.Log("SceneTransitionManager created via static setup!");
        }
    }
} 