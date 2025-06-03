using UnityEngine;
using UnityEngine.UIElements;

public class GearItem
{
    private VisualElement _slotElement;
    private ItemSlot _itemSlot;
    private GearSlot _gearSlot;
    public GearItem(VisualElement slotElement , ItemSlot itemSlot)
    {
        _slotElement = slotElement;
        _itemSlot = itemSlot;

    }

    public ItemSlot GetSlotType()
    {
        return _itemSlot;
    }
    public void SetGearSlot(GearSlot gearSlot)
    {
        _gearSlot = gearSlot;
    }

    public void SetSelectGear()
    {
        _gearSlot.SetSelected();
    }

    public void UnSelectGear()
    {
        _gearSlot.UnSelect();
    }


    public void Assign(ItemInstance item)
    {
        _gearSlot.AssignItem(item);
    }

    public void AssignEmpty()
    {
        _gearSlot.AssignEmpty();
    }
    public bool IsEmptyGearSlot()
    {
        return _gearSlot.IsEmpty;
    }
    public VisualElement GetElement()
    {
        return _slotElement;
    }
}
