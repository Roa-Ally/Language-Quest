using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(3, 10)]
    public string sentence;
    [TextArea(3, 10)]
    public string englishText;
    public Sprite portrait;
}

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("The text for the choice button")]
    public string choiceText;
    [Tooltip("The English text for the choice button")]
    public string englishChoiceText;
    [Tooltip("The dialogue to trigger if this choice is selected")]
    public Dialogue conversation;
    [Tooltip("A special word that might be used or learned")]
    public string specialWord;
} 