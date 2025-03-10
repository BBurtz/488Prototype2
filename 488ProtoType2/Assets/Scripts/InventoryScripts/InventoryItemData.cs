using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Scriptable Objects/InventoryItemData")]
public class InventoryItemData : ScriptableObject
{
    public Sprite DisplaySprite;
    public int MaxStackSize;
    public string DisplayName;
    public float ValuableValue;
    public float RepairableValue;
    public float DamageableValue;
    public GameObject ItemPrefab;
    [TextArea(4, 4)] public string DisplayDescription;
}
