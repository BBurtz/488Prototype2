using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CanvasInteractionBehavior : Singleton<CanvasInteractionBehavior>
{
    [SerializeField] private GameObject interactPrompt;
    public static Action<string> ShowInteractUI;
    public static Action HideInteractUI;
    public Image water;
    public float timerStartY;
    public float timerEndY;



    private void Start()
    {
        ShowInteractUI += EnableInteractUI;
        HideInteractUI += DisableInteractUI;
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
    }
    public void UpdateWater()
    {
        if (water != null)
        {
            //water.fillAmount = Timer.Instance.GetNormalizedTime();
            float temp = timerStartY - timerEndY;
            temp = (temp *(1- Timer.Instance.GetNormalizedTime()));
            Debug.Log(temp + timerEndY);

            water.rectTransform.anchoredPosition = new Vector2(water.rectTransform.anchoredPosition.x, temp + timerEndY);
        }
        else
        {
            //Debug.LogError("SET THE WATER IMAGE ON THE CANVAS");
        }
    }
}