
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using static UnityEditor.Progress;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


public class ToolTipSimple : L2PopupWindow, IToolTips
{
    private ShowToolTip _showToolTip;

    private static ToolTipSimple _instance;
    private VisualElement _content;
    private VisualTreeAsset _windowTemplateWeapon;
    private VisualTreeAsset _windowTemplateSimple;
    private VisualElement _contentInside;
    //Block
    //private Label _nameText;
    //private Label _descriptedText;

    //private VisualElement _icon;
   // private VisualElement _groubBoxIcon;
    private float _lastHeightContent = 0;
    private float _heightContent = 0;
    private float _widtchContent = 0;
    private VisualElement _selectShow;
    //private Dictionary<int, VisualElement> _dictElements;

    public static ToolTipSimple Instance { get { return _instance; } }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
           // _dictElements = new Dictionary<int, VisualElement>();
             _showToolTip = new ShowToolTip(this);
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipObject");
        _windowTemplateSimple = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSimple");
        _windowTemplateWeapon = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipWeapon");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);



        yield return new WaitForEndOfFrame();


        _content = GetElementByClass("control-box");
        
       //_content.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        //_content.RegisterCallback<AttachToPanelEvent>(OnAttachChanged);
        //_content.style.flexGrow = 1; // Позволяет GroupBox расти
        //_content.style.flexShrink = 1; // Позволяет GroupBox сжиматься

        //var weaponToolTips = _windowTemplateSimple.CloneTree();
       // weaponToolTips.style.flexGrow = 1;
        //_content.Add(weaponToolTips);

        //_contentInside = weaponToolTips.Q<VisualElement>(className: "content");
        //_contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        //_contentInside.MarkDirtyRepaint();
        //_nameText = (Label)GetElementByClass("Heading");
        //_descriptedText = (Label)GetElementByClass("DescriptedLabel");
        //_icon = GetElementByClass("Icon");
        //_groubBoxIcon = GetElementByClass("Grow");

