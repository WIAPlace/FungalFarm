using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Runtime.InteropServices;

public static class ItemDetailsDatabase {
    static Dictionary<SerializableGuid, ItemDetails> itemDetailsDictionary;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize() {
        itemDetailsDictionary = new Dictionary<SerializableGuid, ItemDetails>();

        Addressables.LoadAssetsAsync<ItemDetails>("ItemDetails", null).Completed += handle =>
        {
            var itemDetails = handle.Result;
            foreach (var item in itemDetails) {
                itemDetailsDictionary.Add(item.Id, item);
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
