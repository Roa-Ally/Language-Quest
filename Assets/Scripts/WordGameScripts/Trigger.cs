using UnityEngine;

public class Trigger : MonoBehaviour
{
    public PuzzleGameManager puzzleGameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            puzzleGameManager.StartGame();
        }
    }
}
