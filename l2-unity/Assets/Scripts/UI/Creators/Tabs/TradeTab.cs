using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static L2Slot;
using static UnityEngine.Rendering.DebugUI;

public class TradeTab
{
    private TradingSlot[] _tradeSlots;
    
    private string _tabName = "Tab";
    private int _defaultCountSlot = 0;
    private CreateScroller _createScroller;
    private bool _mainTab = false;
    public string TabName { get { return _tabName; } }

    private VisualElement _tabContainer;
    private VisualElement _tabHeader;
    private VisualElement _contentContainer;
    public event Action<TradeTab> EventSwitch;
    public TradeTab(string tabName , int countSlot , VisualElement tabContainer, VisualElement tabHeader, bool initEmpty)
    {
        _createScroller = new CreateScroller();
        _tabName = tabName;
        _defaultCountSlot = countSlot;
        _tabHeader = tabHeader;
        _tabContainer = tabContainer;
        _contentContainer = tabContainer.Q<VisualElement>("Content");
        _createScroller.Start(tabContainer);
        CreateEmptyInventory(initEmpty);
        OnRegisterClickTab(tabHeader);
    }


    private void OnRegisterClickTab(VisualElement tabHeader)
    {

            tabHeader.RegisterCallback<MouseDownEvent>(evt => {
                EventSwitch?.Invoke(this);
            }, TrickleDown.TrickleDown);
    }
    

    private void CreateEmptyInventory(bool initEmty)
    {
        if (_contentContainer != null && initEmty)
        {
            _contentContainer.Clear();
            _tradeSlots = new TradingSlot[_defaultCountSlot];
            CreateSlots(_tradeSlots, _contentContainer);
            //UpdateInventorySlots(_inventorySlots);
        }

    }


    private void CreateSlots(TradingSlot[] tradeSlots , VisualElement contentContainer)
    {
        for (int i = 0; i < _tradeSlots.Length; i++)
        {

            VisualElement slotElement = CretaVisualElement();
            TradingSlot slot = CreateTradeSlot(new TradingSlotModel(i, this, slotElement, SlotType.Multisell));
            contentContainer.Add(slotElement);
            tradeSlots[i] = slot;
        }
    }

    public void AddDataTrade(List<ItemInstance> allItems)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemInstance item = allItems[i];
            item.SetSlot(i);
            //Debug.Log("AssignItem Set Inventory>>>> " + item.ItemId + " ObjectId " + item.ObjectId + " Add Slot " + item.Slot);
            _tradeSlots[i].AssignItem(item);
        }
    }
















    private VisualElement CretaVisualElement()
    {
        return InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
    }

    private TradingSlot CreateTradeSlot(TradingSlotModel model)
    {
        return new TradingSlot(model);
    }
    public bool GetMainTab()
    {
        return _mainTab;
    }
    public void SetMainTab(bool main)
    {
        _mainTab = main;
    }

    public void UnselectTabContainerClass()
    {
        if(_tabContainer != null && _tabHeader != null)
        {
            _tabContainer.AddToClassList("unselected-tab");
            _tabHeader.RemoveFromClassList("active");
        }
        else
        {
            Debug.LogError("TradeTab > RefreshTabContainerClass Not Found _tabContainer or _tabHeader");
        }

    }

    public void SelectTabContainerClass()
    {
        if (_tabContainer != null && _tabHeader != null)
        {
            _tabContainer.RemoveFromClassList("unselected-tab");
            _tabHeader.AddToClassList("active");
        }
        else
        {
            Debug.LogError("TradeTab > RefreshTabContainerClass Not Found _tabContainer or _tabHeader");
        }

    }


}
