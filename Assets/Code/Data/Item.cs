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

    public Item(ItemData details, int quantity = 1)
    {
        Id = SerializableGuid.NewGuid();
        itemData = details;
        //dataId = details.Id;
        
        this.quantity = quantity;
    }
}
