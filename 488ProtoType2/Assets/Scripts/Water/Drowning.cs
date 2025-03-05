using System.Collections;
using UnityEngine;

public class Drowning : MonoBehaviour
{
    [Tooltip("Time until player drowns when under water.")]
    public float TimeUntilDrown = 1;

    private Coroutine drownCoroutine;
    private WaterRising waterRising;

    private void Start()
    {
        waterRising = FindObjectOfType<WaterRising>();
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
            if (waterRising.gameObject.transform.position.y < waterRising.playerTransform.position.y + waterRising.DrowningOffset)
            { 
                drownCoroutine = null;
                StopCoroutine(drownCoroutine);
            }

            yield return new WaitForEndOfFrame();
        }

        //DIE
        Debug.Log("dead");

        yield return null;
    }
}
