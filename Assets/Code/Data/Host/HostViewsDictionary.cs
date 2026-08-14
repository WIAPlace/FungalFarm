using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public static class HostViewsDictionary 
{
    static Dictionary<SerializableGuid, HostView> viewDictionary;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize() {
        viewDictionary = new Dictionary<SerializableGuid, HostView>();

        Addressables.LoadAssetsAsync<GameObject>("Views",null).Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded){
                var views = handle.Result;
                if(views == null) return;
                foreach (var item in views) {
                    
                    if(item.TryGetComponent<HostView>(out HostView temp)){
                        viewDictionary.Add(temp.veiwType, temp);
                    }
                    else
                    {
                        Debug.Log(item.name + " has no HostView");
                    }
                }
            }
        };
        
    }

    public static HostView GetDetailsById(SerializableGuid id) {
        try {
            return viewDictionary[id];
        } catch {
            Debug.LogError($"Cannot find item details with id {id}");
            return null;
        }
    }
}
