using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGameManager : MonoBehaviour
{
    // This script manages a simple word translation puzzle game where players translate English words to Spanish.
    [System.Serializable]
    public class WordPair
    {
        public string english;
        public string spanish;
    }

    // Array of word pairs for the game
    public WordPair[] wordPairs; // Array of word pairs for the game
    public TextMeshProUGUI questionText; // Text to display the question
    public Button[] optionButtons; // Buttons for the answer options
    public GameObject puzzleUI; // UI panel for the puzzle game
    public TextMeshProUGUI feedbackText; // Text to display feedback after an answer is selected

    private string correctAnswer; // The correct answer for the current question

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the game by hiding the puzzle UI and clearing feedback text
        puzzleUI.SetActive(false);
        feedbackText.text = "";
    }

    // Method to start the game, called when the player clicks the "Start Game" button
    public void StartGame()
    {
        // Reset the game state and load a new word
        puzzleUI.SetActive(true);
        LoadNewWord();
        Time.timeScale = 0f; 
    }

    // Method to load a new word for the player to translate
    void LoadNewWord()
    {
        // Reset the feedback text and select a random word pair
        feedbackText.text = "";

        // Ensure that the used answers are cleared for the new question
        int index = Random.Range(0, wordPairs.Length);
        WordPair word = wordPairs[index];
        correctAnswer = word.spanish;

        // Clear previously used answers
        questionText.text = "Translate: " + word.english;

        // Populate the option buttons with the correct answer and random wrong answers
        int correctIndex = Random.Range(0, optionButtons.Length);
        for (int i = 0; i < optionButtons.Length; i++)
        {
            // Clear the used answers set for the new question
            string answer;
            if (i == correctIndex)
                answer = correctAnswer;
            else
                answer = GetRandomWrongAnswer(correctAnswer);
            
            // Ensure the answer is not already used
            var buttonText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = answer;

            // Add a listener to the button to check the answer when clicked
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => CheckAnswer(answer));
            optionButtons[i].interactable = true;
        }
    }

    // Method to get a random wrong answer that is not the correct answer or already used
    string GetRandomWrongAnswer(string exclude)
    {
        // This method returns a random wrong answer from the word pairs, excluding the correct answer and already used answers
        string wrong;
        int attempts = 0;
        do
        {
            // Randomly select a word pair until a valid wrong answer is found
            int idx = Random.Range(0, wordPairs.Length);
            wrong = wordPairs[idx].spanish;
            attempts++;
            // If the wrong answer is the same as the correct answer or already used, try again
        } while ((wrong == exclude || IsAlreadyUsed(wrong)) && attempts < 100);
        return wrong;
    }

    private HashSet<string> usedAnswers = new HashSet<string>();

    // Method to check if a word has already been used in the current game session
    bool IsAlreadyUsed(string word)
    {
        // Check if the word is already in the used answers set
        return usedAnswers.Contains(word);
    }

    // Method to check the player's answer when they select an option
    void CheckAnswer(string selected)
    {
        // If the answer is correct, provide feedback and reset the game after a delay
        if (selected == correctAnswer)
        {
            // Add the correct answer to the used answers set
            feedbackText.text = "Correct!";
            feedbackText.color = new Color(0.1f, 0.6f, 0.1f);

            // Disable all option buttons to prevent further interaction
            foreach (var btn in optionButtons)
                btn.interactable = false;

            // Start a coroutine to reset the game after a delay
            StartCoroutine(DelayedReset());
        }
        else
        {
            // If the answer is incorrect, provide feedback
            feedbackText.text = "Incorrect!";
            feedbackText.color = Color.red;
        }
    }

    // Coroutine to delay the reset of the game after a correct answer
    IEnumerator DelayedReset()
    {
        yield return new WaitForSecondsRealtime(2f);
        ResetGame();
    }

    // Method to reset the game state
    void ResetGame()
    {
        // Clear the feedback text and reset the UI
        feedbackText.text = "";
        puzzleUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
