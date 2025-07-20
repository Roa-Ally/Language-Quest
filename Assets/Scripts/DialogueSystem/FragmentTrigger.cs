using UnityEngine;
using System.Collections.Generic;

public class FragmentTrigger : MonoBehaviour
{
    [Header("Fragment Lines")]
    [TextArea(2, 5)]
    public List<string> fragmentLines = new List<string>();

    public void TriggerFragment()
    {
        var manager = FindObjectOfType<FragmentDisplayManager>();
        if (manager != null && fragmentLines.Count > 0)
        {
            string fragmentText = string.Join("\n", fragmentLines);
            manager.ShowFragment(fragmentText, () => {
                Debug.Log("Fragment added to journal!");
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerFragment();
            GetComponent<Collider2D>().enabled = false;
        }
    }
} 