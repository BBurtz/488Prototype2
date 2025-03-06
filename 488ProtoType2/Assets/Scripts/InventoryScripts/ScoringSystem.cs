using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoringSystem : InventoryHolder
{
    public int score;
    public TMPro.TMP_Text ScoringText;
    [SerializeField][ReadOnly] InventoryItemData HoleItemData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void HandlePickup(Collider collidedObject)
    {
        base.HandlePickup(collidedObject);
        if (collidedObject.gameObject.TryGetComponent(out PickupInteractable pi))
        {
            _inventorySystem.AddToInventory(pi.GetItem(), 1, out _);
            Destroy(collidedObject.gameObject);
            UpdateScore();
        }
    }

    public void UpdateScore()
    {
        List<InventoryItemData> Temp =_inventorySystem.GetInventoryItemList();
        score = 0;
        foreach (InventoryItemData item in Temp)
        {
            score += (int)item.ValuableValue;
        }
    }

    public void DisplayInteractUI()
    {
        CanvasInteractionBehavior.ShowInteractUI?.Invoke("Patch Hole [Click]");
    }
    public void HideInteractUI()
    {
        CanvasInteractionBehavior.HideInteractUI?.Invoke();
    }
}
