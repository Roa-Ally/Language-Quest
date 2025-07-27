using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
       
        Cursor.visible = true;
    }
    public void PlayGame()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
        Time.timeScale = 1f;

    }

   
    public void QuitGame()
    {
       
        Debug.Log("QUIT!");
        Application.Quit();
    }

}
