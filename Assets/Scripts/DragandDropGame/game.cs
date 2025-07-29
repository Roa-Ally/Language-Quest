using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class game : MonoBehaviour
{
    private List<string> originalMessages = new List<string> { "Hola", "Adiós", "Comida", "Madre", "Padre" };
    private List<string> originalQuestions = new List<string> { "Hello", "Goodbye", "Food", "Mother", "Father" };

    private List<int> usedIndices = new List<int>();

    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text questionText;

    public DragOptions option1;
    public DragOptions option2;
    public DragOptions option3;

    private string correctAnswer;

    void Start()
    {
        NextQuestion();
    }

    public void NextQuestion()
    {
        if (usedIndices.Count >= originalMessages.Count)
        {
            questionText.text = "Game Over!";
            text1.text = text2.text = text3.text = "";
            option1.isCorrect = option2.isCorrect = option3.isCorrect = false;
            return;
        }

        // Pick a new unused index
        int correctIndex;
        do
        {
            correctIndex = Random.Range(0, originalQuestions.Count);
        } while (usedIndices.Contains(correctIndex));

        usedIndices.Add(correctIndex);

        correctAnswer = originalMessages[correctIndex];
        questionText.text = originalQuestions[correctIndex];

        // Prepare answer pool with correct + 2 wrongs
        List<string> options = new List<string> { correctAnswer };

        List<string> tempPool = new List<string>(originalMessages);
        tempPool.RemoveAt(correctIndex); // remove correct one

        for (int i = 0; i < 2; i++)
        {
            int rand = Random.Range(0, tempPool.Count);
            options.Add(tempPool[rand]);
            tempPool.RemoveAt(rand);
        }

        Shuffle(options);

        // Set text
        text1.text = options[0];
        text2.text = options[1];
        text3.text = options[2];

        // Set correct flags
        option1.isCorrect = (options[0] == correctAnswer);
        option2.isCorrect = (options[1] == correctAnswer);
        option3.isCorrect = (options[2] == correctAnswer);
    }

    void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
