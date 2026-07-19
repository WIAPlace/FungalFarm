using UnityEngine.UIElements;

public class InventorySlot : VisualElement
{
    private string _rarityClass = "";
    private VisualElement _slotRoot;
    private VisualElement _icon;
    private ItemData _item;
    public ItemData Item => _item;

    public InventorySlot(VisualTreeAsset template)
    {

        focusable = true;
        
        _slotRoot = template.Instantiate().ExtractRoot("ItemSlot");
        this.Add(_slotRoot);

        _icon = _slotRoot.Q<VisualElement>("Icon");

        this.AddManipulator(new ItemDragManipulator(this));
        this.AddManipulator(new ItemTooltipManipulator(this));

    }

    public void HoldItem(ItemData item)
    {
        if (item == null) 
        {
            return;
        }

        ClearSlot();

        _item = item;
        _icon.style.backgroundImage = new StyleBackground(item.Icon);


        _rarityClass = item.RarityClass;
        _slotRoot.AddToClassList(_rarityClass);

    }

    public ItemData DropItem()
    {
        if (_item == null) 
        {
            return null;
        }

        var droppedItem = _item;
        _item = null;

        ClearSlot();

        return droppedItem;
    }

    private void ClearSlot()
    {
        _icon.style.backgroundImage = StyleKeyword.None;

        _slotRoot.RemoveFromClassList(_rarityClass);
        _rarityClass = "";
    }

    public void SetDropHighlight(bool active) => _slotRoot.EnableInClassList("drop-target", active);
}
