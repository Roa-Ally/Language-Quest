using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainMenuMusic;
    public AudioClip introMusic;
    public AudioClip map1Music;
    public AudioClip forestMusic1;
    public AudioClip forestMusic2;  
    public AudioClip forestMusic3;
    public AudioClip endingMusic;

    private AudioSource audioSource;
    private static MusicManager instance;

    [SerializeField] private float fadeDuration = 1.5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip newClip = null;
 
        switch (scene.name)
        {
            case "MainMenu":
                newClip = mainMenuMusic;
                break;
            case "Intro scene":
                newClip = introMusic;
                break;
            case "Art Map 1":
                newClip = map1Music;
                break;
            case "Art Map 2":
                newClip = forestMusic1;
                break;
            case "Art Map 3":
                newClip = forestMusic2;
                break;
            case "Art Map 4":
                newClip = forestMusic3;
                break;
            case "End Scene":
                newClip = endingMusic;
                break;
        }

        if (newClip != null && audioSource.clip != newClip)
        {
            StartCoroutine(FadeToNewMusic(newClip));
        }
    }

    IEnumerator FadeToNewMusic(AudioClip newClip)
    {
        // Fade out current music
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(.151f, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in new music
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, .151f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = .151f;
    }
}

