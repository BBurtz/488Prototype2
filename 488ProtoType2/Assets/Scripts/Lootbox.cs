using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Lootbox : MonoBehaviour, IInteractable
{

    //put this on every lootbox (duh)
    bool isOpen = false;

    [Tooltip("Put the three slot prefabs here.")]
    public List<GameObject> slots = new List<GameObject>();

    [Tooltip("Drop all of the list prefabs here.")]
    public List<GameObject> ListsOfItems = new List<GameObject>();

    List<InventoryItemData> itemsFromLists = new List<InventoryItemData>();

    private GameObject instantiatedObj;

    //don't worry about these
    bool treasureAlreadyActive = false;
    bool treasureAlreadyActiveAgain = false;

    bool toolsAlreadyActive = false;
    bool toolsAlreadyActiveAgain = false;

    [SerializeField] Transform ChestPos;

    public void Interact(GameObject player)
    {
        if (!isOpen)
        {
            isOpen = true;

            StopCoroutine(WaitForPickup());

            StartCoroutine(WaitForPickup());

            AudioManager.instance.PlayOneShot(FMODEvents.instance.ChestOpen, this.transform.position);

            for (int i = 0; i < slots.Count; i++)
            {

                if (slots[i].GetComponent<Slots>().xActive == true)
                {

                    slots[i].GetComponent<Animator>().SetTrigger("X");

                }
                if (slots[i].GetComponent<Slots>().treasureActive == true)
                {

                    slots[i].GetComponent<Animator>().SetTrigger("Treasure");

                    if (treasureAlreadyActive == false && treasureAlreadyActiveAgain == false)
                    {

                        for (int integer = 0; integer < ListsOfItems[0].GetComponent<Lists>().Items.Count; integer++)
                        {

                            itemsFromLists.Add(ListsOfItems[0].GetComponent<Lists>().Items[integer]);

                        }

                        treasureAlreadyActive = true;

                    }
                    else if (treasureAlreadyActive == true && treasureAlreadyActiveAgain == false)
                    {

                        for (int integer = 0; integer < ListsOfItems[1].GetComponent<Lists>().Items.Count; integer++)
                        {

                            itemsFromLists.Add(ListsOfItems[1].GetComponent<Lists>().Items[integer]);

                        }

                        treasureAlreadyActiveAgain = true;

                    }
                    else if (treasureAlreadyActive == true && treasureAlreadyActiveAgain == true)
                    {

                        itemsFromLists.Clear();

                        for (int integer = 0; integer < ListsOfItems[2].GetComponent<Lists>().Items.Count; integer++)
                        {

                            itemsFromLists.Add(ListsOfItems[2].GetComponent<Lists>().Items[integer]);
                        }

                    }

                }
                if (slots[i].GetComponent<Slots>().toolsActive == true)
                {

                    slots[i].GetComponent<Animator>().SetTrigger("Tools");

                    if (toolsAlreadyActive == false && toolsAlreadyActiveAgain == false)
                    {

                        for (int integer = 0; integer < ListsOfItems[3].GetComponent<Lists>().Items.Count; integer++)
                        {

                            itemsFromLists.Add(ListsOfItems[3].GetComponent<Lists>().Items[integer]);

                        }

                        toolsAlreadyActive = true;

                    }
                    else if (toolsAlreadyActive == true && toolsAlreadyActiveAgain == false)
                    {

                        for (int integer = 0; integer < ListsOfItems[4].GetComponent<Lists>().Items.Count; integer++)
                        {

                            itemsFromLists.Add(ListsOfItems[4].GetComponent<Lists>().Items[integer]);

                        }

                        toolsAlreadyActiveAgain = true;

                    }
                    else if (toolsAlreadyActive == true && toolsAlreadyActiveAgain == true)
                    {

                        itemsFromLists.Clear();

                        for (int integer = 0; integer < ListsOfItems[5].GetComponent<Lists>().Items.Count; integer++)
                        {

                            itemsFromLists.Add(ListsOfItems[5].GetComponent<Lists>().Items[integer]);

                        }
                    }
                }
            }

            PickRandomItemToInstantiate();

        }
        //player has not picked up item and is trying to roll again
        else if (isOpen && instantiatedObj != null)
        {

            Destroy(instantiatedObj);

            for (int i = 0; i < slots.Count; i++)
            {

                slots[i].GetComponent<Animator>().SetTrigger("Spin");

            }

            isOpen = false;       

            //GetComponent<Animator>().SetTrigger("Close");

        }
        //player picked up the item and is trying to roll again
        else if (isOpen && instantiatedObj == null)
        {

            //player cannot reopen the chest

        }
    }
    private IEnumerator WaitForPickup()
    {
        while (instantiatedObj != null)
        {
            print("NOT NULL");

            yield return new WaitForSeconds(0.1f);

        }

        print("NULLED");

        yield return null;
    }

    void PickRandomItemToInstantiate()
    {

        int randomItem = Random.Range(0, itemsFromLists.Count);

        var tempItem = itemsFromLists[randomItem];

        //GetComponent<Animator>().SetTrigger("Open");

        instantiatedObj = Instantiate(tempItem.ItemPrefab, ChestPos.position, ChestPos.parent.rotation);

        instantiatedObj.transform.parent = ChestPos;

        if (instantiatedObj.TryGetComponent(out PickupInteractable pi))
        {
            pi.SetHeldInHand(true);

            if(instantiatedObj.TryGetComponent(out Rigidbody rb))
            {

                rb.isKinematic = true;

            }
        }
    }

    public void DisplayInteractUI()
    {

        CanvasInteractionBehavior.ShowInteractUI?.Invoke("Pickup: " + gameObject.name + " [Click]");

    }

    public void HideInteractUI()
    {

        CanvasInteractionBehavior.HideInteractUI?.Invoke();
        
    }
}
