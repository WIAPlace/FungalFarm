using System.Runtime.InteropServices;
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

    /// <summary>
    /// Creates the default inventory and equipment windows at their initial positions.
    /// </summary>
    private void Start()
    {
        var inventoryWindow = _windowManager.CreateWindow("inventory", "INVENTORY", new Vector2(50, 50));
        _inventoryWindow.BuildInventory(inventoryWindow.ContentArea);
        
        var equipmentWindow = _windowManager.CreateWindow("equipment", "EQUIPMENT", new Vector2(50, 50));
        _EquipmentWindow.BuildInventory(equipmentWindow.ContentArea);

        ToggleWindow("inventory");
        ToggleWindow("equipment");
    }

    public void ToggleWindow(string windowID)
    {
        _windowManager.ToggleWindow(windowID);
    }
    
    public void CloseAll()
    {
        if (ItemDragManipulator.IsDragging) return;
        _windowManager.CloseAllWindows();
    }
}
