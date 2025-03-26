using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public MusicManager MM;
    public void Play()
    {
        if (MM != null)
        {
            MM.StopBGM();
        }

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
