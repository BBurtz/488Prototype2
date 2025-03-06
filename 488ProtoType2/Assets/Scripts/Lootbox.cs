using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;

public class Lootbox : MonoBehaviour//, IInteractable
{

    //put this on every lootbox (duh)

    //public InventoryItemData data;

    bool isOpen = false;
    bool canOpenAgain = true;

    [Tooltip("What are the objects that you can get from this box?")]
    public List<ScriptableObject> ItemsInCrate = new List<ScriptableObject>();

    [Tooltip("DON'T TOUCH THIS.")]
    public ScriptableObject Item;

    private void Start()
    {

        Randomization();

    }

    void Randomization()
    {

        int randomItem = Random.Range(0, ItemsInCrate.Count);

        Item = ItemsInCrate[randomItem];

        //inventoryHolder.InventorySystem.AddToInventory(Item.GetComponent<InventoryItemData>(), 1, out _);

        //Debug.Log(item);

    }

    public void Interact(GameObject player)
    {

        if (!isOpen)
        {

            isOpen = true;
            Debug.Log("it's open! and it's a " + Item);

        }
        else if (isOpen && canOpenAgain)
        {

            //InventoryHolder myHolder = player.GetComponent<InventoryHolder>();
            //myHolder.InventorySystem.AddToInventory(Item.GetComponent<InventoryItemData>(), 1, out _);
            Randomization();
            canOpenAgain = false;


        }

    }

    void Update()
    {

        //timer

    }
    

}
