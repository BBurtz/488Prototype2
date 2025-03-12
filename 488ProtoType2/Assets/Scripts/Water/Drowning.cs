using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Drowning : MonoBehaviour
{
    [Tooltip("Time until player drowns when under water.")]
    public float TimeUntilDrown = 1;

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
            if (shipSinking.Water.transform.position.y > shipSinking.playerTransform.position.y + shipSinking.DrowningOffset)
            {
                StopCoroutine(Drown());
                drownCoroutine = null;
            }

            yield return new WaitForEndOfFrame();
        }

        //DIE
        Debug.Log("dead");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield return null;
    }
}
