using UnityEngine;

public class ShipHole : InventoryHolder
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool HandlePickup(Collider collidedObject)
    {
        bool blah = base.HandlePickup(collidedObject);

        return true;
    }
}
