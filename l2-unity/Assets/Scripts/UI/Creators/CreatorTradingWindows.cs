using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatorTradingWindows : ICreatorTrading
{
    private TradeTab[] _tabs;
    private string[] _nameTabs;
    private TradeTab _activeTab;

    public event Action<int , ItemCategory , int> EventLeftClick;

    public void InitTabs(string[] nameTabs)
    {
        _nameTabs = nameTabs;
        _tabs = new TradeTab[nameTabs.Length];
    }

    public void CreateTabs(VisualElement inventoryTabView , VisualTreeAsset _tabTemplate , VisualTreeAsset _tabHeaderTemplate)
    {
        VisualElement tabHeaderContainer = inventoryTabView.Q<VisualElement>("tab-header-container");
        VisualElement tabContainer = inventoryTabView.Q<VisualElement>("tab-content-container");

        if (inventoryTabView != null & _tabTemplate != null  & _tabHeaderTemplate != null & tabContainer != null)
        {
           

            for (int i = _tabs.Count() - 1; i >= 0; i--)
            {
                string tabName = _nameTabs[i];

                VisualElement tabElement = _tabTemplate.CloneTree()[0];
                tabElement.name = tabName;
                tabElement.AddToClassList("unselected-tab");
                

                VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
                tabHeaderElement.name = tabName;
                tabHeaderElement.Q<Label>().text = tabName;

                tabHeaderContainer.Add(tabHeaderElement);
                tabContainer.Add(tabElement);

                _tabs[i] = new TradeTab(tabName, 96, tabElement, tabHeaderElement , true);

                _tabs[i].EventSwitch += OnSwitchEvent;
                _tabs[i].EventLeftClick += OnLeftClick;
            }
        }
        else
        {
            Debug.LogError("CreatorTradingWindows: Not Create Windows tabs!!!!");
        }

        if (_tabs.Length > 0)
        {
            SwitchTab(_tabs[0]);
        }

        SetMainTab(0, true);
    }

    public void SetClickActiveTab(int position)
    {
        if(_activeTab != null)
        {
            _activeTab.OnClickLeftEvent(position);
        }
    }

    public void SwitchTab(TradeTab switchTo)
    {
        if (_activeTab != switchTo)
        {
            if (_activeTab != null) _activeTab.UnselectTabContainerClass();
            _activeTab = switchTo;
            if (_activeTab != null) _activeTab.SelectTabContainerClass();
            
        }
    }

    private void OnSwitchEvent(TradeTab tab)
    {
        if(_tabs.Length > 1)
        {
            if (tab != null)
            {
                SwitchTab(tab);
            }
        }
 
    }

    private void OnLeftClick(int itemId , ItemCategory category , int position)
    {
        EventLeftClick?.Invoke(itemId , category , position);
    }
    public TradeTab GetMainTab()
    {
        foreach (TradeTab tab in _tabs)
        {
            if(tab != null & tab.GetMainTab() == true)
            {
                return tab;
            }
        }

        return null;
    }
    public void SetMainTab(int index , bool main)
    {
        _tabs[index].SetMainTab(main);
    }

    public void AddDataTrade(List<ItemInstance> allItems)
    {
        if(_activeTab != null)
        {
            _activeTab.AddDataTrade(allItems);
        }
    }

    public ItemInstance GetActiveByPosition(int position)
    {
        if(_activeTab != null)
        {
           return _activeTab.GetSlotByPosition(position);
        }

        return null;
    }

}
