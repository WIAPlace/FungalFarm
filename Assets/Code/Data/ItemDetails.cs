using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemDetails : ScriptableObject
{
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public SerializableGuid Id = SerializableGuid.NewGuid();

    private void AssignNewGuid() {
        Id = SerializableGuid.NewGuid();
    }

    public ItemCategory Category;
    public ItemRarity Rarity;
    public int StackAmt;

    public string RarityClass => Rarity == ItemRarity.Common
        ? ""
        : $"rarity-{Rarity.ToString().ToLower()}";

    public Item Create(int quantity) {
        return new Item(this, quantity);
    }

}