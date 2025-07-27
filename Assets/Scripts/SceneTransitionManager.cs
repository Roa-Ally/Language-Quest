using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("UI References")]
    [SerializeField] private Image fadeImage;
    
    private static SceneTransitionManager instance;
    
    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SceneTransitionManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneTransitionManager");
                    instance = go.AddComponent<SceneTransitionManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupFadeUI();
        }
        else if (instance != this)
        {
            // If we already have an instance, destroy this one
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Start with fade overlay invisible
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }
        
        // Listen for scene changes to reset fade
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Don't reset the fade here - let the transition coroutine handle it
    }
    
    private void SetupFadeUI()
    {
        if (fadeImage == null)
        {
            // Create fade UI if not assigned
            GameObject canvasGO = new GameObject("FadeCanvas");
            // Don't set parent initially - create as root object
            // canvasGO.transform.SetParent(transform);
            
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // Ensure it's on top
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            GameObject imageGO = new GameObject("FadeImage");
            imageGO.transform.SetParent(canvasGO.transform);
            
            fadeImage = imageGO.AddComponent<Image>();
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
            fadeImage.raycastTarget = false;
            
            RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Make sure the canvas persists - now it's a root object
            DontDestroyOnLoad(canvasGO);
            
            // Ensure the fade image starts invisible
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }
    }
    
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }
    
    public void TransitionToScene(int sceneBuildIndex)
    {
        StartCoroutine(TransitionCoroutine(sceneBuildIndex));
    }
    
    private IEnumerator TransitionCoroutine(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(FadeToBlack());
        
        // Load the new scene
        SceneManager.LoadScene(sceneName);
        
        // Wait for scene to fully load and stabilize
        yield return new WaitForSeconds(0.1f);
        
        // Fade from black
        yield return StartCoroutine(FadeFromBlack());
    }
    
    private IEnumerator TransitionCoroutine(int sceneBuildIndex)
    {
        // Fade to black
        yield return StartCoroutine(FadeToBlack());
        
        // Load the new scene
        SceneManager.LoadScene(sceneBuildIndex);
        
        // Wait for scene to fully load and stabilize
        yield return new WaitForSeconds(0.1f);
        
        // Fade from black
        yield return StartCoroutine(FadeFromBlack());
    }
    
    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;
            float curveValue = fadeCurve.Evaluate(progress);
            float alpha = Mathf.Lerp(0f, 1f, curveValue);
            fadeImage.color = Color.Lerp(startColor, targetColor, alpha);
            yield return null;
        }
        
        fadeImage.color = targetColor;
    }
    
    private IEnumerator FadeFromBlack()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;
            float curveValue = fadeCurve.Evaluate(progress);
            float alpha = Mathf.Lerp(1f, 0f, curveValue);
            fadeImage.color = Color.Lerp(startColor, targetColor, alpha);
            yield return null;
        }
        
        fadeImage.color = targetColor;
    }
    
    // Helper method to get scene name by build index
    public string GetSceneNameByIndex(int buildIndex)
    {
        if (buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            return System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }
        return "";
    }
} 