using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class InventorySlot : L2DraggableSlot
{
    protected L2Tab _currentTab;
    private int _count;
    private long _remainingTime;
    private SlotClickSoundManipulator _slotClickSoundManipulator;
    private int _objectId;
    private ItemCategory _itemCategory;
    protected bool _empty = true;
    public int Count { get { return _count; } }
    public long RemainingTime { get { return _remainingTime; } }
    public ItemCategory ItemCategory { get { return _itemCategory; } }
    public int ObjectId { get { return _objectId; } }
    public int ItemId { get { return _id; } }

    public InventorySlot(int position, VisualElement slotElement, L2Tab tab, SlotType slotType)
    : base(position, slotElement, slotType, false, true)
    {
        _currentTab = tab;
        _empty = true;

        if (_slotClickSoundManipulator == null)
        {
            _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
            _slotElement.AddManipulator(_slotClickSoundManipulator);
        }
    }

    public InventorySlot(int position, VisualElement slotElement, SlotType slotType)
    : base(position, slotElement, slotType, true, false)
    {
        _empty = true;
    }
 
    public void AssignEmpty()
    {
        //_slotElement.AddToClassList("empty");
        _empty = true;
        _id = 0;
        _name = "Unkown";
        _description = "Unkown item.";

        if (_slotElement != null)
        {
            StyleBackground background = new StyleBackground(IconManager.Instance.GetInvetoryDefaultBackground());
            _slotBg.style.backgroundImage = background;
            _slotDragManipulator.enabled = false;
        }
    }
    public void RefreshPosition(int newPosition)
    {
        Position = newPosition;
    }

    public Product product;
    public void AssignProduct(Product item)
    {
        _slotElement.RemoveFromClassList("empty");
        

        if (item != null)
        {
            _id = item.ItemId;
            _empty = false;
        }
        else
        {
            _id = 0;
            _name = "Unkown";
            _description = "Unkown item.";
            _empty = true;
        }

        if (_slotElement != null)
        { 
            StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
            _slotBg.style.backgroundImage = background;
            _slotDragManipulator.enabled = false;
        }

        product = item;
    }

    public void ManualHideToolTips()
    {
        ToolTipSimple.Instance.ManualHide();
    }
    public void AssignItem(ItemInstance item)
    {
        _slotElement.RemoveFromClassList("empty");

        AddDataItem(item);

        _count = item.Count;
        _remainingTime = item.RemainingTime;

        if (_slotElement != null)
        {
            AddImage(this);
            AddTooltip(item);
            _slotDragManipulator.enabled = true;
        }
    }

    private void AddDataItem(ItemInstance item)
    {
        if (item.ItemData != null)
        {
            _id = item.ItemData.Id;
            _name = item.ItemData.ItemName.Name;
            _description = item.ItemData.ItemName.Description;
            _icon = item.ItemData.Icon;
            _objectId = item.ObjectId;
            _empty = false;
            _itemCategory = item.Category;
        }
        else
        {
            Debug.LogWarning($"Item data is null for item {item.ItemId}.");
            _id = 0;
            _name = "Unkown";
            _description = "Unkown item.";
            _icon = "";
            _objectId = -1;
            _itemCategory = ItemCategory.Item;
        }

    }

    private void AddImage(InventorySlot slot)
    {
        if (slot.GetType() == typeof(GearSlot))
        {
            StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
            _slotElement.style.backgroundImage = background;
        }
        else
        {
            StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
            _slotBg.style.backgroundImage = background;
        }
    }

    private void AddTooltip(ItemInstance item)
    {
        string tooltipText = $"{_name} ({_count})";
        if (item.Category == ItemCategory.Weapon ||
            item.Category == ItemCategory.Jewel ||
            item.Category == ItemCategory.ShieldArmor)
        {
            tooltipText = _name;
        }

        if (_tooltipManipulator != null)
        {
            _tooltipManipulator.SetText(tooltipText);
        }
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_slotClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_slotClickSoundManipulator);
            _slotClickSoundManipulator = null;
        }
    }
    public bool IsEmpty { get { return _empty; } }
    protected override void HandleLeftClick()
    {
        if (_currentTab != null)
        {
            _currentTab.SelectSlot(_position);
        }
    }

    protected override void HandleRightClick()
    {
        UseItem();
    }

    protected override void HandleMiddleClick()
    {
        if (!_empty)
        {
            PlayerInventory.Instance.DestroyItem(_objectId, 1);
        }
    }

    public virtual void UseItem()
    {
        if (!_empty)
        {
            PlayerInventory.Instance.UseItem(_objectId);
        }
    }
}
