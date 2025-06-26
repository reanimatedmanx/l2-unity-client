using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public class TradingSlot: L2DraggableSlot
{
    private TradingSlotModel model;
    protected bool _empty = true;
    private AssignData _data;
    public TradingSlot(TradingSlotModel model)
        : base(model.GetPosition(), model.GetSlotElement(), model.GetSlotType())
    {
        _data = new AssignData();
        _empty = true;
    }

    public void AssignItem(ItemInstance item)
    {
        _slotElement.RemoveFromClassList("empty");
        _data.RefreshData(item);
        _data.RefreshDataItem(item);
        _empty = false;

        if (_slotElement != null)
        {
            AddImage(SlotBg , SlotElement , _data.GetItemId());
        }
        else
        {
            Debug.LogWarning("Не критическая ошибка не смогли найти TradingSlotModel>SlotElement");
        }
    }

    public void AssignEmpty()
    {
        _empty = true;
        _data.ResetData();

        if (_slotElement != null)
        {
            StyleBackground background = new StyleBackground(IconManager.Instance.GetInvetoryDefaultBackground());
            UpdateSlotBg(background);
        }
    }

    private void UpdateSlotBg(StyleBackground background)
    {
        if (SlotBg != null)
        {
            _slotBg.style.backgroundImage = background;
            _slotDragManipulator.enabled = false;

        }
        else
        {

            _slotElement.style.backgroundImage = null;
        }
    }

    private void AddImage(VisualElement slotBg , VisualElement slotElement , int id)
    {
        if (slotBg == null & slotElement == null) return;

        StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(id));
        slotBg.style.backgroundImage = background;
    }
}




public class TradingSlotModel
{
    private TradeTab _currentTab;
    private VisualElement _slotElement;
    private SlotType _slotType;
    private int _position;

    public TradingSlotModel(int position , TradeTab currentTab , VisualElement slotElement , SlotType slotType)
    {
        _position = position;
        _currentTab = currentTab;
        _slotElement = slotElement;
        _slotType = slotType;
    }

    public TradeTab GetTab()
    {
        return _currentTab;
    }

    


    public VisualElement GetSlotElement(){ return _slotElement;}

    public SlotType GetSlotType() { return _slotType;}

    public int GetPosition() { return _position; }
}
