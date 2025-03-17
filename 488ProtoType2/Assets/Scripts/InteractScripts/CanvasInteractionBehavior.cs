using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasInteractionBehavior : Singleton<CanvasInteractionBehavior>
{
    public static Action PauseToggle;
    public static Action KillToggle;

    [SerializeField] private GameObject interactPrompt;
    public static Action<string> ShowInteractUI;
    public static Action HideInteractUI;
    public Image water;
    public float timerStartY;
    public float timerEndY;

    public GameObject EndScrene;
    public GameObject PauseMenu;


    private void OnEnable()
    {
        ShowInteractUI += EnableInteractUI;
        HideInteractUI += DisableInteractUI;
        PauseToggle += togglePause;
        KillToggle += endGame;
    }


    private void Update()
    {
        UpdateWater();
    }

    /// <summary>
    /// General method to show the interactable prompt
    /// </summary>
    private void EnableInteractUI(string text)
    {
        interactPrompt.SetActive(true);
        interactPrompt.GetComponent<TMP_Text>().text = text;
    }

    /// <summary>
    /// <summary>
    /// General method to hide the interactable prompt
    /// </summary>
    private void DisableInteractUI()
    {
        interactPrompt.SetActive(false);
    }
    private void OnDisable()
    {
        ShowInteractUI -= EnableInteractUI;
        HideInteractUI -= DisableInteractUI;
        PauseToggle -= togglePause;
        KillToggle -= endGame;
    }
    public void UpdateWater()
    {
        if (water != null)
        {
            //water.fillAmount = Timer.Instance.GetNormalizedTime();
            float temp = timerStartY - timerEndY;
            temp = (temp *(1- Timer.Instance.GetNormalizedTime()));

            water.rectTransform.anchoredPosition = new Vector2(water.rectTransform.anchoredPosition.x, temp + timerEndY);
        }
        else
        {
            //Debug.LogError("SET THE WATER IMAGE ON THE CANVAS");
        }
    }

    public void togglePause()
    {
        if (PauseMenu == null)
        {
            return;
        }

        if (!PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

    public void endGame()
    {
        Cursor.lockState = CursorLockMode.None;
        EndScrene.SetActive(true);
    }

    public void Play()
    {
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