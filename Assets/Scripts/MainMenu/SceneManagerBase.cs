using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("BaseScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
