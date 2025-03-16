using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ScoringSystem : InventoryHolder, IInteractable
{
    public int score;
    public TMPro.TMP_Text ScoringText;
    public GameObject InteractPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        //UpdateScore();
        UpdateText();
    }

    public override void HandlePickup(Collider collidedObject)
    {
        base.HandlePickup(collidedObject);
        if (collidedObject.gameObject.TryGetComponent(out PickupInteractable pi))
        {
            _inventorySystem.AddToInventory(pi.GetItem(), 1, out _);
            Vector3 saveLocation = collidedObject.transform.position; //grabs object location for playing collection sfx
            Destroy(collidedObject.gameObject);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.LootUp, saveLocation);
            UpdateScore();
        }
    }

    public void UpdateScore()
    {
        List<InventoryItemData> Temp = _inventorySystem.GetInventoryItemList();
        score = 0;
        foreach (InventoryItemData item in Temp)
        {
            if (item != null)
            {
                score += (int)item.ValuableValue;
            }
        }
        UpdateText();
    }

    private void UpdateText()
    {
        ScoringText.text = "You Made It Out With $ " + score;
    }

    public void Interact(GameObject go)
    {
        var playerHands = go.GetComponent<Hands>();
        playerHands.DropObject(playerHands.GetTargetedHand());
    }
    public void DisplayInteractUI()
    {
        CanvasInteractionBehavior.ShowInteractUI?.Invoke("Deposit Item [Click]");
    }
    public void HideInteractUI()
    {
        CanvasInteractionBehavior.HideInteractUI?.Invoke();
    }
}
