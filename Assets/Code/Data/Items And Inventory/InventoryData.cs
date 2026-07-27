using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Inventory Data")]
public class InventoryData : ScriptableObject
{
    public ObservableArray<InventorySlot> _slots = new();
}
