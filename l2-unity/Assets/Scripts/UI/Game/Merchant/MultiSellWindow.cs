using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiSellWindow : L2PopupWindow
{
    private static MultiSellWindow _instance;
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualElement _inventoryTabView;
    private List<InventoryTab> _tabs;
    private InventoryTab _activeTab;
    public static MultiSellWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _tabs = new List<InventoryTab>(2);
        }
        else
        {
            Destroy(this);
        }
    }


    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Merchant/MultiSellWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTabHeader");
    }


    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();
        InitTabs();
        CreateTabs();
        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
        yield return new WaitForEndOfFrame();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void InitTabs()
    {
        InventoryTab mainTab = new InventoryTab();
        mainTab.SetTabName("All Items");
        mainTab.SetAutoScroll(true);
        mainTab.SetSelectedSlot(-1);

        List<ItemCategory> filter1 = new List<ItemCategory>();
        filter1.Add(ItemCategory.Item);
        mainTab.SetFilterCategories(filter1);

         InventoryTab otherTab = new InventoryTab();
         otherTab.SetTabName("Available");
        List<ItemCategory> filter2 = new List<ItemCategory>();
        filter2.Add(ItemCategory.Item);
        otherTab.SetFilterCategories(filter2);


        _tabs.Add(mainTab);
        _tabs.Add(otherTab);
    }

    private void CreateTabs()
    {
        _inventoryTabView = GetElementById("InventoryTabView");

        VisualElement tabHeaderContainer = _inventoryTabView.Q<VisualElement>("tab-header-container");
        if (tabHeaderContainer == null)
        {
            Debug.LogError("tab-header-container is null");
        }
        VisualElement tabContainer = _inventoryTabView.Q<VisualElement>("tab-content-container");

        if (tabContainer == null)
        {
            Debug.LogError("tab-content-container");
        }

        for (int i = _tabs.Count - 1; i >= 0; i--)
        {

            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement, true, true);
        }

        if (_tabs.Count > 0)
        {
            SwitchTab(_tabs[0]);
        }

        _tabs[0].MainTab = true;
    }

    public bool SwitchTab(InventoryTab switchTo)
    {

        if (_activeTab != switchTo)
        {
            if (_activeTab != null)
            {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");

            _activeTab = switchTo;
            return true;
        }

        return false;
    }

}
