using UnityEngine;

public class BasketMerchant : MonoBehaviour
{
    [field:SerializeReference] public ItemsDataBase db;
    [field:SerializeField] public ItemsDataBase boughtItems;
    [field:SerializeField] public Money money;

    void OnDisable()
    {
        SellItems();
    }
    
    void OnEnable()
    {
        FillBasket();    
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

    public void AddItems(ItemDetails item,int quantity)
    {
        Item newItem = item.Create(quantity);
        boughtItems.items.TryAdd(newItem);
    }

    public void FillBasket()
    {
        if (boughtItems != null && boughtItems.items.items != null)
        {
            foreach(Item item in boughtItems.items.items)
            {
                if(item==null||item.quantity<=0||item.itemData==null||item.dataId == SerializableGuid.Empty) continue;
                db.items.TryAdd(item);
            }
            boughtItems.items.Clear();
        }
    }


}
