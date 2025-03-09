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

    private void Start()
    {
        ShowInteractUI += EnableInteractUI;
        HideInteractUI += DisableInteractUI;
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
        }
        else
        {
            Debug.LogError("SET THE WATER IMAGE ON THE CANVAS");
        }
    }
}
