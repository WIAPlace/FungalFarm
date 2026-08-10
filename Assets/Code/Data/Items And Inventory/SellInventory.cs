using UnityEngine;

public class SellInventory : MonoBehaviour
{
    [field:SerializeReference] public ItemsDataBase db;
    [field:SerializeField] public Money money;

    void OnDisable()
    {
        SellItems();
    }
    public void SellItems()
    {
        if(db==null && db.items.Count < 0) return;

        foreach(Item item in db.items.items)
        {
            if(item==null||item.quantity<=0||item.itemData==null||item.dataId == SerializableGuid.Empty) continue;
            int bonus = item.itemData.price * item.quantity;
            money.Amt+=bonus;
        }

        db.items.Clear();
    }
}
