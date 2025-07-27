using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private SpriteSwap[] spriteSwappers;
    [SerializeField] private float delayBetweenSwaps = 0.5f;
    [SerializeField] private bool triggerSwap = false;

    [SerializeField] private bool hasSwapped = false;

    void Update()
    {
        if (triggerSwap && !hasSwapped)
        {
            hasSwapped = true; // ensure it only runs once
            StartCoroutine(SwapAll());
        }
    }

    private IEnumerator SwapAll()
    {
        foreach (var swapper in spriteSwappers)
        {
            swapper.Swap();
            yield return new WaitForSeconds(delayBetweenSwaps);
        }
    }
}
