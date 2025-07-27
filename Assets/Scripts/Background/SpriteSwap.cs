using UnityEngine;

public class SpriteSwap : MonoBehaviour
{
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite newSprite;

    private Vector2 originalSize;
    private Vector3 originalScale;
    private bool originalFlipX;
    private bool originalFlipY;
    private Sprite originalSprite;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalSprite = spriteRenderer.sprite;
        originalSize = GetSpriteSize(originalSprite);
        originalScale = transform.localScale;

        originalFlipX = spriteRenderer.flipX;
        originalFlipY = spriteRenderer.flipY;
    }

    public void Swap()
    {
        if (newSprite == null) return;

        Vector2 newSize = GetSpriteSize(newSprite);
        float scaleX = originalSize.x / newSize.x;
        float scaleY = originalSize.y / newSize.y;

        // Apply scale compensation
        transform.localScale = new Vector3(
            originalScale.x * scaleX,
            originalScale.y * scaleY,
            originalScale.z
        );

        spriteRenderer.sprite = newSprite;

        spriteRenderer.flipX = originalFlipX;
        spriteRenderer.flipY = originalFlipY;
    }

    private Vector2 GetSpriteSize(Sprite sprite)
    {
        return sprite.bounds.size;
    }
}
