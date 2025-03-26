using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
        print("WORKS");
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }
}
