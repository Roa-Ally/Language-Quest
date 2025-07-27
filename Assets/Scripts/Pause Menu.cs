using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] public GameObject pauseMenu;

   
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (!pauseMenu.activeSelf)
            {
                Time.timeScale = 0f; 
                pauseMenu.SetActive(true); 
                 
            }
            
            else
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                
            }
        }
    }

   
    public void quit()
    {
        Application.Quit();
    }

    
    public void resume()
    {
        Debug.Log("Resume");
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
       
    }

    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
