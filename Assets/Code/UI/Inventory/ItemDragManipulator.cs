using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;

public class ItemDragManipulator : PointerManipulator
{
    // Ghost: shared across all slots (only one drag at a time)
    private static VisualElement _ghost;
    private static Image _ghostIcon;
    private static VisualElement _ghostRarity;
    private static string _currentGhostRarityClass;

    // Hovering for splitting items Variables 
    // variables are static because there should only be one happening at any time
    private static List<InventorySlot> _hoveredSlots = new List<InventorySlot>();
    private static bool _isRightClickDragging = false;
    private static int _totalDraggingQuantity = 0;

    //private static ObservableArray<Item> Items => InventorySlot.Items;

    public static bool IsDragging { get; private set; }

    private InventorySlot _sourceSlot;
    private Item _draggedItem;
    private int _capturedPointerId;

    private InventorySlot _highlightedSlot;

    public ItemDragManipulator(InventorySlot slot)
    {
        target = slot;
    }

    // --- Ghost Setup (pre-built, we'll discuss on camera) ---

    // Build the ghost once and park it on the panel root so it can float over every slot.
    public static void InitGhost(VisualElement panelRoot, StyleSheet ghostStyleSheet)
    {
        _ghost = new VisualElement();
        _ghost.name = "drag-ghost";
        _ghost.AddToClassList("drag-ghost");
        // ignore picking so the ghost never steals events from slots underneath
        _ghost.pickingMode = PickingMode.Ignore;

        if (ghostStyleSheet != null)
        {
            _ghost.styleSheets.Add(ghostStyleSheet);
        }

        _ghostIcon = new Image();
        _ghostIcon.AddToClassList("drag-ghost-icon");
        _ghostIcon.pickingMode = PickingMode.Ignore;
        _ghost.Add(_ghostIcon);

        _ghostRarity = new VisualElement();
        _ghostRarity.AddToClassList("drag-ghost-rarity");
        _ghostRarity.pickingMode = PickingMode.Ignore;
        _ghost.Add(_ghostRarity);

        panelRoot.Add(_ghost);
    }

    private void ShowGhost(Item item, Vector2 position)
    {
        _ghostIcon.sprite = item.Icon;

        // Apply rarity class so the ghost mirrors the slot's appearance
        _currentGhostRarityClass = item.RarityClass;
        
        if (!string.IsNullOrEmpty(_currentGhostRarityClass))
            _ghostRarity.AddToClassList(_currentGhostRarityClass);

        // -28 centers the 56px ghost on the cursor
        _ghost.style.translate = new Translate(position.x - 28, position.y - 28);
        _ghost.style.display = DisplayStyle.Flex;
    }

    private void UpdateGhostPosition(Vector2 position)
    {
        _ghost.style.translate = new Translate(position.x - 28, position.y - 28);
    }

    private static void HideGhost()
    {
        _ghost.style.display = DisplayStyle.None;
        _ghostIcon.sprite = null;

        if (!string.IsNullOrEmpty(_currentGhostRarityClass))
        {
            _ghostRarity.RemoveFromClassList(_currentGhostRarityClass);
            _currentGhostRarityClass = null;
        }
    }

    private void ClearHighlight()
    {
        if (_highlightedSlot != null)
        {
            _highlightedSlot.SetDropHighlight(false);
            _highlightedSlot = null;
        }
    }

    // Put the item back where it came from and tear down drag state.
    private void CancelDrag()
    {
        _sourceSlot.HoldItem(_draggedItem);
        _sourceSlot.RemoveFromClassList("drag-active");
        ClearHighlight();

        IsDragging = false;
        _draggedItem = null;
        _sourceSlot = null;

        HideGhost();
        target.ReleasePointer(_capturedPointerId);
    }

    // --- Callback Registration ---

    // KeyDown is in the mix so Escape can bail out mid-drag (target is focused on pickup)
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    // --- Event Handler Stubs (we'll fill these in on camera) ---

    private void OnPointerDown(PointerDownEvent evt)
    {
        // middle-click during drag: cancel
        if (evt.button == 2 && IsDragging)
        {
            CancelDrag();
            evt.StopPropagation();
            return;
        }

        if (evt.button == 2) return;

        //////////////////////////////////////////////////////////[if is dragging and right key down]
        if(evt.button == 1 && IsDragging && _draggedItem != null)
        {
            _isRightClickDragging = true;
            _hoveredSlots.Clear();
            _totalDraggingQuantity = _draggedItem.quantity;

            InventorySlot startingSlot = FindSlotUnderPointer(evt.position);
            // will nee to be changed to effect ones of the same, but with higher quantity, to add to
            if (startingSlot != null && (startingSlot.item==null || startingSlot.item.quantity <= 0))
            {
                _hoveredSlots.Add(startingSlot);
                DistributItems();
            }
            target.CapturePointer(evt.pointerId);
            evt.StopPropagation();
            return;
        }

        var slot = (InventorySlot)target;
        if (slot.item == null) return;

       // depending on the amount of these we should probably speerate them into switch statments and functions.
        if (evt.button == 0 && evt.shiftKey) // [if left click and Shift key down]
        {
            _sourceSlot = slot; // get original slot incase this fails,
            _draggedItem = slot.DropItem(); // drop item out of data index.

            if (UIController.PlaceInOpenContainer(slot.DB_Refrence, _draggedItem))
            {
                evt.StopPropagation();
                return;
            } 
            else
            {
                _sourceSlot.HoldItem(_draggedItem);
                evt.StopPropagation();
                return;
            }  
        }

        // pull the item off the slot up front so the source visually empties immediately
        IsDragging = true;
        _sourceSlot = slot;
        _draggedItem = slot.DropItem();
        _sourceSlot.AddToClassList("drag-active");

        ShowGhost(_draggedItem, evt.position);

        // capture so we keep getting move/up even when the pointer leaves the slot
        target.CapturePointer(evt.pointerId);
        _capturedPointerId = evt.pointerId;
        // focus the slot so KeyDown (Escape) routes here
        target.Focus();
        evt.StopPropagation();
    }

