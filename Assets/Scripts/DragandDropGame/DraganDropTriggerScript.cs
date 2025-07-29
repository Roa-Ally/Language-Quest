using UnityEngine;

public class SubgameTrigger2D : MonoBehaviour
{
    [Header("Subgame Settings")]
    public GameObject subgameUI;          // Your subgame panel or canvas
    public bool pauseMainGame = true;     // Optional: pause Time.timeScale
    public bool disablePlayer = true;     // Optional: disable player script
    public bool triggerOnce = true;

    private bool hasTriggered = false;
    private GameObject player;

    void Start()
    {
        // Ensure this is a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            player = other.gameObject;
            TriggerSubgame();
        }
    }

    public void TriggerSubgame()
    {
        if (hasTriggered && triggerOnce)
            return;

        if (subgameUI != null)
        {
            subgameUI.SetActive(true);

            if (pauseMainGame)
                Time.timeScale = 0f;

            if (disablePlayer)
            {
                var controller = player.GetComponent<PlayerMovement>();
                if (controller != null)
                    controller.enabled = false;
            }

            hasTriggered = true;

            if (triggerOnce)
                GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            Debug.LogError("Subgame UI not assigned in the inspector!");
        }
    }

    public void OnSubgameComplete()
    {
        subgameUI.SetActive(false);

        if (pauseMainGame)
            Time.timeScale = 1f;

        if (disablePlayer && player != null)
        {
            var controller = player.GetComponent<PlayerMovement>();
            if (controller != null)
                controller.enabled = true;
        }
    }
}

