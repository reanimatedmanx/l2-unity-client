using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static L2Slot;
using static UnityEditor.Progress;

public class DealerWindow : L2PopupWindow
{


    private static DealerWindow _instance;
    private VisualElement _contentBuy;
    private VisualElement _contentSell;
    private VisualTreeAsset _inventorySlotTemplate;
    private int _deaultCount = 24;
    private Label _adena;
    private Label _price;
    private Label _currentWeight;
    private List<Product> _listBuy;
    private List<Product> _listSell;
    private InventoryTab _tabSell;
    private InventoryTab _tabBuy;
    private InventorySlot[] _inventorySlotsBuy;
    private VisualElement _weiBar;
    private VisualElement _weiBarBg;
    private int _currentDataWeight = 0;
    private int _maxWeight = 0;
    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }
    public static DealerWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Merchant/DealerWindow");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Template/Slot");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        _contentBuy = GetElementByClass("content-buy");
        _contentSell = GetElementByClass("content-sell");

        _price = (Label)GetElementById("PriceCount");
        _adena = (Label) GetElementById("AdenaCount");
        _currentWeight = (Label)GetElementById("CurrentWeight");

        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        _weiBar = GetElementById("WeightBg");
        _weiBarBg = GetElementById("WeightGauge");
        OnCenterScreen(root);
    }

   public void UpdateBuyData(List<Product>listBuy)
   {
        
        _listBuy = listBuy;
        if(_listSell != null) _listSell.Clear();


        CreateSell(listBuy);
        CreateBuy();
   }

    public void UpdateDataForm( int adena, double weightPercent, int currentWeight, int maxWeight)
    {
        _currentDataWeight = currentWeight;
        _maxWeight = maxWeight;
        UpdateAdena(adena);
        UpdateWeight(weightPercent);
        UpdateWeightBar(currentWeight, maxWeight);
    }

    public void EventDoubleClick(VisualElement slotElement)
   {
        string[] ids = GetUniquePosition(slotElement);
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);
        SlotType slot = DetectedClickPanel(type);
        Move—ellElsePriceType(slot , _listBuy , position);
   }

   

    private string[] GetUniquePosition(VisualElement ve)
    {
        return ve.name.Split('_');
    }

    private SlotType DetectedClickPanel(int type)
    {

        if(type == (int)SlotType.PriceBuy )
        {
            return SlotType.PriceBuy;
        }
        else if (type == (int)SlotType.PriceSell)
        {
            return SlotType.PriceSell;
        }
        return SlotType.Other;
    }

    private void Move—ellElsePriceType(SlotType type , List<Product> listBuy, int position)
    {
        if (type == SlotType.PriceBuy)
        {

        }
        else if (type == SlotType.PriceSell)
        {
            Product source = listBuy[position];

            _listSell = AddProduct(_listSell, source);
            int positionMove = 0;
            if (_listSell.Count != 0) positionMove = _listSell.Count - 1;

            ToolTipManager.GetInstance().SetBuyData(_listSell);
            MoveBuy(positionMove, source);
            UpdateAllPrice();
            UpdateWeight();
        }
    }

    private List<Product> AddProduct(List<Product> listSell , Product source)
    {
        if (listSell == null)
        {
            return new List<Product>() { source };
           //return _listSell;
        }

        listSell.Add(source);
        return listSell;
    }

    private void MoveBuy(int newPosition , Product product)
    {
        _inventorySlotsBuy[newPosition].AssignProduct(product);
        _tabBuy.UpdateInventorySlots(_inventorySlotsBuy);
    }
    private void UpdateAllPrice()
    {
        int allPrice = 0;

        if(_listSell != null)
        {
            for (int i = 0; i < _listSell.Count; i++)
            {
                allPrice = allPrice + _listSell[i].Price;
            }

            UpdatePrice(allPrice);
        }
 
    }

    private void UpdateWeight()
    {
        int addWeight = 0;

        if (_listSell != null)
        {
            for (int i = 0; i < _listSell.Count; i++)
            {
                addWeight = addWeight + _listSell[i].GetWeapon().Weight;
            }

            UpdateAllWeight(_currentDataWeight + addWeight , _maxWeight);
        }
    }

    public void UpdateAllWeight(int currentWeight , int  maxWeight)
    {
        UpdateWeightBar(currentWeight, maxWeight);
    }


    private string ConvertToPrice(int wholeNumber)
    {
        string formattedNumber = wholeNumber.ToString("N0", CultureInfo.InvariantCulture);
        return formattedNumber.Replace('.', ',');
    }
    private void UpdateAdena(int adena)
    {
        _adena.text = ConvertToPrice(adena);
    }

    private void UpdateWeight(double weightPercent)
    {
        _currentWeight.text = weightPercent.ToString()+"%";
    }

    private void UpdateWeightBar(int currentWeight , int maxWeight)
    {
        if (_weiBar != null & _weiBarBg != null)
        {
            float bgWidth = 138;
            //float bgWidth = _expBarBg.resolvedStyle.width;
            double expRatio = (double)currentWeight / maxWeight;
            double barWidth = bgWidth * expRatio;
            if (maxWeight == 0)
            {
                barWidth = 0;
            }
            //_weiBar.style.width = Convert.ToSingle(barWidth);
            _weiBarBg.style.width = Convert.ToSingle(barWidth);
        }
    }

    private void UpdatePrice(int allPrice)
    {
        _price.text = ConvertToPrice(allPrice);
    }
    

    private void CreateSell(List<Product> listBuy)
    {
        _tabSell = new InventoryTab();
        _contentSell.Clear();
        _tabSell.Initialize(_windowEle, _contentSell, _contentSell);

        ToolTipManager.GetInstance().SetSellData(listBuy);

        if(listBuy == null | listBuy.Count == 0)
        {
            CreateEmptySell(_tabSell);
        }
        else
        {
            CreateNoEmptySell(listBuy, _tabSell);
        }


    }
  
    private void CreateEmptySell(InventoryTab tabSell)
    {
        var _inventorySlotsSell = new InventorySlot[_deaultCount];

        for (int i = 0; i < _deaultCount; i++)
        {
            VisualElement slotElement = InventorySlotTemplate.Instantiate()[0];
            _contentSell.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, slotElement, tabSell, SlotType.PriceSell);
            _inventorySlotsSell[i] = slot;
        }
        tabSell.UpdateInventorySlots(_inventorySlotsSell);
    }

    private void CreateNoEmptySell(List<Product> listBuy , InventoryTab tabSell)
    {
        if(listBuy.Count <= 24)
        {

            InventorySlot[] slotsSell = InitEmpty(_deaultCount, tabSell);

            for (int i = 0; i < listBuy.Count; i++)
            {
                slotsSell[i].AssignProduct(listBuy[i]);
            }
        }
        else
        {
            InventorySlot[] slotsSell = InitEmpty(listBuy.Count, tabSell);

            for (int i = 0; i < listBuy.Count; i++)
            {
                slotsSell[i].AssignProduct(listBuy[i]);
            }
        }
        
    }

    private InventorySlot[] InitEmpty(int count , InventoryTab tabSell)
    {
        var _inventorySlotsSell = new InventorySlot[_deaultCount];

        for (int i = 0; i < _deaultCount; i++)
        {
            VisualElement slotElement = InventorySlotTemplate.Instantiate()[0];
            _contentSell.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, slotElement, tabSell, SlotType.PriceSell);
            _inventorySlotsSell[i] = slot;
        }
        tabSell.UpdateInventorySlots(_inventorySlotsSell);

        return _inventorySlotsSell;
    }

    private void CreateBuy()
    {
        _tabBuy = new InventoryTab();
        _contentBuy.Clear();
        _tabBuy.Initialize(_windowEle, _contentBuy, _contentBuy);
        CreateEmptyBuy(_tabBuy);
    }

    private void CreateEmptyBuy(InventoryTab tabBuy)
    {
        _inventorySlotsBuy = new InventorySlot[_deaultCount];
        for (int i = 0; i < _deaultCount; i++)
        {
            VisualElement slotElement = InventorySlotTemplate.Instantiate()[0];
            _contentBuy.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, slotElement, tabBuy, SlotType.PriceBuy);
            _inventorySlotsBuy[i] = slot;
        }

        tabBuy.UpdateInventorySlots(_inventorySlotsBuy);
    }


    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");

    }


}
