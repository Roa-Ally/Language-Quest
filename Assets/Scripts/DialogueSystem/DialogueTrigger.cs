using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    void Start()
    {
        Debug.Log($"DialogueTrigger Start: {gameObject.name} at position {transform.position}");
        var collider = GetComponent<Collider2D>();
        Debug.Log($"DialogueTrigger: Collider2D={collider != null}, IsTrigger={collider?.isTrigger}, Enabled={collider?.enabled}");
    }

    public void TriggerDialogue()
    {
        Debug.Log($"DialogueTrigger: TriggerDialogue called for {gameObject.name}");
        FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DialogueTrigger: OnTriggerEnter2D called with {other.name}, tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("DialogueTrigger: Player detected, triggering dialogue!");
            TriggerDialogue();
            GetComponent<Collider2D>().enabled = false; 
        }
    }
} 