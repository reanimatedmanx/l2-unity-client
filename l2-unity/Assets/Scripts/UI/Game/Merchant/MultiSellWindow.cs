using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiSellWindow : L2PopupWindow
{
    private static MultiSellWindow _instance;
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualElement _inventoryTabView;
    private ICreatorTrading _creatorWindow;
    public static MultiSellWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorTradingWindows();
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


        _inventoryTabView = GetElementById("InventoryTabView");
        _creatorWindow.InitTabs(new string[] { "ALL" , "Other" });
        _creatorWindow.CreateTabs(_inventoryTabView, _tabTemplate, _tabHeaderTemplate);

        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
        yield return new WaitForEndOfFrame();
    }


    public void AddData(List<ItemInstance> allItems)
    {
        _creatorWindow.AddDataTrade(allItems);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    

}
