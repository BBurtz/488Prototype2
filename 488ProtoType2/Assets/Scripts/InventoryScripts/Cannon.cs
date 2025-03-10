using UnityEngine;
using System.Collections;

public class Cannon : InventoryHolder, IInteractable
{
    [SerializeField][Tooltip("Item damageable value * this is cannon range")]float cannonPower;
    [SerializeField] float cannonCooldownTime;
    [SerializeField][ReadOnly] bool cannonLoaded;
    [SerializeField][ReadOnly] bool cannonOnCooldown;
    [SerializeField][ReadOnly] InventoryItemData CannonItemData;
    [SerializeField][ReadOnly] GameObject ItemInCannon;
    [SerializeField] Transform CannonAmmoLoc;

    public void PutGameObjInCannon(InventoryItemData data)
    {
        if (data != null)
        {
            ItemInCannon = Instantiate(data.ItemPrefab, CannonAmmoLoc.position, CannonAmmoLoc.parent.rotation);
            ItemInCannon.transform.parent = CannonAmmoLoc;
            
            if (ItemInCannon.TryGetComponent(out PickupInteractable pi))
            {
                pi.DisableRB();
                pi.SetHeldInHand(true);
            }
        }
        else
        {
            Debug.LogWarning("Passed data was null; could not fill cannon with item");
        }
    }
    public void FireCannon()
    {
        if (ItemInCannon != null && cannonLoaded)
        {
            StartCoroutine(CoolDown(cannonCooldownTime));

            _inventorySystem.RemoveFromInventory(_inventorySystem.GetInventoryItemList()[0], 999, true, out InventoryItemData tempData, out _);

            GameObject instantiated = Instantiate(tempData.ItemPrefab, CannonAmmoLoc.position, CannonAmmoLoc.parent.rotation);
            instantiated.transform.parent = null;

            PickupInteractable pi = instantiated.GetComponent<PickupInteractable>();
            if (pi != null)
            {
                pi.EnableRB();
                pi.SetHeldInHand(false);
            }
            Destroy(ItemInCannon);
            ItemInCannon = null;
            cannonLoaded = false;

            Rigidbody rb = instantiated.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddForce(CannonAmmoLoc.transform.forward * cannonPower * tempData.DamageableValue, ForceMode.Impulse);
            }
            //GetComponent<Rigidbody>().AddForce(-transform.forward * cannonPower / 2); //recoil
            //GetComponent<Rigidbody>().AddForce(transform.up * cannonPower / 2); //recoil
        }
        else
        {
            print("something was null");
        }
    }
    public void PopItemFromCannon()
    {
        if (ItemInCannon != null)
        {
            _inventorySystem.RemoveFromInventory(_inventorySystem.GetInventoryItemList()[0], 999, true, out InventoryItemData temp, out _);

            GameObject instantiated = Instantiate(temp.ItemPrefab, CannonAmmoLoc.position, Quaternion.identity);
            instantiated.transform.parent = null;      

            PickupInteractable pi = instantiated.GetComponent<PickupInteractable>();
            if (pi != null)
            {
                pi.EnableRB();
                pi.SetHeldInHand(false);
            }
            Destroy(ItemInCannon);
            ItemInCannon = null;
        }
        else
        {
            print("something was null");
        }
    }
    private IEnumerator CoolDown(float time)
    {
        cannonOnCooldown = true;
        yield return new WaitForSeconds(time);
        cannonOnCooldown = false;
    }

    public void Interact(GameObject go)
    {
        var playerHands = go.GetComponent<Hands>();

        if (cannonLoaded)
        {
            FireCannon();
        }
        else
            {
                playerHands.DropObject(playerHands.GetTargetedHand());
            }
    }
    public void DisplayInteractUI()
    {
        CanvasInteractionBehavior.ShowInteractUI?.Invoke(cannonLoaded ? "Fire cannon [Click]" : "Load cannon [Click]");
    }
    public void HideInteractUI()
    {
        CanvasInteractionBehavior.HideInteractUI?.Invoke();
    }

    public override void HandlePickup(Collider collidedObject)
    {
        base.HandlePickup(collidedObject);
        if(!cannonOnCooldown && collidedObject.gameObject.TryGetComponent(out PickupInteractable pi))
        {
            if (!cannonLoaded)
            {
                _inventorySystem.AddToInventory(pi.GetItem(), 1, out _);
                CannonItemData = _inventorySystem.GetInventoryItemList()[0];
                PutGameObjInCannon(pi.GetItem());
                Destroy(collidedObject.gameObject);
                cannonLoaded = true;
            }
            else
            {
                PopItemFromCannon();
                _inventorySystem.AddToInventory(pi.GetItem(), 1, out _);
                CannonItemData = _inventorySystem.GetInventoryItemList()[0];
                PutGameObjInCannon(pi.GetItem());
                Destroy(collidedObject.gameObject);
            }
        }
    }
}
