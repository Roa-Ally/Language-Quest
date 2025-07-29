using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private GameObject pendingCollisionObject = null;

    public GameObject targetObject1;
    public GameObject targetObject2;
    public GameObject targetObject3;

    public DragOptions option1;
    public DragOptions option2;
    public DragOptions option3;

    public float fadeDuration = 0.6f;
    public float waitBeforeNext = 1f;

    Vector3 center = new Vector3(2.20f, -0.49f, 0f);

    private Vector3 originalPos1;
    private Vector3 originalPos2;
    private Vector3 originalPos3;

    private game gameController;

    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        gameController = FindFirstObjectByType<game>();

        originalPos1 = targetObject1.transform.position;
        originalPos2 = targetObject2.transform.position;
        originalPos3 = targetObject3.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pendingCollisionObject = collision.gameObject;
    }

    void Update()
    {
        if (pendingCollisionObject != null && Input.GetMouseButtonUp(0))
        {
            HandleCollision(pendingCollisionObject, targetObject1, option1, originalPos1);
            HandleCollision(pendingCollisionObject, targetObject2, option2, originalPos2);
            HandleCollision(pendingCollisionObject, targetObject3, option3, originalPos3);

            pendingCollisionObject = null;
        }
    }

    void HandleCollision(GameObject collisionObject, GameObject targetObject, DragOptions option, Vector3 originalPos)
    {
        if (collisionObject != targetObject)
            return;

        Debug.Log("Trigger Enter");

        if (option.isCorrect)
        {
            m_spriteRenderer.color = Color.green;
            targetObject.transform.position = center;

            StartCoroutine(ResetAndContinue(targetObject, originalPos));
        }
        else
        {
            m_spriteRenderer.color = Color.red;
            StartCoroutine(FadeBackToWhite(m_spriteRenderer, fadeDuration));
        }
    }

    IEnumerator FadeBackToWhite(SpriteRenderer spriteRenderer, float duration)
    {
        Color startColor = spriteRenderer.color;
        Color targetColor = Color.white;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t / duration);
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    IEnumerator ResetAndContinue(GameObject correctObject, Vector3 originalPos)
    {
        yield return new WaitForSeconds(waitBeforeNext);

        // Reset color instantly
        m_spriteRenderer.color = Color.white;

        // Snap object back to original position
        correctObject.transform.position = originalPos;

        // Continue to next question
        gameController.NextQuestion();
    }

}
