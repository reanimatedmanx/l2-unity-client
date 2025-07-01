
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UnityEngine.UIElements;
using static UnityEditor.Progress;


public class MultiSellWindow : L2PopupWindow
{
    private static MultiSellWindow _instance;
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualElement _inventoryTabView;
    private ICreatorTrading _creatorWindow;

    private ScrollView _scrollView1;
    private ScrollView _scrollView2;
    private VisualElement _contentPanel1;
    private VisualElement _contentPanel2;

    private VisualTreeAsset _windowTemplateWeapon;
    private VisualTreeAsset _windowTemplateAcccesories;
    private VisualTreeAsset _windowTemplateArmor;
    private VisualTreeAsset _itemTemplateIngredient;
    private MultiSellList _listMultisell;
    private TooltipDataProvider _dataProvider;
    private MultiSellToolTips _toolTips;
    // private CreateScroller _createScroller;
    private VisualElement _selectContainer;
    public static MultiSellWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorTradingWindows();
            _dataProvider = new TooltipDataProvider();
            _toolTips = new MultiSellToolTips(_dataProvider);

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

        _windowTemplateWeapon = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipWeapon");
        _windowTemplateAcccesories = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipAccessories");
        _windowTemplateArmor = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipArmor");
        _itemTemplateIngredient = LoadAsset("Data/UI/_Elements/Template/Merchant/ItemsIngredient");
    }


    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();


        _inventoryTabView = GetElementById("InventoryTabView");

        _creatorWindow.InitTabs(new string[] { "ALL" , "Other" });
        _creatorWindow.CreateTabs(_inventoryTabView, _tabTemplate, _tabHeaderTemplate);

        _scrollView1 = (ScrollView)GetElementById("1Panel-ScrollView");
        _scrollView2 = (ScrollView)GetElementById("2Panel-ScrollView");
   

        //_createScroller.RegisterPlayerScrollEvent(_scrollView1, _scrollView1.verticalScroller);
        _contentPanel1 = _scrollView1.contentContainer;
        _contentPanel2 = _scrollView2.contentContainer;

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        RegisterCloseWindowEventByName("CloseButton");

        OnCenterScreen(_root);

        _creatorWindow.EventLeftClick += OnEventLeftClick;
        _toolTips.SetTemplate(_windowTemplateWeapon, _windowTemplateAcccesories, _windowTemplateArmor, _itemTemplateIngredient);

        yield return new WaitForEndOfFrame();
    }


    public void AddData(List<ItemInstance> allItems , MultiSellList listMultisell)
    {
        if(allItems.Count > 0 && listMultisell != null)
        {
            _listMultisell = listMultisell;
            _creatorWindow.AddDataTrade(allItems);
        }

        _creatorWindow.SetClickActiveTab(0);
    }


    public void OnEventLeftClick(int itemId , ItemCategory category, int position)
    {
        ItemInstance itemInstance = new ItemInstance(0, itemId, ItemLocation.Trade, 0, 1, category, false, ItemSlot.none, 0, 999);
        itemInstance.SetMultiSell(true);
        _contentPanel1.Clear();
        _selectContainer = _toolTips.GetContainer(itemInstance);

        if(_selectContainer != null)
        {
            _toolTips.UseItem(itemInstance, _selectContainer);
            _contentPanel1.Add(_selectContainer);
            _selectContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);


            _contentPanel2.Clear();

            List<MultiSellData> data = _listMultisell.GetMultiSell();
            List<Ingredient> ingredients = null;

            if (!IfMore1Item( data, itemId))
            {
                ingredients = GetIngredient(data, itemInstance.ItemId);
            }
            else
            {
                ingredients = GetIngredientByIndex(data, position);
            }
            
            Debug.Log("Select position " + position);
            
           if(ingredients != null)
           {
                _toolTips.AddIngredient(ingredients, _contentPanel2);
           }
            
        }
    }

   

    public void OnGeometryChanged(GeometryChangedEvent evt)
    {
        _contentPanel1.style.height = evt.newRect.height;
        _selectContainer.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private List<Ingredient> GetIngredient(List<MultiSellData> data , int itemId)
    {
        foreach (MultiSellData item in data)
        {
            List<ItemInstance> listItem = item.ProductList;
            bool result =  listItem.Any(x => x.ItemId == itemId);
            if (result)
            {
                return item.IngredientList;
            }
        }

        return null;
    }

    public bool IfMore1Item(List<MultiSellData> data, int itemId)
    {
        List<MultiSellData>  list = data.Where(x => x.ProductList[0].ItemId == itemId).ToList();

        if(list.Count > 1)
        {
            return true;
        }
        return false;
    }

    private List<Ingredient> GetIngredientByIndex(List<MultiSellData> data, int index)
    {
        if(IsValidIndex(data, index))
        {
            return data[index].IngredientList;
        }
        return null;
    }

    private bool IsValidIndex(List<MultiSellData> array, int index)
    {
        return index >= 0 && index < array.Count;
    }


    private void OnDestroy()
    {
        _instance = null;
    }


}
