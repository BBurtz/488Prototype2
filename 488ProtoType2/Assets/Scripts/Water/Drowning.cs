using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Drowning : MonoBehaviour
{
    [Tooltip("Time until player drowns when under water.")]
    public float TimeUntilDrown = 1;

    [Tooltip("Image that indicates player drowning.")]
    public Image DrownOverlayImage;

    private Coroutine drownCoroutine;
    private ShipSink shipSinking;

    private void Start()
    {
        shipSinking = FindFirstObjectByType<ShipSink>();
    }

    /// <summary>
    /// Starts the coroutine if not started already
    /// </summary>
    public void DrownStart()
    {
        if (drownCoroutine == null)
        {
            DrownOverlayImage.color = new Color(DrownOverlayImage.color.r, DrownOverlayImage.color.g, DrownOverlayImage.color.b, 0.5f);
            drownCoroutine = StartCoroutine(Drown());
        }
    }


    /// <summary>
    /// Coroutine that runs player drowning and decides player death
    /// </summary>
    /// <returns></returns>
    private IEnumerator Drown()
    {
        float updatedTime = 0;

        while (updatedTime < TimeUntilDrown)
        {
            updatedTime += Time.deltaTime;
            //if player resurfaces
            if (shipSinking.Water.transform.position.y < shipSinking.playerTransform.position.y + shipSinking.DrowningOffset)
            {
                DrownOverlayImage.color = new Color(DrownOverlayImage.color.r, DrownOverlayImage.color.g, DrownOverlayImage.color.b, 0);
                StopCoroutine(drownCoroutine);
                drownCoroutine = null;
            }

            //drowning indication
            float drownOverlayImageAlpha = Mathf.Lerp(0.5f, 1, updatedTime / TimeUntilDrown);
            DrownOverlayImage.color = new Color(DrownOverlayImage.color.r, DrownOverlayImage.color.g, DrownOverlayImage.color.b, Mathf.Pow(drownOverlayImageAlpha, 4));

            yield return new WaitForEndOfFrame();
        }

        //DIE
        Debug.Log("dead");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield return null;
    }

}
