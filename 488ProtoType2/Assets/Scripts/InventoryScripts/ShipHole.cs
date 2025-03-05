using UnityEngine;

public class ShipHole : InventoryHolder
{
    public override bool HandlePickup(Collider collidedObject)
    {
        bool addedItem = base.HandlePickup(collidedObject);
        
        if(addedItem)
        {
            var item = base.InventorySystem.GetInventoryItemList()[0];
        }
        return true;
    }
}
