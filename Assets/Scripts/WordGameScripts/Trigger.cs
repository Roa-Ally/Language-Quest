using UnityEngine;

// This script handles the trigger for starting the puzzle game when the player enters a specific area
public class Trigger : MonoBehaviour
{
    // Reference to the PuzzleGameManager to start the game
    public PuzzleGameManager puzzleGameManager;

    // This method is called when another collider enters the trigger collider attached to this GameObject
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other collider has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Start the puzzle game by calling the StartGame method on the PuzzleGameManager
            puzzleGameManager.StartGame();
        }
    }
}
