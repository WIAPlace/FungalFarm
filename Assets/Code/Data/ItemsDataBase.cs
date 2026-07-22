using System;
using UnityEngine;

// Data Holder for all items in a level
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Items Data Base")]
[Serializable]
public class ItemsDataBase : ScriptableObject
{
    //public ObservableArray<Item> items = new(25);
    public SerializableGuid ID = SerializableGuid.NewGuid();
    public ObservableItemArray items = new(25); 
}
