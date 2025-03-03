using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Lootbox : MonoBehaviour, IInteractable
{

    //put this on every lootbox (duh)

    private InventoryHolder inventoryHolder;
    public InventoryItemData data;

    [Tooltip("What are the objects that you can get from this box?")]
    public List<ScriptableObject> ItemsInCrate = new List<ScriptableObject>();

    [Tooltip("DON'T TOUCH THIS.")]
    public ScriptableObject Item;

    private void Start()
    {

        inventoryHolder = GetComponent<InventoryHolder>();
        Randomization();

    }

    void Randomization()
    {

        int randomItem = Random.Range(0, ItemsInCrate.Count);

        Item = ItemsInCrate[randomItem];

        //inventoryHolder.InventorySystem.AddToInventory(Item.GetComponent<InventoryItemData>(), 1, out _);

        //Debug.Log(item);

    }

    public void AddToInventory()
    {

        inventoryHolder.InventorySystem.AddToInventory(Item.GetComponent<InventoryItemData>(), 1, out _);

    }
    

}
