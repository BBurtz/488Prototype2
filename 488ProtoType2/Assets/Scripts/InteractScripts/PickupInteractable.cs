using UnityEngine;
using UnityEngine.UI;

public class PickupInteractable : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    [SerializeField] private InventoryItemData itemData;

    [Tooltip("The scale of the gameobject is multiplied by the scale of the held point AND this number")]
    public float heldScaleMultiplier = 1f;
    [SerializeField] private bool IsHeldInHand;
    private Vector3 defaultScale;
    [HideInInspector] public Quaternion defaultRotation;

    //public static void CreateItemObject(InventoryItemData data, Vector3 loc)
    //{
    //    var go = new GameObject(data.name);
    //    var meshFilter = go.AddComponent<MeshFilter>();
    //    var meshRenderer = go.AddComponent<MeshRenderer>();
    //    meshFilter.mesh = data.mesh;
    //    meshRenderer.material = data.material;
    //    var collider = go.AddComponent<MeshCollider>();
    //    collider.convex = true;
    //    var rb = go.AddComponent<Rigidbody>();
    //    rb.mass = data.Weight;
    //    var pi = go.AddComponent<PickupInteractable>();
    //    pi.SetItem(data);
    //    pi.EnableRB();
    //    go.transform.localScale = new Vector3(1,1,1);
    //    go.transform.position = loc;
    //    go.transform.rotation = Quaternion.identity;
    //    go.transform.parent = null;
    //}
    private void Awake()
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
        var handscript = player.GetComponent<Hands>();
        if (handscript != null)
        {
            handscript.AddItem(itemData, player.GetComponent<Hands>().GetTargetedHand());
            Destroy(gameObject);
        }

    }
    public bool GetHeldInHand()
    {
        return IsHeldInHand;
    }
    public void SetHeldInHand(bool handHeld)
    {
        IsHeldInHand = handHeld;
    }
    public InventoryItemData GetItem()
    {
        return itemData;
    }
    public void SetItem(InventoryItemData data)
    { 
        itemData = data; 
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
    public void DisableRB()
    {
        if (rb == null)
        {
            Debug.LogWarning(gameObject.name + " has no rigidbody");
            return;
        }

        rb.detectCollisions = false;
        rb.isKinematic = true;
    }
}
