using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour
{
    public GameObject choicePrefab;
    public Transform testContainer;

    [ContextMenu("Test Button Creation")]
    public void TestButtonCreation()
    {
        if (choicePrefab == null)
        {
            Debug.LogError("Choice prefab is not assigned!");
            return;
        }

        if (testContainer == null)
        {
            Debug.LogError("Test container is not assigned!");
            return;
        }

        // Clear existing test buttons
        foreach (Transform child in testContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        // Create a test button
        GameObject buttonInstance = Instantiate(choicePrefab, testContainer);
        buttonInstance.transform.localScale = Vector3.one;
        buttonInstance.name = "TestButton";

        // Check components
        Button button = buttonInstance.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component missing!");
        }
        else
        {
            Debug.Log("Button component found!");
        }

        TMP_Text text = buttonInstance.GetComponentInChildren<TMP_Text>();
        if (text == null)
        {
            Debug.LogError("TMP_Text component missing!");
        }
        else
        {
            Debug.Log($"TMP_Text component found! Current text: '{text.text}'");
            text.text = "Test Choice Button";
        }

        Image image = buttonInstance.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Image component missing!");
        }
        else
        {
            Debug.Log("Image component found!");
        }

        Debug.Log("Button test completed!");
    }
} 