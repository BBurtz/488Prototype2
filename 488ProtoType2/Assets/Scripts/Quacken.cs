using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Quacken : MonoBehaviour
{
    public static Action QuackenDamaged;
    [SerializeField] [Range(0,100)] [Tooltip("How much damage the Quacken must take to move to a different spot")]private float DamageThreshold;

    public float horizontalRadius = 5f; // Horizontal radius
    public float verticalRadius = 3f; // Vertical radius
    public float speed = 1f;

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
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            QuackenDamaged?.Invoke();
        }
    }
    public void Damaged()
    {
        if (!isMoving)
        {
            if(currentStopIndex < stopAngles.Count - 1)
            {
                currentStopIndex++; // Move to the next stop
                
            }
            else
            {
                currentStopIndex = 0;
            }
            StartCoroutine(MoveToNextStop());
        }

    }

    private IEnumerator MoveToNextStop()
    {
        isMoving = true;
        float targetAngle = stopAngles[currentStopIndex];

        while (Mathf.Abs(Mathf.Repeat(t, 2 * Mathf.PI) - targetAngle) > 0.05f)
        {
            t += speed * Time.deltaTime;
            UpdatePosition();
            yield return null;
        }

        // Snap to exact stop position and pause movement
        t = targetAngle;
        UpdatePosition();
        isMoving = false;
    }

    private void UpdatePosition()
    {
        // Calculate new position
        float x = horizontalRadius * Mathf.Cos(t);
        float y = verticalRadius * Mathf.Sin(t);
        Vector3 newPosition = new Vector3(x, transform.position.y, y);
        transform.position = newPosition;

        // Rotate to always face the center (0,0)
        Vector3 directionToCenter = -new Vector3(x, 0, y).normalized; // Ignore Y for flat rotation
        float angle = Mathf.Atan2(directionToCenter.x, directionToCenter.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0); // Rotate only on Y-axis
    }



    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<PickupInteractable>(out PickupInteractable temp))
        {
            var tempdmg = temp.GetItem().DamageableValue;
            Timer.Instance.HalveTickSpeedForDuration(tempdmg);
            if (tempdmg > DamageThreshold)
            {
                QuackenDamaged?.Invoke();
            }
        }
    }
}
