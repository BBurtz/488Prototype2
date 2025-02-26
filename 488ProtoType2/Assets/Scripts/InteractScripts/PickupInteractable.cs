using UnityEngine;
using UnityEngine.UI;

public class PickupInteractable : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    public float throwForce;

    [Tooltip("The scale of the gameobject is multiplied by the scale of the held point AND this number")]
    public float heldScaleMultiplier = 1f;

    private Vector3 defaultScale;
    [HideInInspector] public Quaternion defaultRotation;

    private void Start()
    {
        defaultScale = transform.lossyScale;
        defaultRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// tells the player to pick up the object and disables collisions for object
    /// </summary>
    /// <param name="player"></param>
    public void Interact(GameObject player)
    {
        player.GetComponent<Interact>().PickUpObj(gameObject);
        rb.detectCollisions = false;
        rb.isKinematic = true;
    }

    /// <summary>
    /// enables collisions
    /// </summary>
    public void EnableRB()
    {
        transform.localScale = defaultScale;

        if (rb == null)
        {
            Debug.LogWarning(gameObject.name + " has no rigidbody");
            return;
        }

        rb.detectCollisions = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    //Adds force to pickupable object
    public void ThrowObj(Vector3 direction)
    {
        rb.AddForce(direction * 100 * (float)throwForce);
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
}
