using Unity.VisualScripting;
using UnityEngine;

public class DragOptions : MonoBehaviour
{
    Vector3 mousePositionOffset;
    Vector3 originalPosition;

    public bool isCorrect;

    void Start()
    {
        originalPosition = gameObject.transform.position;
    }
    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    }


    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mousePositionOffset;
    }

    private void OnMouseUp()
    {
        transform.position = originalPosition;
    }
}