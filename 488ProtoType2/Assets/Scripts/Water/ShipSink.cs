using System.Collections;
using UnityEngine;

public class ShipSink : MonoBehaviour
{
    [Tooltip("Y Height the ship goes to.")]
    public float ShipHeightY;

    [Tooltip("Length in seconds it takes for ship to sink to Y Height.")]
    public float SecondsUntilShipReachesYHeight = 1;

    [Tooltip("Lowers or raises the drowning height in respect to the player height.")]
    public float DrowningOffset;

    [Tooltip("Reference to water GameObject.")]
    public GameObject Water;

    private Coroutine shipCoroutine;
    //how much ship moves each frame
    private float shipMoveIncrement;
    [HideInInspector] public Transform playerTransform;
    private Drowning DrownScript;
    private float startingY;
    private float waterHeightY;
    private Timer timerScript;

    public float ShipPercent => Mathf.InverseLerp(startingY, ShipHeightY, transform.position.y);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
        waterHeightY = Water.transform.position.y;
        DrownScript = FindFirstObjectByType<Drowning>();
        timerScript = FindFirstObjectByType<Timer>();
        startingY = transform.position.y;

        //calculates how much to move the water every frame
        shipMoveIncrement = Mathf.Abs(ShipHeightY - gameObject.transform.position.y) / SecondsUntilShipReachesYHeight;

        if (shipCoroutine == null)
        {
            shipCoroutine = StartCoroutine(ShipSinking());
        }
    }

    /// <summary>
    /// Starts rising the water
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShipSinking()
    {
        float shipTransformY = gameObject.transform.position.y;

        while (true)
        {
            //moves water
            /*
            if (gameObject.transform.position.y >= ShipHeightY)
            {
                shipTransformY -= shipMoveIncrement * Time.deltaTime;
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, shipTransformY, gameObject.transform.position.z);
            }
            */

            float y = Mathf.Lerp(startingY, ShipHeightY, timerScript.GetNormalizedTime());
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z);

            //calls to start drowning
            if (playerTransform.position.y + DrowningOffset <= waterHeightY)
            {
                DrownScript.DrownStart();
            }

            else
            {
                DrownScript.DrownOverlayImage.color = new Color(DrownScript.DrownOverlayImage.color.r, DrownScript.DrownOverlayImage.color.g, DrownScript.DrownOverlayImage.color.b, 0);
            }

            yield return null;
        }
    }


    /// <summary>
    /// Draw gizmos the reference how tall the water will go
    /// </summary>
    private void OnDrawGizmos()
    {
        Vector3 ShipHeight = new Vector3(gameObject.transform.position.x, ShipHeightY, gameObject.transform.position.z); 
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ShipHeight, 1);
        Gizmos.DrawLine(gameObject.transform.position, ShipHeight);


        Gizmos.color = Color.green;
        if (playerTransform == null)
        {
            playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
        }
        float offsetLocation = playerTransform.position.y + DrowningOffset;
        Gizmos.DrawCube(new Vector3(playerTransform.position.x, playerTransform.position.y + DrowningOffset, playerTransform.position.z), new Vector3(0.5f, 0.5f, 0.5f));

    }
}

