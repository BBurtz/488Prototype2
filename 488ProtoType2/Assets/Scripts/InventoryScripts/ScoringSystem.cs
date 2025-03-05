using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoringSystem : InventoryHolder
{
    public int score;
    public TMPro.TMP_Text ScoringText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool HandlePickup(Collider collidedObject)
    {
        bool blah = base.HandlePickup(collidedObject);
        if(blah)
        {
            UpdateScore();
        }
        return true;
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
}
