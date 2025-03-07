using System.Collections;
using Unity.Collections;
using UnityEngine;

public class WaterRising : MonoBehaviour
{

    [Tooltip("Y Height the water goes to.")]
    public float WaterHeightY;

    [Tooltip("Length in seconds it takes for water to rise to Y Height.")]
    public float SecondsUntilWaterReachesYHeight = 1;

    [Tooltip("Lowers or raises the drowning height in respect to the player height.")]
    public float DrowningOffset;

    private Coroutine waterCoroutine;
    //how much water moves each frame
    private float waterMoveIncrement;
    [HideInInspector] public Transform playerTransform;
    private Drowning DrownScript;
    private float startingY;

    public float WaterPercent => Mathf.InverseLerp(startingY, WaterHeightY, transform.position.y);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
        DrownScript = FindFirstObjectByType<Drowning>();
        startingY = transform.position.y;

        //calculates how much to move the water every frame
        waterMoveIncrement = Mathf.Abs(WaterHeightY - gameObject.transform.position.y) / SecondsUntilWaterReachesYHeight;

        if (waterCoroutine == null)
        {
            waterCoroutine = StartCoroutine(WaterRise());
        }
    }

    /// <summary>
    /// Starts rising the water
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaterRise()
    {
        float waterTransformY = gameObject.transform.position.y;

        while (true)
        {
            //moves water
            if (gameObject.transform.position.y <= WaterHeightY)
            {
                waterTransformY += waterMoveIncrement * Time.deltaTime;
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, waterTransformY, gameObject.transform.position.z);
            }
            
            //calls to start drowning
            if (waterTransformY >= playerTransform.position.y + DrowningOffset)
            {
                DrownScript.DrownStart();
            }
            yield return null;
        }
    }


    /// <summary>
    /// Draw gizmos the reference how tall the water will go
    /// </summary>
    private void OnDrawGizmos()
    {
        Vector3 WaterHeight = new Vector3(gameObject.transform.position.x, WaterHeightY, gameObject.transform.position.z); Gizmos.color = Color.blue;
        Gizmos.DrawSphere(WaterHeight, 1);
        Gizmos.DrawLine(gameObject.transform.position, WaterHeight);

    }
}
