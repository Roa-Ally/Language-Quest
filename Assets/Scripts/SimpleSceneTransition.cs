using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleSceneTransition : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;
    
    private static SimpleSceneTransition instance;
    private Image fadeImage;
    private bool isTransitioning = false;
    
    public static SimpleSceneTransition Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SimpleSceneTransition>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SimpleSceneTransition");
                    instance = go.AddComponent<SimpleSceneTransition>();
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
            CreateFadeUI();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void CreateFadeUI()
    {
        GameObject canvasGO = new GameObject("FadeCanvas");
        // Don't set parent initially - create as root object
        // canvasGO.transform.SetParent(transform);
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        
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
        
        // Now it's a root object, so DontDestroyOnLoad will work
        DontDestroyOnLoad(canvasGO);
    }
    
    public void TransitionToScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }
    }
    
    private IEnumerator TransitionCoroutine(string sceneName)
    {
        isTransitioning = true;
        
        // Fade to black
        yield return StartCoroutine(FadeToBlack());
        
        // Load scene
        SceneManager.LoadScene(sceneName);
        
        // Wait a bit
        yield return new WaitForSeconds(0.1f);
        
        // Fade from black
        yield return StartCoroutine(FadeFromBlack());
        
        isTransitioning = false;
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
            fadeImage.color = Color.Lerp(startColor, targetColor, progress);
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
            fadeImage.color = Color.Lerp(startColor, targetColor, progress);
            yield return null;
        }
        
        fadeImage.color = targetColor;
    }
} 