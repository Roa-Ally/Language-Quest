using UnityEngine;

public class SceneTransitionSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool destroyAfterSetup = true;
    
    private void Start()
    {
        if (autoSetupOnStart)
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
            if (destroyAfterSetup)
                Destroy(gameObject);
            return;
        }
        
        // Create the SceneTransitionManager
        GameObject managerGO = new GameObject("SceneTransitionManager");
        SceneTransitionManager manager = managerGO.AddComponent<SceneTransitionManager>();
        
        Debug.Log("SceneTransitionManager created successfully!");
        
        if (destroyAfterSetup)
            Destroy(gameObject);
    }
    
    // Static method to setup from anywhere
    public static void Setup()
    {
        if (SceneTransitionManager.Instance == null)
        {
            GameObject go = new GameObject("SceneTransitionManager");
            go.AddComponent<SceneTransitionManager>();
            Debug.Log("SceneTransitionManager created via static setup!");
        }
    }
} 