using System;
using UnityEngine.UIElements;
using UnityEngine;

//[Serializable]

public class InventorySlot : VisualElement
{
    private string _rarityClass = "";
    private VisualElement _slotRoot;
    private VisualElement _icon;
    private Label _amt;
    private ItemsDataBase itemsDB;
    //private ItemData _item;
    public Item item;

    public ObservableItemArray Items => itemsDB.items;
    public ItemsDataBase DB_Refrence => itemsDB;

    public int index;

    public InventorySlot(VisualTreeAsset template)
    {

        focusable = true;
        
        _slotRoot = template.Instantiate().ExtractRoot("ItemSlot");
        this.Add(_slotRoot);

        _icon = _slotRoot.Q<VisualElement>("Icon");
        _amt = _slotRoot.Q<Label>("Amt");
        //_amt.BringToFront();
        
        UpdateAmt();

        this.AddManipulator(new ItemDragManipulator(this));
        this.AddManipulator(new ItemTooltipManipulator(this));

    }

    // used if needs to be indexed
    public InventorySlot(VisualTreeAsset template,int newIndex, ItemsDataBase idb) : this(template)
    {
        index = newIndex;
        itemsDB = idb;
        UpdateItemToIndex();

        Items.AnyValueChanged += HandleOnAnyValueChanged;
    }
    

    public void HoldItem(Item item)
    {
        if (item == null) 
        {
            return;
        }
        //Items.TryRemoveAt(index);
        if(!Items.TryAddAt(index, item)) Debug.Log("Failed at index: "+index);
    }

    public Item DropItem()
    {
        if (item == null) 
        {
            return null;
        }

        var droppedItem = item;

        Items.TryRemoveAt(index);

        return droppedItem;
    }

    private void ClearSlot()
    {
        _icon.style.backgroundImage = StyleKeyword.None;

        _slotRoot.RemoveFromClassList(_rarityClass);
        _rarityClass = "";
        
        UpdateAmt();
    }

    public void UpdateAmt()
    {
        if(item != null){
            _amt.text = item.quantity > 1 ? item.quantity.ToString() : string.Empty;
            _amt.visible = item.quantity > 1;
        }
        else
        {
            _amt.text = "0";
            _amt.visible = false;
        }
    }

    public void UpdateItemToIndex(){
        if(item != null){
            item.currentIndex.y = index;
        }
    }

    public void SetDropHighlight(bool active) => _slotRoot.EnableInClassList("drop-target", active);

    public void UnsubscribeFromEvents()
    {
        Items.AnyValueChanged -= HandleOnAnyValueChanged;
    }

    public void HandleOnAnyValueChanged(Item[] idb, int index) // update the view
    {
        if(index == -1)
        {
            item = null;
            ClearSlot();
        }
        if(this.index != index) return;
        
        if (idb[index] != default(Item) && idb[index] != item && idb[index].quantity > 0)
        {
            item = idb[index];
            ClearSlot();

            if(idb[index].Icon != null){
                _icon.style.backgroundImage = Background.FromSprite(item.Icon);
            }
            else return;
            
            //_icon.style.backgroundImage = new StyleBackground(idb[index].Icon);

            UpdateAmt();
            UpdateItemToIndex();

            _rarityClass = item.RarityClass;
            _slotRoot.AddToClassList(_rarityClass);
        }
        else if(item != null)
        {
            item = null;
            ClearSlot();
        }
        
    }
}
