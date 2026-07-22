using UnityEngine;
using System;
using System.Linq;
using System.ComponentModel;

[Serializable] public class SaveData
{
    ///////// Information about saves will go here
    public ItemsDataBase[] containers;
    [HideInInspector] public ContainerDataBase[] SavedContainerData;
}

[Serializable] public class ContainerData // represents one slot of an inventory
{
    public SerializableGuid Id;
    public SerializableGuid dataId;
    //public int indexSlot;
    public int quantity;

    public ContainerData()
    {
        Id = SerializableGuid.Empty;
        dataId = SerializableGuid.Empty;
        quantity = 0;
    }
    public ContainerData(Item item) 
    {
        if(item == null || item.quantity <= 0) return; // if there is not item make it empty

        Id = item.Id;
        dataId = item.dataId;
        quantity = item.quantity;
    }
}
[Serializable] public class ContainerDataBase // represents an inventory
{
    public ContainerData[] containerData;

    // constructor
    public ContainerDataBase()
    {
        containerData = null;
    }
    public ContainerDataBase(ItemsDataBase idb)
    {
        if(idb == null) return;
        containerData = new ContainerData[idb.items.Length];

        for(int i=0;i < idb.items.Length; i++)
        {
            containerData[i] = new(idb.items.items[i]);
        } 
    }
}

[DefaultExecutionOrder(-100)] 
public class SaveManager : PersistentSingleton<SaveManager>
{
    [SerializeField] public SaveData saveData;

    protected override void Awake()
    {
        base.Awake();
        SaveSystem.Init();
        //ItemDetailsDatabase.Initialize();
    }

    public void OnSceneLoad()
    {
        // some stuff to stop this from occuring on menus

    }


    public void SaveGameData()
    {
        if(saveData == null ) return;

        // new array at the length of the containers one
        saveData.SavedContainerData = new ContainerDataBase[saveData.containers.Length];

        // iterate through to save just the needed stuff
        for(int i = 0; i < saveData.containers.Length; i++)
        {
            saveData.SavedContainerData[i] = new(saveData.containers[i]);
        }

        string dataString = JsonUtility.ToJson(saveData);
        // save settings data
        SaveSystem.QuickSave(dataString);
    }

    public void LoadGameData()
    {
        string loadedData = SaveSystem.QuickLoad();
        if (loadedData != null)
        {
            ItemsDataBase[] containers = saveData.containers;
            saveData = JsonUtility.FromJson<SaveData>(loadedData);
            saveData.containers = containers;
        }
        if(saveData.SavedContainerData==null) return;

        for(int i = 0; i<saveData.containers.Length; i++)
        {
            if(saveData.SavedContainerData[i]!=null) {
                saveData.containers[i].InitializeData
                    (saveData.SavedContainerData[i]);
            }
        }

    }
}
