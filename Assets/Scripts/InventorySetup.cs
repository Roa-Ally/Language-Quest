using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySetup : MonoBehaviour
{
    [Header("Inventory Prefab")]
    public GameObject inventoryPrefab; // Assign the InventoryManager prefab here
    
    void Start()
    {
        // Check if InventoryManager already exists
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("No InventoryManager found in scene. Please add InventoryManager to your scenes.");
        }
    }
} 