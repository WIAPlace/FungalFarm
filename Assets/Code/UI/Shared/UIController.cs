using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles keyboard input to toggle individual windows or close all windows.
/// Uses inline InputActions — no .inputactions asset required.
/// </summary>
public class UIController : MonoBehaviour
{
    [SerializeField] private WindowManager _windowManager;
    [SerializeField] private InventoryWindow _inventoryWindow;
    [SerializeField] private InventoryWindow _EquipmentWindow;

    [SerializeField] private ItemsDataBase[] startingContainers;

    [SerializeField] public static List<ItemsDataBase> Containers;
    //public Dictionary<string,> containers;

    /// <summary>
    /// Creates the default inventory and equipment windows at their initial positions.
    /// </summary>
    private void Start()
    {
        // make an empty instance of containers
        Containers = new List<ItemsDataBase>();


        var inventoryWindow = _windowManager.CreateWindow(startingContainers[0], "INVENTORY", new Vector2(50, 50));
        _inventoryWindow.BuildInventory(inventoryWindow.ContentArea, startingContainers[0]);
        Containers.Add(startingContainers[0]);
        
        var equipmentWindow = _windowManager.CreateWindow(startingContainers[1], "EQUIPMENT", new Vector2(50, 50));
        _EquipmentWindow.BuildInventory(equipmentWindow.ContentArea,startingContainers[1]);
        Containers.Add(startingContainers[1]);

        ToggleWindow(startingContainers[0]);
        ToggleWindow(startingContainers[1]);
    }

    public void ToggleWindow(ItemsDataBase windowID)
    {
        _windowManager.ToggleWindow(windowID);
        windowID.open = !windowID.open; // swaps its status.
    }
    
    public void CloseAll()
    {
        if (ItemDragManipulator.IsDragging) return;
        _windowManager.CloseAllWindows();
    }


    void OnDestroy()
    {
        foreach(ItemsDataBase items in Containers)
        {   // empty them before runtime end.
            items.items.Clear();
        }
    }


    public static bool PlaceInOpenContainer(ItemsDataBase currentContainer, Item item)
    {
        // 0 will be changed to 1 when we have the hotbar, because we want that to be the last thing to put stuff into.
        for(int i = 0; i<Containers.Count; i++)
        {
            if(currentContainer == Containers[i]) continue;
            if(Containers[i].open && Containers[i].items.TryAdd(item)) return true;
        }
        if(Containers[0].items.TryAdd(item)) return true; // will be hotbar

        return false;
    }
}
