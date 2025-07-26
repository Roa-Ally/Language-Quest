using UnityEngine;
using UnityEditor;

public class TestDialogue : MonoBehaviour
{
    [MenuItem("Dialogue System/Create Test Dialogue")]
    public static void CreateTestDialogue()
    {
        // Create the dialogue asset
        Dialogue dialogue = ScriptableObject.CreateInstance<Dialogue>();
        
        // Set up the lines
        dialogue.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Chullachaqui",
                sentence = "¿Por qué has venido al bosque olvidado...?"
            }
        };

        // Set up the choices
        dialogue.choices = new DialogueChoice[]
        {
            new DialogueChoice
            {
                choiceText = "Estoy buscando tu historia."
            },
            new DialogueChoice
            {
                choiceText = "No entiendo este lugar..."
            },
            new DialogueChoice
            {
                choiceText = "[Use word: 'bosque']"
            }
        };

        // Save the asset
        #if UNITY_EDITOR
        AssetDatabase.CreateAsset(dialogue, "Assets/Resources/TestDialogue.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Test dialogue created at Assets/Resources/TestDialogue.asset");
        #endif
    }
} 