
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static L2Slot;
using static UnityEditor.Progress;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


public class ToolTipSimple : L2PopupWindow, IToolTips
{
    private ShowToolTip _showToolTip;

    private static ToolTipSimple _instance;
    private VisualElement _content;
    private VisualTreeAsset _windowTemplateWeapon;
    private VisualTreeAsset _windowTemplateSimple;
    private VisualTreeAsset _windowTemplateString;
    private VisualTreeAsset _windowTemplateAcccesories;
    private VisualTreeAsset _windowTemplateArmor;
    private VisualTreeAsset _setsElements;
    private VisualTreeAsset _setsEffects;
    private VisualElement _contentInside;
    private float _lastHeightContent = 0;
    private float _heightContent = 0;
    private float _widtchContent = 0;
    private VisualElement _selectShow;
    private TooltipDataProvider _dataProvider;

    public static ToolTipSimple Instance { get { return _instance; } }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
 
             _showToolTip = new ShowToolTip(this);
            _dataProvider = new TooltipDataProvider();
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipObject");
        _windowTemplateString = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipString");
        _windowTemplateSimple = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSimple");
        _windowTemplateWeapon = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipWeapon");
        _windowTemplateAcccesories = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipAccessories");
        _windowTemplateArmor = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipArmor");
        _setsElements = LoadAsset("Data/UI/_Elements/Game/ToolTips/Elements/SetsElements");
        _setsEffects = LoadAsset("Data/UI/_Elements/Game/ToolTips/Elements/SetsEffectNameElements");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);



        yield return new WaitForEndOfFrame();


        _content = GetElementByClass("control-box");
        _windowEle.style.display = DisplayStyle.None;

    }


    public void EventLeftClickSlot(VisualElement ve)
    {
        string[] ids = GetUniquePosition(ve);
        TemplateContainer template = SwitchToolTip(ids, true);

        if(template != null)
        {
            int position = Int32.Parse(ids[0]);
            int type = Int32.Parse(ids[1]);

            object data = GetDataClickLeft(type, position);

            if(data != null)
            {
                AddData(data, template);
            }
        }
    }


    private void AddData(object data , TemplateContainer template)
    {
        if(data.GetType() == typeof(Product))
        {
            Product product = (Product)data;
            UseProduct(product, template);
        }else if (data.GetType() == typeof(ItemInstance))
        {
            ItemInstance item = (ItemInstance)data;
            UseItem(item, template);
        }
  
    }

    private void UseProduct(Product product, TemplateContainer template)
    {
        switch (product.GetTypeItem())
        {
            case EnumType2.TYPE2_WEAPON:
                _dataProvider.AddDataWeapon(template, product);
                break;
            case EnumType2.TYPE2_ACCESSORY:
                _dataProvider.AddDataAccessories(template, product);
                break;
            case EnumType2.TYPE2_OTHER:
                _dataProvider.AddDataOther(template, product);
                break;
            case EnumType2.TYPE2_SHIELD_ARMOR:
                _dataProvider.AddDataArmor(template, product, _setsElements , _setsEffects);
                break;
        }
    }

    private void UseItem(ItemInstance item, TemplateContainer template)
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
                _dataProvider.AddDataArmor(template, item, _setsElements, _setsEffects);
                break;
        }
    }

    public void ManualHide()
    {
        StartCoroutine(AllHide());
    }
    public void RegisterCallback(SlotType type , List<VisualElement> list)
    {

        list.ForEach(item =>
        {
            if (item != null)
            {
                item.RegisterCallback<MouseOverEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        StartCoroutine(StartNewPosition(ve));
                        //Debug.Log("TooTipAction StartNewPosition disabled 33" + _windowEle.worldBound.height);
                       /// Debug.Log("TooTipAction StartNewPosition disabled 33 " + _content.worldBound.height);
                    }
                }, TrickleDown.TrickleDown);

                item.RegisterCallback<MouseOutEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        StartCoroutine(EndNewPosition(ve));
                    }

                    evt.StopPropagation();

                }, TrickleDown.TrickleDown);

            }
        });

    }

    public void RegisterCallback(SlotType type, VisualElement item , int position)
    {
         RegisterPrice(type , item, position);   
    }

    private void RegisterPrice(SlotType type , VisualElement item , int position)
    {
        if (item != null)
        {
            SetIdElement(item, position, (int)type);

            item.RegisterCallback<MouseOverEvent>(evt =>
            {
                VisualElement ve = (VisualElement)evt.currentTarget;
                if (ve != null)
                {
                    StartCoroutine(StartNewPosition(ve));
                }
            }, TrickleDown.TrickleDown);

            item.RegisterCallback<MouseOutEvent>(evt =>
            {
                VisualElement ve = (VisualElement)evt.currentTarget;
                if (ve != null)
                {
                    StartCoroutine(EndNewPosition(ve));
                }

                evt.StopPropagation();

            }, TrickleDown.TrickleDown);

        }
    }

    private void SetIdElement(VisualElement item, int position , int type)
    {
        item.name = position + "_"+ type.ToString();
    }

    private IEnumerator EndNewPosition(VisualElement ve)
    {
        _windowEle.style.display = DisplayStyle.None;
        _showToolTip.Hide(ve);
        yield return new WaitForEndOfFrame();
    }
    public IEnumerator AllHide()
    {
        _windowEle.style.display = DisplayStyle.None;
        yield return new WaitForEndOfFrame();
    }
    private IEnumerator StartNewPosition(VisualElement ve)
    {
        string[] ids = GetUniquePosition(ve);
        TemplateContainer template = SwitchToolTip(ids , false);
        if(template != null)
        {
            _windowEle.style.display = DisplayStyle.Flex;
            _contentInside.MarkDirtyRepaint();

            AddData(ids, template);
            _selectShow = ve;
            //esle position layout != 0 <--- (EndNewPosition add new Vector2(0,0))
            //This means the layout did not return to the base, most likely this is an error. Restarting the transition to a new position
            //if (_windowEle.worldBound.height != 0)
            if (_contentInside.worldBound.height != 0)
            {
                _showToolTip.Show(_selectShow);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private TemplateContainer SwitchToolTip(string[] ids, bool isClickLeft)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        if (isClickLeft)
        {
            switch (type)
            {
                case (int)SlotType.PriceSell:
                    Product productSell = ToolTipManager.GetInstance().FindProductInSellList(position);
                    if (productSell != null) return GetProductContainer(productSell);
                    break;
                case (int)SlotType.PriceBuy:
                    Product productBuy = ToolTipManager.GetInstance().FindProductInBuyList(position);
                    if (productBuy != null) return GetProductContainer(productBuy);
                    break;

                case (int)SlotType.Inventory:
                    ItemInstance item = InventoryWindow.Instance.GetItemByPosition(position);
                    if (item != null) return GetInventoryContainer(item);
                    break;
                case (int)SlotType.Enchant:
                    ItemInstance enchantItem = EnchantWindow.Instance.GetItemByPosition(position);
                    if (enchantItem != null) return GetInventoryContainer(enchantItem);
                    break;
                case (int)SlotType.Gear:
                    GearItem gearItem = InventoryWindow.Instance.GetGearPosition(position);
                    return GetGearContainer(gearItem);
                    //break;
            }
        }
        else
        {
            switch (type)
            {
                case (int)SlotType.PriceSell:
                case (int)SlotType.PriceBuy:
                    return SwitchToSimple();

                case (int)SlotType.Inventory:
                    return SwitchToString();
                case (int)SlotType.Enchant:
                    return SwitchToString();
                case (int)SlotType.Gear:
                    GearItem gearItem = InventoryWindow.Instance.GetGearPosition(position);
                    return GetGearStringContainer(gearItem);
            }
        }

        return null;
    }

    private TemplateContainer GetProductContainer(Product product)
    {
        switch (product.GetTypeItem())
        {
            case EnumType2.TYPE2_WEAPON:
                return SwitchToWeapon();
            case EnumType2.TYPE2_ACCESSORY:
            case EnumType2.TYPE2_OTHER:
                return SwitchToAccessories();
            case EnumType2.TYPE2_SHIELD_ARMOR:
                return SwitchToArmor();
        }
        return null;
    }

    private TemplateContainer GetInventoryContainer(ItemInstance item)
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

    public TemplateContainer GetContainer(ItemInstance item)
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


    private TemplateContainer GetGearContainer(GearItem gearItem)
    {
        if (gearItem != null && gearItem.GetItemId() != 0)
        {
            int objectId = gearItem.GetObjectId();

            ItemInstance assignItem = PlayerInventory.Instance.GetItemEquip(objectId);

            if (assignItem.BodyPart == ItemSlot.fullarmor)
            {
                if (gearItem.GetSlotType() != ItemSlot.legs)
                {
                    return GetInventoryContainer(assignItem);
                }
            }
            else if (assignItem.BodyPart == ItemSlot.lrhand)
            {
                if (gearItem.GetSlotType() != ItemSlot.lhand)
                {
                    return GetInventoryContainer(assignItem);
                }
            }
            else
            {
                return GetInventoryContainer(assignItem);
            }

        }

        return null;
    }

    private TemplateContainer GetGearStringContainer(GearItem gearItem)
    {
        if (gearItem != null && gearItem.GetItemId() != 0)
        {
            int objectId = gearItem.GetObjectId();

            ItemInstance assignItem = PlayerInventory.Instance.GetItemEquip(objectId);

            if (assignItem.BodyPart == ItemSlot.fullarmor)
            {
                if (gearItem.GetSlotType() != ItemSlot.legs)
                {
                    return SwitchToString();
                }
            }
            else if (assignItem.BodyPart == ItemSlot.lrhand)
            {
                if (gearItem.GetSlotType() != ItemSlot.lhand)
                {
                    return SwitchToString();
                }
            }
            else
            {
                return SwitchToString();
            }

        }

        return null;
    }

    //backup code for switch tooltip
    // private TemplateContainer SwitchToolTip(string[] ids , bool isClickLeft)
    //{
    //int position = Int32.Parse(ids[0]);
    //  int type = Int32.Parse(ids[1]);

    //if (isClickLeft)
    //{
    //if (type == (int)SlotType.PriceSell)
    //{
    // Product product =  ToolTipManager.GetInstance().FindProductInSellList(position);
    //if(product != null)
    //{
    // switch (product.GetTypeItem())
    // {
    // case EnumType2.TYPE2_WEAPON:
    //  return SwitchToWeapon();
    // case EnumType2.TYPE2_ACCESSORY:
    //   return SwitchToAccessories();
    // case EnumType2.TYPE2_OTHER:
    //  return SwitchToAccessories();
    //case EnumType2.TYPE2_SHIELD_ARMOR:
    //    return SwitchToArmor();
    // }
    // }

    // }else if (type == (int)SlotType.PriceBuy)
    //{
    //Product product = ToolTipManager.GetInstance().FindProductInBuyList(position);
    //if(product != null)
    // {
    //switch (product.GetTypeItem())
    //{
    //case EnumType2.TYPE2_WEAPON:
    //    return SwitchToWeapon();
    // case EnumType2.TYPE2_ACCESSORY:
    //   return SwitchToAccessories();
    //case EnumType2.TYPE2_OTHER:
    //  return SwitchToAccessories();
    //case EnumType2.TYPE2_SHIELD_ARMOR:
    //  return SwitchToArmor();
    //}
    //}

    // }
    // else if (type == (int)SlotType.Inventory)
    //{
    // ItemInstance item = InventoryWindow.Instance.GetItemByPosition(position);
    // if (item != null)
    /// {
    // switch (item.Category)
    // {
    // case ItemCategory.Weapon:
    //    return SwitchToWeapon();
    // case ItemCategory.Jewel:
    //    return SwitchToAccessories();
    // case ItemCategory.Item:
    //       return SwitchToAccessories();
    //   case ItemCategory.ShieldArmor:
    //       return SwitchToArmor();
    //  }
    // }
    // }
    // }
    //else
    // {
    //if (type == (int)SlotType.PriceSell | type == (int)SlotType.PriceBuy)
    // {
    //     return SwitchToSimple();
    // }else if (type == (int)SlotType.Inventory)
    // {
    //    return SwitchToString();
    // }
    // else if (type == (int)SlotType.Gear)
    // {
    //     return SwitchToString();
    // }
    // }

    // return null;
    //}

    public object GetDataClickLeft(int type , int position)
    {
            if (type == (int)SlotType.PriceSell)
            {
                return ToolTipManager.GetInstance().FindProductInSellList(position);
            }
            else if (type == (int)SlotType.PriceBuy)
            {
                return ToolTipManager.GetInstance().FindProductInBuyList(position);
            }
            else if (type == (int)SlotType.Inventory)
            {
                return InventoryWindow.Instance.GetItemByPosition(position);
            }
            else if (type == (int)SlotType.Enchant)
            {
                return EnchantWindow.Instance.GetItemByPosition(position);
            }
        else if (type == (int)SlotType.Gear)
            {
                GearItem gearItem = InventoryWindow.Instance.GetGearPosition(position);

                if (gearItem != null && gearItem.GetItemId() != 0)
                {
                    int objectId = gearItem.GetObjectId();
                    return  PlayerInventory.Instance.GetItemEquip(objectId);
                }
            }

        return null;
    }

    private TemplateContainer SwitchToSimple()
    {
        _content.Clear();
        var template = _windowTemplateSimple.CloneTree();
        _content.Add(template);
        _contentInside = template.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return template;
    }

  

    private TemplateContainer SwitchToString()
    {
        _content.Clear();
        var template = _windowTemplateString.CloneTree();
        _content.Add(template);
        _contentInside = template.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return template;
    }

    private TemplateContainer GetStringContainerToGear(int position)
    {
        GearItem gearItem = InventoryWindow.Instance.GetGearPosition(position);
        return GetGearContainer(gearItem);
    }

    private TemplateContainer SwitchToWeapon()
    {
        _content.Clear();
        var weapon = _windowTemplateWeapon.CloneTree();
        _content.Add(weapon);
        _contentInside = weapon.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return weapon;
    }

    private TemplateContainer SwitchToAccessories()
    {
        _content.Clear();
        var _accessories = _windowTemplateAcccesories.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }

    private TemplateContainer SwitchToArmor()
    {
        _content.Clear();
        var _accessories = _windowTemplateArmor.CloneTree();
        _content.Add(_accessories);
        _contentInside = _accessories.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return _accessories;
    }



    private string[] GetUniquePosition(VisualElement ve)
    {
        return  ve.name.Split('_');
    }


    //experimental code 
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (evt.newRect.height == 0)
            return;

        if (_contentInside != null)
        {
            _heightContent = _contentInside.worldBound.height;
            _widtchContent = _contentInside.worldBound.width;
            _showToolTip.Show(_selectShow);
        }

    }

 
    //experimental code 
    public void NewPosition(Vector2 newPoint, float sdfig)
    {
        
        var highest = highestPoint(newPoint, _heightContent);
        var lowest = lowestPoint(newPoint, _heightContent);
        bool insideRoot = L2GameUI.Instance.IsWindowContain(highest);
        //if (!ActionWindow.Instance.IsWindowContain(lowest) | IsTop(newPoint))
       // {
            if (!insideRoot)
            {
                //shift down to 0 and to the right to the icon border
                //2px border 
                float sdfig1 = sdfig + 2;
               // float new_x = newPoint.x + _widtchContent;
                float new_x2 = newPoint.x + sdfig1;
                Vector2 reversePoint = new Vector2(new_x2, 0);
            
                // var test1 = _windowTemplateWeapon.Instantiate()[0];
               // _content.Add(test1);
                _content.transform.position = reversePoint;
            }
            else
            {
                float width = _heightContent;
                float newddfig = width;
                //2px border 
                float sdfig1 = sdfig + 2;
                float new_y = newPoint.y - newddfig;
                float new_y2 = new_y - sdfig1;
                Vector2 reversePoint = new Vector2(newPoint.x, new_y2);

                _content.transform.position = reversePoint;
            }

        BringToFront();
    }

    private Vector2 highestPoint(Vector2 newPoint, float element)
    {
        //Added 28px. If there are problems with the upper tooltips, you need to remove them
        var element1 = element + 28;
        return new Vector2(newPoint.x, newPoint.y - element1);
    }


    private Vector2 lowestPoint(Vector2 newPoint, float element)
    {
        return new Vector2(newPoint.x, newPoint.y + element);
    }



    private void AddData(string[] ids, TemplateContainer template)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        switch (type)
        {
            case (int)SlotType.PriceSell:
                Product productSell = ToolTipManager.GetInstance().FindProductInSellList(position);
                SetSimpleSingleToolTip(productSell, template);
                break;

            case (int)SlotType.PriceBuy:
                Product productBuy = ToolTipManager.GetInstance().FindProductInBuyList(position);
                SetSimpleSingleToolTip(productBuy, template);
                break;

            case (int)SlotType.Inventory:
                ItemInstance itemInstance = InventoryWindow.Instance.GetItemByPosition(position);
                SetSimpleItemSingleToolTip(itemInstance, template);
                break;
            case (int)SlotType.Enchant:
                ItemInstance itemEnchant = EnchantWindow.Instance.GetItemByPosition(position);
                SetSimpleItemSingleToolTip(itemEnchant, template);
                break;
            case (int)SlotType.Gear:
                GearItem gearItem = InventoryWindow.Instance.GetGearPosition(position);

                if (gearItem != null && gearItem.GetItemId() != 0)
                {
                    int objectId = gearItem.GetObjectId();

                    ItemInstance assignItem = PlayerInventory.Instance.GetItemEquip(objectId);
                    SetSimpleItemSingleToolTip(assignItem, template);
                }
                
                break;
        }
    }

    //private void AddData(string[]ids , TemplateContainer template)
    //{
    //int position = Int32.Parse(ids[0]);
    //int type = Int32.Parse(ids[1]);

    //if(type == (int)SlotType.PriceSell)
    // {
    //Product product =  ToolTipManager.GetInstance().FindProductInSellList(position);
    //SetSimpleSingleToolTip(product , template);

    //}
    //else if (type == (int)SlotType.PriceBuy)
    //{
    //Product product = ToolTipManager.GetInstance().FindProductInBuyList(position);
    //  SetSimpleSingleToolTip(product , template);
    //}
    //else if (type == (int)SlotType.Inventory)
    //{
    // ItemInstance itemInstance = InventoryWindow.Instance.GetItemByPosition(position);
    // SetSimpleItemSingleToolTip(itemInstance, template);
    //}

    //}





    private void SetSimpleSingleToolTip(Product product , TemplateContainer template)
    {
        if (product != null)
        {
            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
            var _descriptedText = (Label)template.Q<VisualElement>(null, "DescriptedLabel");
            var _nameText = (Label)template.Q<VisualElement>(null, "Heading");

            _descriptedText.style.fontSize = 12;
            _nameText.style.paddingLeft = 0;
            _descriptedText.style.color = ToolTipsUtils.GetColorPrice(text.GetPrice());

            var icon = template.Q<VisualElement>(null, "Icon");
            var groubBoxIcon = template.Q<VisualElement>(null, "Grow");

            SetIcon(icon , groubBoxIcon , null);
            string decription = text.GetPrice();
            if (!string.IsNullOrEmpty(decription))
            {
                SetDataToolTip(_nameText, _descriptedText, text.GetName(), "Price: " + ToolTipsUtils.ConvertToPrice(Int32.Parse(decription)) + " Adena");
            }
            else
            {
                SetDataToolTip(_nameText, _descriptedText, text.GetName(), "");
            }
            
        }
        else
        {
            _windowEle.style.display = DisplayStyle.None;
            _showToolTip.Hide(null);
        }
    }


    private void SetSimpleItemSingleToolTip(ItemInstance item, TemplateContainer template)
    {
        if (item != null)
        {
            IDataTips data = ToolTipManager.GetInstance().GetProductText(item);
            // var _nameText = (Label)template.Q<VisualElement>(null, "name");
            // var _gradeImg = template.Q<VisualElement>(null, "grade");

            Label nameText =(Label) template.Q<VisualElement>("name");
            VisualElement gradeImg = template.Q<VisualElement>("grade");
            Label encahntText = template.Q<Label>("enchant");

            Texture2D grade = data.GetGradeTexture();

            if (nameText != null)
            {
                nameText.text = data.GetName();
            }

            SetImageElement(gradeImg, grade);
            SetTextEnchantIfNot0(item.EnchantLevel, encahntText);
        }
        else
        {
            _windowEle.style.display = DisplayStyle.None;
            _showToolTip.Hide(null);
        }
    }



    private void SetImageElement(VisualElement element , Texture2D texture)
    {
        if(element != null)
        {
            if (texture == null)
            {
                element.style.display = DisplayStyle.None;
            }
            else
            {
                element.style.display = DisplayStyle.Flex;
                element.style.backgroundImage = texture;
            }
        }

    }


    private void SetIcon(VisualElement icon , VisualElement groubBoxIcon, string nameIcon)
    {
        
        if(nameIcon != null)
        {
            icon.style.backgroundImage = IconManager.Instance.LoadTextureByName(nameIcon);
        }
        else
        {
            groubBoxIcon.style.display = DisplayStyle.None;
        }
       
    }

    public void SetTextEnchantIfNot0(int data , Label element)
    {
        if (data != 0)
        {
            element.style.display = DisplayStyle.Flex;
            element.text = "+"+ data;
        }
        else
        {
            element.style.display = DisplayStyle.None;
        }
    }
    private void SetDataToolTip(Label labelName , Label descriptedText,  string name, string descripted)
    {
        if (!string.IsNullOrEmpty(name))
        {
            labelName.style.display = DisplayStyle.Flex;
            labelName.text = name;
        }
        else
        {
            descriptedText.style.display = DisplayStyle.None;
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            descriptedText.style.display = DisplayStyle.Flex;
            descriptedText.text = descripted;
        }
        else
        {
            descriptedText.style.display = DisplayStyle.None;

        }
    }



    public void ResetPosition(Vector2 vector2)
    {
        _content.transform.position = vector2;
        SendToBack();
    }
}
