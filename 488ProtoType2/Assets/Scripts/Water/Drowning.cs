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
    public void DrownStart()
    {
        Debug.Log("starting");

        if (drownCoroutine == null)
        {
            drownCoroutine = StartCoroutine(Drown());
        }
    }

    private IEnumerator Drown()
    {
        float updatedTime = 0;

        while (updatedTime < TimeUntilDrown)
        {
            updatedTime += Time.deltaTime;

            Debug.Log("here");
            if (waterRising.gameObject.transform.position.y < waterRising.playerTransform.position.y + waterRising.DrowningOffset)
            {
                Debug.Log("stop");
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
