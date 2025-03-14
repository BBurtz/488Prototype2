using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quacken : MonoBehaviour
{
    public static Action QuackenDamaged;
    [SerializeField][Range(0, 100)][Tooltip("How much damage the Quacken must take to move to a different spot")] private float DamageThreshold;

    public float horizontalRadius = 5f; // Horizontal radius
    public float verticalRadius = 3f; // Vertical radius
    public float speed = 1f;

    public float frequency;
    public float amplitude;
    public float tiltAmount;

    public List<float> stopAngles; // Degrees to stop at

    private int currentStopIndex = 0;
    private bool isMoving = false;
    private float t = 0f;


    private void OnEnable()
    {
        QuackenDamaged += Damaged;
    }
    private void OnDisable()
    {
        QuackenDamaged -= Damaged;
    }
    void Start()
    {
        // Convert stop angles from degrees to radians
        for (int i = 0; i < stopAngles.Count; i++)
        {
            stopAngles[i] = Mathf.Deg2Rad * stopAngles[i];
        }

        // Set initial position at the first stop
        t = stopAngles[currentStopIndex];
        UpdatePosition();
        StartCoroutine(Bob());
    }
    public void Damaged()
    {
        if (!isMoving)
        {
            if (currentStopIndex < stopAngles.Count - 1)
            {
                currentStopIndex++; 

            }
            else
            {
                currentStopIndex = 0;
            }
            StartCoroutine(MoveToNextStop());
        }
    }

    /// <summary>
    /// Thanks internet
    /// </summary>
    /// <returns></returns>
    private IEnumerator Bob()
    {
        float startY = transform.position.y; 
        float time = UnityEngine.Random.Range(0f, 100f); 

        while (true)
        {
            //for bobbing up and down
            float verticalOffset = Mathf.Sin(time * frequency) * amplitude;
            transform.position = new Vector3(transform.position.x, startY + verticalOffset, transform.position.z);

            //this is a randomized Perlin noise for smooth, varied tilting
            float tiltX = (Mathf.PerlinNoise(time * 0.5f, 0f) - 0.5f) * 2f * tiltAmount;
            float tiltZ = (Mathf.PerlinNoise(0f, time * 0.5f) - 0.5f) * 2f * tiltAmount;

            //only apply tilts
            Quaternion tiltRotation = Quaternion.Euler(tiltX, 0, tiltZ);
            transform.rotation = Quaternion.Euler(tiltX, transform.eulerAngles.y, tiltZ);

            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveToNextStop()
    {
        isMoving = true;
        float startAngle = t;
        float targetAngle = stopAngles[currentStopIndex];

        if (targetAngle < startAngle)
        {
            targetAngle += 2 * Mathf.PI; //add full circle to ensure forward movement
        }

        float duration = speed; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float tValue = elapsedTime / duration; 

            //smoothStep for acceleration and deceleration
            float easedT = Mathf.SmoothStep(0f, 1f, tValue);
            t = Mathf.Lerp(startAngle, targetAngle, easedT) % (2 * Mathf.PI); //keep within valid range

            UpdatePosition();
            yield return null;
        }

        t = stopAngles[currentStopIndex];
        UpdatePosition();
        isMoving = false;
    }

    private void UpdatePosition()
    {
        float x = horizontalRadius * Mathf.Cos(t);
        float y = verticalRadius * Mathf.Sin(t);
        Vector3 newPosition = new Vector3(x, transform.position.y, y);
        transform.position = newPosition;

        //rotate to always face the center (0,0), preserving tilt from Bob()
        Vector3 directionToCenter = -new Vector3(x, 0, y).normalized;
        float angle = Mathf.Atan2(directionToCenter.x, directionToCenter.z) * Mathf.Rad2Deg;

        //keeping the existing X/Z tilt but updateing the Y-axis rotation
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);
    }



    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PickupInteractable>(out PickupInteractable temp))
        {
            var tempdmg = temp.GetItem().DamageableValue;
            Timer.Instance.HalveTickSpeedForDuration(tempdmg);
            if (tempdmg > DamageThreshold)
            {
                QuackenDamaged?.Invoke();
            }
            Destroy(collision.gameObject);
        }
    }
}
