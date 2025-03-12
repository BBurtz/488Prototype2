using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;

public class Lootbox : MonoBehaviour, IInteractable
{

    //put this on every lootbox (duh)

    bool isOpen = false;
    bool canOpenAgain = true;

    [Tooltip("Put the three slot prefabs here.")]
    public List<GameObject> slots = new List<GameObject>();

    [Tooltip("Drop all of the list prefabs here.")]
    public List<ScriptableObject> ListsOfItems = new List<ScriptableObject>();

    List<ScriptableObject> itemsFromLists = new List<ScriptableObject>();

    //don't worry about these

    bool treasureAlreadyActive = false;
    bool treasureAlreadyActiveAgain = false;

    bool toolsAlreadyActive = false;
    bool toolsAlreadyActiveAgain = false;

    [Tooltip("DON'T TOUCH THIS.")]
    public ScriptableObject Item;

    private void Start()
    {


    }

    public void Interact(GameObject player)
    {

        if (!isOpen)
        {

            isOpen = true;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.ChestOpen, this.transform.position);

            for(int i = 0; i < slots.Count; i++)
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

                        for(int integer = 0; integer < ListsOfItems[0].GetComponent<Lists>().Items.Count; integer++)
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

            Randomization();
            Debug.Log("it's open!");

        }
        else if (isOpen && canOpenAgain)
        {

            for (int i = 0; i < slots.Count; i++)
            {

                slots[i].GetComponent<Animator>().SetTrigger("Spin");

            }

            isOpen = false;
            canOpenAgain = false;

        }

    }

    void Randomization()
    {

        int randomItem = Random.Range(0, itemsFromLists.Count);

        Item = itemsFromLists[randomItem];

        Instantiate(Item);

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
