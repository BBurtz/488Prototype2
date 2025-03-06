using System;
using TMPro;
using UnityEngine;

public class CanvasInteractionBehavior : MonoBehaviour
{
    [SerializeField] private GameObject interactPrompt;
    public static Action<string> ShowInteractUI;
    public static Action HideInteractUI;

    private void Awake()
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
}
