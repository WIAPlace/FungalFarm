using UnityEngine;

// Data Holder for all items in a level
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Items Data Base")]
public class ItemsDataBase : ScriptableObject
{
    //public ObservableArray<Item> items = new(25);
    public ObservableItemArray items = new(25); 
}
