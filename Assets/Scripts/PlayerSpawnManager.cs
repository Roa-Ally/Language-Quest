using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform backSpawnPoint;  // For going backward to previous scene
    [SerializeField] private Transform defaultSpawnPoint; // Default spawn for first time and going forward
    
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    
    private static string lastSceneDirection = ""; // "forward" or "backward"
    
    private void Start()
    {
        SpawnPlayer();
    }
    
    private void SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogWarning("No player found with tag: " + playerTag);
            return;
        }
        
        Transform spawnPoint = GetSpawnPoint();
        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            Debug.Log($"Player spawned at: {spawnPoint.name} (Direction: {lastSceneDirection})");
        }
        else
        {
            Debug.LogWarning("No spawn point found!");
        }
    }
    
    private Transform GetSpawnPoint()
    {
        switch (lastSceneDirection)
        {
            case "backward":
                return backSpawnPoint != null ? backSpawnPoint : defaultSpawnPoint;
            default:
                return defaultSpawnPoint; // For "forward" and first time
        }
    }
    
    // Called by SceneChangeTrigger to set direction
    public static void SetSceneDirection(string direction)
    {
        lastSceneDirection = direction;
    }
    
    // Reset direction (for first time loading)
    public static void ResetDirection()
    {
        lastSceneDirection = "";
    }
} 