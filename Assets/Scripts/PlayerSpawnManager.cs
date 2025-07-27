using UnityEngine;
using System.Collections;

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
        // Wait for spawn points to be available
        StartCoroutine(SpawnPlayerDelayed());
    }
    
    private System.Collections.IEnumerator SpawnPlayerDelayed()
    {
        // Wait until spawn points are assigned
        while (backSpawnPoint == null || defaultSpawnPoint == null)
        {
            yield return null; // Wait one frame
        }
        
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
            // Player spawned successfully
            
            // Make sure player is active and enabled
            player.SetActive(true);
            
            // Re-enable player movement if it was disabled
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ResumeMovement();
            }
        }
        else
        {
            Debug.LogWarning("No spawn point found!");
        }
    }
    
    private Transform GetSpawnPoint()
    {
        Debug.Log($"GetSpawnPoint called. Direction: '{lastSceneDirection}', BackSpawn: {backSpawnPoint != null}, DefaultSpawn: {defaultSpawnPoint != null}");
        
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