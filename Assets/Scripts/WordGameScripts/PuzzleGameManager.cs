using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGameManager : MonoBehaviour
{
    [System.Serializable]
    public class WordPair
    {
        public string english;
        public string spanish;
    }

    public WordPair[] wordPairs;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons; // Now supports 4 buttons
    public GameObject puzzleUI;
    public TextMeshProUGUI feedbackText;

    private string correctAnswer;

    void Start()
    {
        puzzleUI.SetActive(false);
        feedbackText.text = "";
    }

    public void StartGame()
    {
        puzzleUI.SetActive(true);
        LoadNewWord();
        Time.timeScale = 0f; // Freeze game
    }

    void LoadNewWord()
    {
        feedbackText.text = "";

        int index = Random.Range(0, wordPairs.Length);
        WordPair word = wordPairs[index];
        correctAnswer = word.spanish;

        questionText.text = "Translate: " + word.english;

        int correctIndex = Random.Range(0, optionButtons.Length);
        for (int i = 0; i < optionButtons.Length; i++)
        {
            string answer;
            if (i == correctIndex)
                answer = correctAnswer;
            else
                answer = GetRandomWrongAnswer(correctAnswer);

            var buttonText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = answer;

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => CheckAnswer(answer));
            optionButtons[i].interactable = true;
        }
    }

    string GetRandomWrongAnswer(string exclude)
    {
        string wrong;
        int attempts = 0;
        do
        {
            int idx = Random.Range(0, wordPairs.Length);
            wrong = wordPairs[idx].spanish;
            attempts++;
        } while ((wrong == exclude || IsAlreadyUsed(wrong)) && attempts < 100);
        return wrong;
    }

    private HashSet<string> usedAnswers = new HashSet<string>();

    bool IsAlreadyUsed(string word)
    {
        return usedAnswers.Contains(word);
    }

    void CheckAnswer(string selected)
    {
        if (selected == correctAnswer)
        {
            feedbackText.text = "Correct!";
            feedbackText.color = new Color(0.1f, 0.6f, 0.1f); // Dark green to match retelling puzzle

            foreach (var btn in optionButtons)
                btn.interactable = false;

            StartCoroutine(DelayedReset());
        }
        else
        {
            feedbackText.text = "Incorrect!";
            feedbackText.color = Color.red;
        }
    }

    IEnumerator DelayedReset()
    {
        yield return new WaitForSecondsRealtime(2f);
        ResetGame();
    }

    void ResetGame()
    {
        feedbackText.text = "";
        puzzleUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
