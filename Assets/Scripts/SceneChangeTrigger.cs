using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName = "";
    [SerializeField] private Direction direction = Direction.Forward;
    
    public enum Direction
    {
        Forward,
        Backward
    }
    
    private void Start()
    {
        // Make sure we have a collider and it's set as trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Set the direction for the next scene
            PlayerSpawnManager.SetSceneDirection(direction.ToString().ToLower());
            SimpleSceneTransition.Instance.TransitionToScene(targetSceneName);
        }
    }
    
    // Draw gizmos for debugging
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            // Draw the trigger area
            Gizmos.color = Color.green;
            
            if (collider is BoxCollider2D boxCollider)
            {
                Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.1f);
                Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw a line to show the target direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
        
        // Draw text label
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f, $"→ {targetSceneName} ({direction})");
        #endif
    }
} 