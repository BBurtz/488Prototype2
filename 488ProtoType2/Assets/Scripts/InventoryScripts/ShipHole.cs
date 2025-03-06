using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShipHole : InventoryHolder, IInteractable
{
    [SerializeField] Transform PatchLoc;
    [SerializeField] float patchCoolDown;
    [SerializeField][ReadOnly] bool isFlowing;
    [SerializeField][ReadOnly] bool patchOnCoolDown;
    [SerializeField][ReadOnly] InventoryItemData HoleItemData;
    [SerializeField][ReadOnly] GameObject HoleObj;
    public void Start()
    {
        patchOnCoolDown = false;
    }
    public bool GetIsFlowing()
    { 
        return isFlowing; 
    }
    public void PutPatchOnCooldown()
    {
        PopItemFromHole();
    }

    private void PutGameObjInHole()
    {
        if (HoleItemData != null)
        {
            HoleObj = Instantiate(HoleItemData.ItemPrefab, PatchLoc.position, Quaternion.identity);
            HoleObj.transform.parent = PatchLoc;
            HoleItemData = HoleObj.GetComponent<PickupInteractable>().GetItem();
            if (HoleObj.TryGetComponent(out PickupInteractable pi))
            {
                pi.DisableRB();
                pi.SetHeldInHand(true);
            }
            StopLeakForTime(HoleItemData.RepairableValue);
        }
        else
        {
            Debug.LogWarning("Passed data was null; could not fill ship hole with item");
        }
    }
    public void PopItemFromHole()
    {
        if(HoleObj != null && HoleItemData != null)
        {
            _inventorySystem.RemoveFromInventory(_inventorySystem.GetInventoryItemList()[0], 999, true,  out InventoryItemData temp, out _);

            GameObject instantiated = Instantiate(temp.ItemPrefab, PatchLoc.position, Quaternion.identity);
            instantiated.transform.parent = null;

            PickupInteractable pi =instantiated.GetComponent<PickupInteractable>();
            if (pi != null)
            {
                pi.EnableRB();
                pi.SetHeldInHand(false);
            }
            HoleItemData = null;
            Destroy(HoleObj);
            HoleObj = null;
            StartCoroutine(CoolDown(patchCoolDown));
        }
        else
        {
            print("something was null");
        }
    }
    public void StopLeakForTime(float time)
    {
        StartCoroutine(WaitToResumeLeak(time));
    }
    public void ResumeLeakEarly()
    {
        StopCoroutine(WaitToResumeLeak(0));
    }
    private IEnumerator WaitToResumeLeak(float time)
    {
        isFlowing = false;
        yield return new WaitForSeconds(time);
        isFlowing = true;
        PopItemFromHole();
    }
    private IEnumerator CoolDown(float time)
    {
        yield return new WaitForSeconds(time);
        patchOnCoolDown = false;
    }




    public void Interact(GameObject go)
    {
        var temp = go.GetComponent<Hands>();
        temp.DropObject(temp.GetTargetedHand());
    }
    public void DisplayInteractUI()
    {
        CanvasInteractionBehavior.ShowInteractUI?.Invoke("Patch Hole [Click]");
    }
    public void HideInteractUI()
    {
        CanvasInteractionBehavior.HideInteractUI?.Invoke();
    }
    public override void HandlePickup(Collider collidedObject)
    {
        base.HandlePickup(collidedObject);
        if (!patchOnCoolDown  && collidedObject.gameObject.TryGetComponent(out PickupInteractable pi))
        {
            _inventorySystem.AddToInventory(pi.GetItem(), 1, out _);
            Destroy(collidedObject.gameObject);
            HoleItemData = _inventorySystem.GetInventoryItemList()[0];
            patchOnCoolDown = true;
            PutGameObjInHole();
        }
    }
}