        //_heightContent = _contentInside.worldBound.height;
        //_widtchContent = _contentInside.worldBound.width;
        _windowEle.style.display = DisplayStyle.None;
        //_descriptedText.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        //var test1 = _windowTemplateWeapon.Instantiate()[0];
        //_content.Add(test1);
    }


    public void EventLeftClickSlot(VisualElement ve)
    {
        string[] ids = GetUniquePosition(ve);
        TemplateContainer template = SwitchToolTip(ids, true);
        if(template != null)
        {
            int position = Int32.Parse(ids[0]);
            int type = Int32.Parse(ids[1]);

            Product product = GetProductClickLeft(type, position);
            AddTestDataWeaponToolTip(template, product);
        }

        
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
    private IEnumerator StartNewPosition(VisualElement ve)
    {
        string[] ids = GetUniquePosition(ve);
        TemplateContainer template = SwitchToolTip(ids , false);
        if(template != null)
        {
            _windowEle.style.display = DisplayStyle.Flex;
            _contentInside.MarkDirtyRepaint();

            Debug.Log("TooTipAction StartNewPosition disabled 2 " + _contentInside.worldBound.height);

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

    private TemplateContainer SwitchToolTip(string[] ids , bool isClickLeft)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);
        if (isClickLeft)
        {
            if (type == (int)SlotType.PriceSell)
            {
               Product product =  ToolTipManager.GetInstance().FindProductInSellList(position);
                if(product != null)
                {
                    if (product.GetTypeItem() == EnumType2.TYPE2_WEAPON)
                    {
                        return SwitchToWeapon();
                    }
                }

            }else if (type == (int)SlotType.PriceBuy)
            {
                Product product = ToolTipManager.GetInstance().FindProductInBuyList(position);
                if(product != null)
                {
                    if (product.GetTypeItem() == EnumType2.TYPE2_WEAPON)
                    {
                        return SwitchToWeapon();
                    }
                }
  
            }
        }
        else
        {
            if (type == (int)SlotType.PriceSell | type == (int)SlotType.PriceBuy)
            {
                return SwitchToSimple();
            }
        }

        return null;
    }

    public Product GetProductClickLeft(int type , int position)
    {
            if (type == (int)SlotType.PriceSell)
            {
                return ToolTipManager.GetInstance().FindProductInSellList(position);

            }
            else if (type == (int)SlotType.PriceBuy)
            {
               return ToolTipManager.GetInstance().FindProductInBuyList(position);

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

    private TemplateContainer SwitchToWeapon()
    {
        _content.Clear();
        var weapon = _windowTemplateWeapon.CloneTree();
        _content.Add(weapon);
        _contentInside = weapon.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return weapon;
        //AddTestDataWeaponToolTip(weapon, ids);
    }

    private void AddTestDataWeaponToolTip(TemplateContainer container ,  Product product)
    {

        if (product != null)
        {
            Weapongrp weapon = product.GetWeapon();
            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);

            var nameWeapon = container.Q<Label>("nameWeapon").text = text.GetName();

            if(ItemGrade.none != weapon.Grade)
            {
                var gradeWeapon = container.Q<Label>("gradeWeapon");
                gradeWeapon.text = ItemGradeParser.Converter(weapon.Grade);
            }
            
            var typeWeapon = container.Q<Label>("typeWeapon").text = WeaponTypeParser.WeaponTypeName(weapon.WeaponType);
            var physLabel = container.Q<Label>("physLabel").text = weapon.PAtk.ToString();
            var magLabel = container.Q<Label>("magLabel").text = weapon.Matk.ToString();
            var spdLabel = container.Q<Label>("spdLabel").text = weapon.GetSpeedName();
            var critLabel = container.Q<Label>("critLabel").text = weapon.CriticalRate.ToString();
            var soullabel = container.Q<Label>("soullabel").text = "X"+weapon.Soulshot.ToString();
            var spiritlabel = container.Q<Label>("spiritlabel").text = "X" + weapon.Spiritshot.ToString();
            var weightlabel = container.Q<Label>("weightlabel").text = weapon.Weight.ToString();
            var descriptedLabel = container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
            var icon = container.Q<VisualElement>("icon");
            icon.style.backgroundImage = IconManager.Instance.LoadTextureByName(weapon.Icon);
        }
       


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
        //}
        //else
        //{
            //2px border
         //   float new_y = newPoint.y + 2;
         //   Vector2 reversePoint = new Vector2(newPoint.x, new_y);
         //   _content.transform.position = reversePoint;
        //}
        BringToFront();
    }

    private Vector2 highestPoint(Vector2 newPoint, float element)
    {
        //Added 28px. If there are problems with the upper tooltips, you need to remove them
        var element1 = element + 28;
        return new Vector2(newPoint.x, newPoint.y - element1);
    }

    //PosY/2 - center point left vertical border
    // PosY > newPoint = TOP
    //PosY < newPoint = Footer
    //private bool IsTop(Vector2 newPoint)
    //{
        //float yPos = ActionWindow.Instance.getYposition();
       // float heightAction = ActionWindow.Instance.getHeight() / 2;
        //var end = yPos + heightAction;
        //return end >= newPoint.y;
    //}
    private Vector2 lowestPoint(Vector2 newPoint, float element)
    {
        return new Vector2(newPoint.x, newPoint.y + element);
    }



    private void AddData(string[]ids , TemplateContainer template)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        if(type == (int)SlotType.PriceSell)
        {
            Product product =  ToolTipManager.GetInstance().FindProductInSellList(position);
            SetSimpleSingleToolTip(product , template);

        }
        else if (type == (int)SlotType.PriceBuy)
        {
            Product product = ToolTipManager.GetInstance().FindProductInBuyList(position);
            SetSimpleSingleToolTip(product , template);
        }

    }

  



    private void SetSimpleSingleToolTip(Product product , TemplateContainer template)
    {
        if (product != null)
        {
            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
            var _descriptedText = (Label)template.Q<VisualElement>(null, "DescriptedLabel");
            var _nameText = (Label)template.Q<VisualElement>(null, "Heading");

            _descriptedText.style.fontSize = 12;
            _nameText.style.paddingLeft = 0;
            _descriptedText.style.color = GetColorPrice(text.GetDiscription());

            var icon = template.Q<VisualElement>(null, "Icon");
            var groubBoxIcon = template.Q<VisualElement>(null, "Grow");

            SetIcon(icon , groubBoxIcon , null);
            SetDataTooTip(_nameText , _descriptedText , text.GetName(), "Price: " + ConvertToPrice(Int32.Parse(text.GetDiscription())) + " Adena");
        }
        else
        {
            _windowEle.style.display = DisplayStyle.None;
            _showToolTip.Hide(null);
        }
    }

    private Color GetColorPrice(string price)
    {
        if(price.Length == 6)
        {
            return new Color(255f / 255f, 126f / 255f, 255f / 255f);
            //return new Color(0, 251, 255);
        }
        else if (price.Length == 5)
        {
            return new Color(0, 251, 255);
        }
        return new Color(169, 180, 196);
    }
    private string ConvertToPrice(int wholeNumber)
    {
        string formattedNumber = wholeNumber.ToString("N0", CultureInfo.InvariantCulture);
        return  formattedNumber.Replace('.', ',');
    }
    private void SetIcon(VisualElement icon , VisualElement groubBoxIcon, string nameIcon)
    {
        
        if(nameIcon != null)
        {
            icon.style.backgroundImage = IconManager.Instance.LoadTextureByName(nameIcon);
        }
        else
        {
            //_icon.style.backgroundImage = null;
            // _groubBoxIcon.style.backgroundImage = null;
            //icon.style.display = DisplayStyle.None;
            groubBoxIcon.style.display = DisplayStyle.None;
        }
       
    }

    private void SetDataTooTip(Label labelName , Label descriptedText,  string name, string descripted)
    {
        if (!string.IsNullOrEmpty(name))
        {
            labelName.text = name;
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            descriptedText.text = descripted;
        }
    }

    public void ResetPosition(Vector2 vector2)
    {
        _content.transform.position = vector2;
        SendToBack();
    }
}
