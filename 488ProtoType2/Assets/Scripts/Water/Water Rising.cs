using System.Collections;
using Unity.Collections;
using UnityEngine;

public class WaterRising : MonoBehaviour
{
    [Tooltip("Water height vector3 for drawing gizmos.")]
    [ReadOnly] private Vector3 WaterHeight;

    [Tooltip ("Y Height the water goes to.")]
    public float WaterHeightY;

    [Tooltip("Length in seconds it takes for water to rise to Y Height.")]
    public int SecondsUntilWaterReachesYHeight = 1;

    private Coroutine waterCoroutine;
    private float waterMoveIncrement;

    private void Awake()
    {
        WaterHeight = new Vector3(gameObject.transform.position.x, WaterHeightY, gameObject.transform.position.z);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waterMoveIncrement = Mathf.Abs(WaterHeightY - gameObject.transform.position.y) / SecondsUntilWaterReachesYHeight;

        if (waterCoroutine == null)
        {
            waterCoroutine = StartCoroutine(WaterRise());
        }
    }

    private IEnumerator WaterRise()
    {
        float waterTransformY = gameObject.transform.position.y;

        while (gameObject.transform.position.y <= WaterHeightY)
        {
            waterTransformY += waterMoveIncrement * Time.deltaTime;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, waterTransformY, gameObject.transform.position.z);

            yield return null;
        }

        yield return null;
    }


    private void OnDrawGizmos()
    {
        Awake();
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(WaterHeight, 1);
        Gizmos.DrawLine(gameObject.transform.position, WaterHeight);

    }
}
