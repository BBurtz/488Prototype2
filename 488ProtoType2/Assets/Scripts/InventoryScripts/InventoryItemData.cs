using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Scriptable Objects/InventoryItemData")]
public class InventoryItemData : ScriptableObject
{
    public Sprite Icon;
    public int MaxStackSize;
    public string DisplayName;
    public bool DoesNotPersist;
    [TextArea(4, 4)] public string DisplayDescription;
}
