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
    public bool open = true;

    public void InitializeData(ContainerDataBase cdb)
    {
        if(cdb == null) return;
        
        //items = new(cdb.containerData.Length);
        items.Clear();

        for(int i = 0; i < items.Length; i++)
        {
            //Debug.Log(i);
            ContainerData cd = cdb.containerData[i];
            //Debug.Log(i);
            if(cd.dataId == SerializableGuid.Empty) // if there is no data skip this one.
            {
                items.TryRemoveAt(i);
                continue;
            }

            ItemDetails itemDetails = ItemDetailsDatabase.GetDetailsById(cd.dataId);

            if(cd.quantity <=0)// if there is nothing here we dont need it.
            {
                items.TryRemoveAt(i);
                continue;
            } 

            Item tempItem = new(itemDetails, cd.quantity)
            {
                Id = cd.Id
            };
            items.TryAddAt(i,tempItem);
            
        }

    }
}
