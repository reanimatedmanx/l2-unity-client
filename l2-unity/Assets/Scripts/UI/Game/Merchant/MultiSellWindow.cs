
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UnityEngine.UIElements;


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

    private VisualTreeAsset _windowTemplateWeapon;
    private VisualTreeAsset _windowTemplateAcccesories;
    private VisualTreeAsset _windowTemplateArmor;

    private TooltipDataProvider _dataProvider;
    private CreateScroller _createScroller;
    private VisualElement _selectContainer;
    public static MultiSellWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorTradingWindows();
            _dataProvider = new TooltipDataProvider();
            _createScroller = new CreateScroller();
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
   

        _createScroller.RegisterPlayerScrollEvent(_scrollView1, _scrollView1.verticalScroller);
        _contentPanel1 = _scrollView1.contentContainer;


        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        RegisterCloseWindowEventByName("CloseButton");

        OnCenterScreen(_root);
        _creatorWindow.EventLeftClick += OnEventLeftClick;
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
    public void OnEventLeftClick(int itemId , ItemCategory category)
    {
        ItemInstance itemInstance = new ItemInstance(0, itemId, ItemLocation.Trade, 0, 1, category, false, ItemSlot.none, 0, 999);
        _selectContainer = GetContainer(itemInstance);

        if(_selectContainer != null)
        {
            UseItem(itemInstance, _selectContainer);
            _contentPanel1.Add(_selectContainer);
            _selectContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }

    public VisualElement GetContainer(ItemInstance item)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                return SwitchToWeapon();
            case ItemCategory.Jewel:
            case ItemCategory.Item:
                return SwitchToAccessories();
            case ItemCategory.ShieldArmor:
                return SwitchToArmor();
        }
        return null;
    }

    public void OnGeometryChanged(GeometryChangedEvent evt)
    {
        _contentPanel1.style.height = evt.newRect.height;
        _selectContainer.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }


    private void UseItem(ItemInstance item, VisualElement template)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                _dataProvider.AddDataWeapon(template, item);
                break;
           case ItemCategory.Jewel:
                _dataProvider.AddDataAccessories(template, item);
                break;
            case ItemCategory.Item:
                _dataProvider.AddDataOther(template, item);
                break;
           case ItemCategory.ShieldArmor:
                _dataProvider.AddDataArmor(template, item, null, null);
                break;
        }
    }


    private VisualElement SwitchToWeapon()
    {
        _contentPanel1.Clear();
        VisualElement weapon = ToolTipsUtils.CloneOne(_windowTemplateWeapon);
        var boxBackground = weapon.Q<VisualElement>("Box");
        var topBackground = weapon.Q<VisualElement>("Top");
        var centerBackground = weapon.Q<VisualElement>("Center");
        var growIcon = weapon.Q<VisualElement>("GrowIcon");
        ClearBorder(boxBackground, topBackground, centerBackground, growIcon);
        return weapon;
    }




    private VisualElement SwitchToAccessories()
    {
        _contentPanel1.Clear();

        var accessories = ToolTipsUtils.CloneOne(_windowTemplateAcccesories); 
        var boxBackground = accessories.Q<VisualElement>("Box");
        var topBackground = accessories.Q<VisualElement>("Top");
        var centerBackground = accessories.Q<VisualElement>("Center");
        var growIcon = accessories.Q<VisualElement>("GrowIcon");
        ClearBorder(boxBackground, topBackground, centerBackground, growIcon);

        return accessories;
    }

    private VisualElement SwitchToArmor()
    {
        _contentPanel1.Clear();

        var armor = ToolTipsUtils.CloneOne(_windowTemplateArmor);
        var boxBackground = armor.Q<VisualElement>("Box");
        var topBackground = armor.Q<VisualElement>("Top");
        var centerBackground = armor.Q<VisualElement>("Center");
        var growIcon= armor.Q<VisualElement>("GrowIcon");
        ClearBorder(boxBackground, topBackground, centerBackground , growIcon);

        return armor;

    }


    private void ClearBorder(VisualElement boxBackground, VisualElement topBackground, VisualElement centerBackground , VisualElement growIcon)
    {
        StyleColor currentColor = boxBackground.style.backgroundColor;
        boxBackground.style.backgroundColor = new Color(currentColor.value.r, currentColor.value.g, currentColor.value.b, 0);

        ResetBorder(boxBackground);

        topBackground.style.borderBottomWidth = 0;
        centerBackground.style.borderBottomWidth = 0;

        if(growIcon != null)
        {
            ResetBorder(growIcon);
        }
    }

    private void ResetBorder(VisualElement element)
    {
        element.style.borderBottomWidth = 0;
        element.style.borderLeftWidth = 0;
        element.style.borderRightWidth = 0;
        element.style.borderTopWidth = 0;
    }






}
