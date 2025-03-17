using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInBehavior : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeDuration = 2.0f; // Duration of the fade effect

    void Awake()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeToClear());
    }

    IEnumerator FadeToClear()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeImage.color = endColor; // Ensure it's fully transparent at the end
        fadeImage.gameObject.SetActive(false);
    }
}