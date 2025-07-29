using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class IntroSceneController : MonoBehaviour
{
    [Header("Intro Settings")]
    [SerializeField] private float introDuration = 5f; // How long the intro scene plays
    [SerializeField] private string nextSceneName = "Art Map 1"; // Scene to load after intro
    
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // For fade effects
    [SerializeField] private TextMeshProUGUI introText; // Intro text display
    [SerializeField] private TextMeshProUGUI skipInstructionText; // Skip instruction text
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    
    [Header("Intro Content")]
    [SerializeField] private string[] introMessages = {
        "She never meant to find it.",
        "",
        "Tucked beneath loose floorboards in her late grandmother's cabin — the one nestled on the edge of a forgotten woodland — lay a weathered book. Its pages were brittle, its ink faded, but its voice… unmistakably alive.",
        "",
        "The language was fragmented, written half in Spanish, half in something older — something almost lost. Among the broken entries, one story stood out: the tale of a spirit with one twisted foot, a guardian named Chullachaqui, and a forest where words held power.",
        "",
        "But it wasn't just a story.",
        "",
        "Scrawled between the lines was a plea. A quest. A warning.\nHer childhood forest — once vibrant, now wilting — was dying.\nNot from fire, or drought, or time.\nIt was forgetting.",
        "",
        "The world tree, buried deep within the ancient wood, was withering. Its roots weakened as the language of the forest faded from memory. Only by journeying through the forgotten paths and recovering the lost fragments of the story could she hope to restore what was broken.",
        "",
        "Now, guided only by the scattered pages and the whispers of the forest, she must relive the tale of Chullachaqui — word by word, memory by memory — and remind the world why we must listen.",
        "",
        "Because when we forget the stories that bind us to the earth…\neven the roots begin to die."
    };
    
    [SerializeField] private float baseMessageDisplayTime = 4f; // Base time for messages
    [SerializeField] private float shortMessageDisplayTime = 2.5f; // For shorter lines
    [SerializeField] private float characterTimeMultiplier = 0.02f; // Additional time per character
    
    [Header("Skip Options")]
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipEntireIntroKey = KeyCode.Return; // Skip entire intro
    [SerializeField] private KeyCode skipToNextKey = KeyCode.Space; // Skip to next text line
    [SerializeField] private KeyCode skipToNextKeyAlt = KeyCode.Mouse0; // Alternative skip to next (mouse click)
    
    private int currentMessageIndex = 0;
    private Coroutine currentMessageCoroutine;
    private bool isDisplayingMessage = false;
    
    private void Start()
    {
        // Ensure time scale is normal
        Time.timeScale = 1f;
        
        // Initialize UI
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f; // Start with black screen
        }
        
        // Show skip instruction
        if (skipInstructionText != null)
        {
            // Determine if this is the end scene or intro scene
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string skipText = sceneName == "End Scene" ? "ending" : "intro";
            
            skipInstructionText.text = $"Press {skipToNextKey} or click to continue, {skipEntireIntroKey} to skip {skipText}";
            skipInstructionText.alpha = 0f; // Start hidden
        }
        
        // Start the intro sequence
        StartCoroutine(IntroSequence());
    }
    
    private void Update()
    {
        if (!allowSkip) return;
        
        // Skip entire intro with Space
        if (Input.GetKeyDown(skipEntireIntroKey))
        {
            StopAllCoroutines();
            StartCoroutine(TransitionToNextScene());
            return;
        }
        
        // Skip to next text line with Enter or Click
        if (Input.GetKeyDown(skipToNextKey) || Input.GetKeyDown(skipToNextKeyAlt))
        {
            if (isDisplayingMessage && currentMessageCoroutine != null)
            {
                // Stop current message and show next
                StopCoroutine(currentMessageCoroutine);
                isDisplayingMessage = false;
                DisplayNextMessage();
            }
        }
    }
    
    private IEnumerator IntroSequence()
    {
        // Fade in
        if (fadeCanvasGroup != null)
        {
            yield return StartCoroutine(FadeIn());
        }
        
        // Display intro messages
        if (introText != null && introMessages.Length > 0)
        {
            yield return StartCoroutine(DisplayIntroMessages());
        }
        else
        {
            // Just wait for intro duration if no messages
            yield return new WaitForSeconds(introDuration);
        }
        
        // Transition to next scene
        yield return StartCoroutine(TransitionToNextScene());
    }
    
    private IEnumerator DisplayIntroMessages()
    {
        currentMessageIndex = 0;
        
        while (currentMessageIndex < introMessages.Length)
        {
            string message = introMessages[currentMessageIndex];
            
            if (!string.IsNullOrEmpty(message))
            {
                // Fade out current text
                yield return StartCoroutine(FadeTextOut());
                
                // Set new text
                introText.text = message;
                
                // Fade in new text
                yield return StartCoroutine(FadeTextIn());
                
                // Show skip instruction after first text appears
                if (currentMessageIndex == 0 && skipInstructionText != null)
                {
                    yield return StartCoroutine(FadeSkipInstructionIn());
                }
                
                // Calculate display time and wait
                float displayTime = CalculateDisplayTime(message);
                isDisplayingMessage = true;
                currentMessageCoroutine = StartCoroutine(WaitForMessageDisplay(displayTime));
                yield return currentMessageCoroutine;
                isDisplayingMessage = false;
            }
            
            currentMessageIndex++;
        }
    }
    
    private IEnumerator WaitForMessageDisplay(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);
    }
    
    private void DisplayNextMessage()
    {
        // Stop current message display
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
            currentMessageCoroutine = null;
        }
        isDisplayingMessage = false;
        
        // Move to next message
        currentMessageIndex++;
        
        // If we have more messages, display the next one
        if (currentMessageIndex < introMessages.Length)
        {
            string message = introMessages[currentMessageIndex];
            if (!string.IsNullOrEmpty(message))
            {
                // Set the text immediately
                introText.text = message;
                
                // Start fade in
                StartCoroutine(FadeTextIn());
                
                // Start new display timer
                float displayTime = CalculateDisplayTime(message);
                isDisplayingMessage = true;
                currentMessageCoroutine = StartCoroutine(WaitForMessageDisplay(displayTime));
            }
            else
            {
                // If it's empty, try the next one
                DisplayNextMessage();
            }
        }
        else
        {
            // We've reached the end, transition to next scene
            StartCoroutine(TransitionToNextScene());
        }
    }
    
    private float CalculateDisplayTime(string message)
    {
        // Base time for short messages
        if (message.Length < 50)
        {
            return shortMessageDisplayTime;
        }
        
        // For longer messages, add time based on character count
        float additionalTime = message.Length * characterTimeMultiplier;
        return baseMessageDisplayTime + additionalTime;
    }
    
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }
        
        fadeCanvasGroup.alpha = 0f;
    }
    
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }
        
        fadeCanvasGroup.alpha = 1f;
    }
    
    private IEnumerator FadeTextIn()
    {
        if (introText == null) yield break;
        
        float elapsedTime = 0f;
        float fadeTime = 0.8f;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            introText.alpha = alpha;
            yield return null;
        }
        
        introText.alpha = 1f;
    }
    
    private IEnumerator FadeTextOut()
    {
        if (introText == null) yield break;
        
        float elapsedTime = 0f;
        float fadeTime = 0.8f;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            introText.alpha = alpha;
            yield return null;
        }
        
        introText.alpha = 0f;
    }
    
    private IEnumerator FadeSkipInstructionIn()
    {
        if (skipInstructionText == null) yield break;
        
        float elapsedTime = 0f;
        float fadeTime = 1f;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 0.7f, elapsedTime / fadeTime);
            skipInstructionText.alpha = alpha;
            yield return null;
        }
        
        skipInstructionText.alpha = 0.7f;
    }
    
    private IEnumerator TransitionToNextScene()
    {
        // Fade out
        if (fadeCanvasGroup != null)
        {
            yield return StartCoroutine(FadeOut());
        }
        
        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
    
    public void SkipIntro()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionToNextScene());
    }
} 