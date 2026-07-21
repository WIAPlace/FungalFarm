using UnityEngine;
using System;

[Serializable]
public class Item
{
    [field: SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    public SerializableGuid dataId => itemData.Id; 
    public ItemData itemData;
    public int quantity = 1 ;
    public Sprite Icon => itemData.Icon;
    public string RarityClass => itemData.RarityClass;

    // x is the indicator for what obArray they are in
    // y is the indicator for placement in that array, the index itself
    public Vector2Int currentIndex = Vector2Int.zero; 

    public Item(ItemData details, int quantity = 1)
    {
        Id = SerializableGuid.NewGuid();
        itemData = details;
        //dataId = details.Id;
        
        this.quantity = quantity;
    }
}