    private InventorySlot FindSlotUnderPointer(Vector2 position)
   {
       // Pick can land on a child (icon, label) — walk up to the slot itself
       var picked = target.panel.Pick(position);

       var current = picked;
       while (current != null)
       {
           if (current is InventorySlot slot)
               return slot;
           current = current.parent;
       }

       return null;
   }

    private void OnPointerMove(PointerMoveEvent evt)
    {
       if (!IsDragging) return;

       UpdateGhostPosition(evt.position);

        // Highlight the slot under the cursor
        var slotUnderPointer = FindSlotUnderPointer(evt.position);

        if (slotUnderPointer != _highlightedSlot)
        {
            ClearHighlight();

            if (slotUnderPointer != null && slotUnderPointer != _sourceSlot)
            {
                slotUnderPointer.SetDropHighlight(true);
                _highlightedSlot = slotUnderPointer;
            }
        }

        if(!_isRightClickDragging || !target.HasPointerCapture(evt.pointerId)) {
            evt.StopPropagation();
            return;
        }

        // Again will need to implement change for if its able to be stacked on top of instead of just quantity 0
        if(slotUnderPointer!=null && 
        !_hoveredSlots.Contains(slotUnderPointer) && 
        (slotUnderPointer.item==null || slotUnderPointer.item.quantity==0))
        {
            _hoveredSlots.Add(slotUnderPointer);
            DistributItems();
        }


       evt.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if(_isRightClickDragging && target.HasPointerCapture(evt.pointerId)) // [was right click dragging]
        {
            target.ReleasePointer(evt.pointerId);
            _isRightClickDragging = false;

            _sourceSlot.RemoveFromClassList("drag-active");
            ClearHighlight();

            IsDragging = false;
            _draggedItem = null;
            _sourceSlot = null;

            HideGhost();

            _hoveredSlots.Clear();
            evt.StopPropagation();
            return;
        }

       if (!IsDragging || evt.button == 2) return;

       var targetSlot = FindSlotUnderPointer(evt.position);

       if (targetSlot != null && targetSlot != _sourceSlot)
       {
           if (targetSlot.item != null)
           {
                if(IfSameAndLess(targetSlot))
                {
                    int remainder = targetSlot.ChangeAmt(_draggedItem.quantity);
                    targetSlot.UpdateAmt(); // this is updating the quantity in both the visual and data at once, may want to move that to be handled by data itself.
                    if(remainder > 0)
                    {
                        TryAddRemainderItem(targetSlot,remainder);
                    }
                }
                else
                {
                    // Swap: pull the target's item out before placing ours, then send it back to source
                    var swappedItem = targetSlot.DropItem();
                    targetSlot.HoldItem(_draggedItem);
                    _sourceSlot.HoldItem(swappedItem);
                }
           }
           else
           {
               // Place
               targetSlot.HoldItem(_draggedItem);
           }

           _sourceSlot.RemoveFromClassList("drag-active");
           ClearHighlight();

           IsDragging = false;
           _draggedItem = null;
           _sourceSlot = null;

           HideGhost();
           target.ReleasePointer(evt.pointerId);
       }
       else
       {
           // No valid target, or dropped on source: cancel
           ClearHighlight();
           CancelDrag();
       }
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        if (!IsDragging) return;

        if (evt.keyCode == KeyCode.Escape)
        {
            CancelDrag();
            evt.StopPropagation();
        }
    }

    private void DistributItems()
    {
        if(_hoveredSlots.Count == 0) return;
        int baseAmount = _totalDraggingQuantity / _hoveredSlots.Count;
        int remainder = _totalDraggingQuantity % _hoveredSlots.Count;

        if(baseAmount == 0) return;
        
        for(int i = 0; i < _hoveredSlots.Count; i++)
        {
            // Give the remainder items to the first few slots
            int amountForThisSlot = baseAmount + (i<remainder?1:0);

            Item tempItem = new(_draggedItem.itemData);
            _hoveredSlots[i].HoldItem(tempItem);
            _hoveredSlots[i].item.quantity = amountForThisSlot;
            _hoveredSlots[i].UpdateAmt();
        }
    }

    private void TryAddRemainderItem(InventorySlot targetSlot, int remainder)
    {
        Item remItem = new(targetSlot.item.itemData,remainder);
        if(targetSlot.AddItemToDB(remItem)) return;
        else _sourceSlot.HoldItem(remItem);
    }

    private bool IfSameAndLess(InventorySlot targetSlot)
    {
        return targetSlot.item.dataId == _draggedItem.dataId && targetSlot.item.quantity < targetSlot.item.itemData.StackAmt;
    }
}
