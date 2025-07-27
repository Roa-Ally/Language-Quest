using System.Collections;
using UnityEngine;

public class SpriteSwapController : MonoBehaviour
{
    [SerializeField] private SpriteSwap[] spriteSwappers;
    [SerializeField] private float delayBetweenSwaps = 0.5f;
    [SerializeField] private float initialDelay = 3f;
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
        yield return new WaitForSeconds(initialDelay);

        foreach (var swapper in spriteSwappers)
        {
            swapper.Swap();
            yield return new WaitForSeconds(delayBetweenSwaps);
        }
    }
}
