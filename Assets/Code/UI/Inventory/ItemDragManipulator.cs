using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
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
    private static List<int> _hoveredSlotsOriginalAmt = new List<int>();
    private static bool _isRightClickDragging = false;
    private static int _totalDraggingQuantity = 0;

    //private static ObservableArray<Item> Items => InventorySlot.Items;

    public static bool IsDragging { get; private set; }

    private InventorySlot _sourceSlot;
    private Item _draggedItem;
    private int _capturedPointerId;

    private InventorySlot _highlightedSlot;

    //////////////////////////////////////////////////////////////////////////////////// Constructor
    public ItemDragManipulator(InventorySlot slot)
    {
        target = slot;
    }

    // --- Ghost Setup (pre-built, we'll discuss on camera) ---
    //////////////////////////////////////////////////////////////////////////////////// Initialize Ghost
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

    //////////////////////////////////////////////////////////////////////////////////// Show Ghost
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

    //////////////////////////////////////////////////////////////////////////////////// Update Ghost Position
    private void UpdateGhostPosition(Vector2 position)
    {
        _ghost.style.translate = new Translate(position.x - 28, position.y - 28);
    }

    //////////////////////////////////////////////////////////////////////////////////// Hide Ghost
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

    //////////////////////////////////////////////////////////////////////////////////// Clear Highlight
    private void ClearHighlight()
    {
        if (_highlightedSlot != null)
        {
            _highlightedSlot.SetDropHighlight(false);
            _highlightedSlot = null;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////// Cancel Drag
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
//////////////////////////////////////////////////////////////////////////////////// Call Backs
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
    //////////////////////////////////////////////////////////////////////////////////// On Pointer Down
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
            _hoveredSlotsOriginalAmt.Clear();
            
            _totalDraggingQuantity = _draggedItem.quantity;

            InventorySlot startingSlot = FindSlotUnderPointer(evt.position);
            // will nee to be changed to effect ones of the same, but with higher quantity, to add to
            if (startingSlot != null && (startingSlot.item==null || 
            (startingSlot.item.itemData == _draggedItem.itemData && startingSlot.item.quantity < startingSlot.item.itemData.StackAmt)))
            {
                _hoveredSlots.Add(startingSlot);
                if(startingSlot.item != null)_hoveredSlotsOriginalAmt.Add(startingSlot.item.quantity);
                else _hoveredSlotsOriginalAmt.Add(0);
                DistributItems();
                target.CapturePointer(evt.pointerId);
            }
            else
            {
                CancelDrag();
                _isRightClickDragging = false;
            }   
            
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

    //////////////////////////////////////////////////////////////////////////////////// Find Slot Under Pointer
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
    //////////////////////////////////////////////////////////////////////////////////// On Pointer Move
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

        // right click dragging
        if(!_isRightClickDragging || !target.HasPointerCapture(evt.pointerId)) {
            evt.StopPropagation();
            return;
        }

        // Again will need to implement change for if its able to be stacked on top of instead of just quantity 0
        if(slotUnderPointer!=null && 
        !_hoveredSlots.Contains(slotUnderPointer) && 
        (slotUnderPointer.item==null || 
        (slotUnderPointer.item.itemData == _draggedItem.itemData && slotUnderPointer.item.quantity <  slotUnderPointer.item.itemData.StackAmt)))
        {
            _hoveredSlots.Add(slotUnderPointer);
            if(slotUnderPointer.item != null)_hoveredSlotsOriginalAmt.Add(slotUnderPointer.item.quantity);
            else _hoveredSlotsOriginalAmt.Add(0);
            DistributItems();
        }


       evt.StopPropagation();
    }

    //////////////////////////////////////////////////////////////////////////////////// On Pointer Up
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

    //////////////////////////////////////////////////////////////////////////////////// On Key Down
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (!IsDragging) return;

        if (evt.keyCode == KeyCode.Escape)
        {
            CancelDrag();
            evt.StopPropagation();
        }
    }  

    //////////////////////////////////////////////////////////////////////////////////// Distribute Items
    private void DistributItems()
    {
        if(_hoveredSlots.Count == 0) return;

        int totalRemaining = _totalDraggingQuantity;
        int[] finalAmounts = new int[_hoveredSlots.Count];

        int baseAmount = totalRemaining / _hoveredSlots.Count;
        int remainder = totalRemaining % _hoveredSlots.Count;

        if(baseAmount == 0) return;

        for(int i = 0; i < _hoveredSlots.Count; i++) // does a first sweep to check for if stuff needs capping
        {
            finalAmounts[i] = _hoveredSlotsOriginalAmt[i] + baseAmount + (i < remainder ? 1 : 0);
        }

        bool needsCapping = true;

        // keep checking through if needs to be redistributed.
        while (needsCapping)
        {
            needsCapping = false;
            int overflow = 0;
            int openSlotCount = 0;

            ItemDetails baseData = _draggedItem.itemData;
            int maxLimit = baseData.StackAmt;

            for(int i=0; i < _hoveredSlots.Count; i++)
            {
                if (finalAmounts[i] > maxLimit)
                {
                    overflow += finalAmounts[i]-maxLimit;
                    finalAmounts[i] = maxLimit;
                }
                if (finalAmounts[i] < maxLimit)
                {
                    openSlotCount++;
                }
            }

            // redistribut overflow items if there is space left items
            if(overflow > 0 && openSlotCount > 0)
            {
                int extraBase = overflow/openSlotCount;
                int extraRem = overflow % openSlotCount;
                int addedSoFar = 0;

                for (int i = 0; i < _hoveredSlots.Count; i++)
                {
                    if (finalAmounts[i] < maxLimit)
                    {
                        finalAmounts[i] += extraBase + (addedSoFar < extraRem ? 1 : 0);
                        addedSoFar++;
                        needsCapping = true; // Re-check in case the new distribution exceeds max again
                    }
                }
            }
        }
        
        // finalize distribution
        for(int i = 0; i < _hoveredSlots.Count; i++)
        {
            Item tempItem = new(_draggedItem.itemData, finalAmounts[i]);
            _hoveredSlots[i].HoldItem(tempItem);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////// Try Add Remainder Items
    private void TryAddRemainderItem(InventorySlot targetSlot, int remainder)
    {
        Item remItem = new(targetSlot.item.itemData,remainder);
        if(targetSlot.AddItemToDB(remItem)) return;
        else _sourceSlot.HoldItem(remItem);
    }

    //////////////////////////////////////////////////////////////////////////////////// [Bool] If Same And Less
    private bool IfSameAndLess(InventorySlot targetSlot)
    {
        return targetSlot.item.dataId == _draggedItem.dataId && targetSlot.item.quantity < targetSlot.item.itemData.StackAmt;
    }
}
