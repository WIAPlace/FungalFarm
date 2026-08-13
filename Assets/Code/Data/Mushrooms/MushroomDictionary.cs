using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Runtime.InteropServices;

public static class MushroomDictionary
{
    static Dictionary<SerializableGuid, MushroomDetails> fungiDictionary;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize() {
        fungiDictionary = new Dictionary<SerializableGuid, MushroomDetails>();

        Addressables.LoadAssetsAsync<MushroomDetails>("Fungi",null).Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded){
                var fungi = handle.Result;
                if(fungi == null) return;
                foreach (var item in fungi) {
                    fungiDictionary.Add(item.Id, item);
                }
            }
        };
        
    }

    public static MushroomDetails GetDetailsById(SerializableGuid id) {
        try {
            return fungiDictionary[id];
        } catch {
            Debug.LogError($"Cannot find item details with id {id}");
            return null;
        }
    }
}
