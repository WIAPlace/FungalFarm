using System;
using UnityEngine.UIElements;

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

    public ObservableArray<Item> Items => itemsDB.items;

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
    }

    public void HoldItem(Item item)
    {
        if (item == null) 
        {
            return;
        }

        ClearSlot();

        this.item = item;
        _icon.style.backgroundImage = new StyleBackground(item.Icon);

        UpdateAmt();
        UpdateItemToIndex();
        Items.TryAddAt(index, item);

        _rarityClass = item.RarityClass;
        _slotRoot.AddToClassList(_rarityClass);

    }

    public Item DropItem()
    {
        if (item == null) 
        {
            return null;
        }

        var droppedItem = item;
        item = null;

        ClearSlot();

        return droppedItem;
    }

    private void ClearSlot()
    {
        _icon.style.backgroundImage = StyleKeyword.None;

        _slotRoot.RemoveFromClassList(_rarityClass);
        _rarityClass = "";
        
        UpdateAmt();

        Items.TryRemoveAt(index);
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
}
