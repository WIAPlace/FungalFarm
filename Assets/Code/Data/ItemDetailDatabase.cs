using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Runtime.InteropServices;

public static class ItemDetailsDatabase {
    static Dictionary<SerializableGuid, ItemDetails> itemDetailsDictionary;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize() {
        itemDetailsDictionary = new Dictionary<SerializableGuid, ItemDetails>();

        Addressables.LoadAssetsAsync<ItemDetails>("ItemDetails",null).Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded){
                var itemDetails = handle.Result;
                if(itemDetails == null) return;
                foreach (var item in itemDetails) {
                    itemDetailsDictionary.Add(item.Id, item);
                }
            }
        };
        
    }

    public static ItemDetails GetDetailsById(SerializableGuid id) {
        try {
            return itemDetailsDictionary[id];
        } catch {
            Debug.LogError($"Cannot find item details with id {id}");
            return null;
        }
    }
}
