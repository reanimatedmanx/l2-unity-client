using FMOD.Studio;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
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
    private int _defaultCount = 24;
    private Label _adena;
    private Label _price;
    private Label _currentWeight;
    private List<Product> _listBuy;
    private List<Product> _listSell;
    private InventoryTab _tabSell;
    private InventoryTab _tabBuy;
    private InventorySlot[] _inventorySlotsBuy;
    private InventorySlot[] _inventorySlotsSell;
    private VisualElement _weiBar;
    private VisualElement _weiBarBg;
    private int _currentDataWeight = 0;
    private int _maxWeight = 0;
    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }
    private ShopItemEvaluator _itemEvalutor;
    private ShopInventoryController _shopCellCreator;
    public static DealerWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _itemEvalutor = new ShopItemEvaluator(_instance);
            _shopCellCreator = new ShopInventoryController(_instance , _defaultCount);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
        _itemEvalutor = null;
        _inventorySlotsBuy = null;
        _inventorySlotsSell = null;
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
        CreateInitBuy();
    }

   public void UpdateBuyData(List<Product>listBuy)
   {
        _listBuy = listBuy;
        if(_listSell != null) _listSell.Clear();

        _shopCellCreator.ClearSell(_tabSell);
        _shopCellCreator.CreateSell(listBuy, _tabSell, _contentSell , _windowEle);
        _shopCellCreator.ClearBuy(_inventorySlotsBuy, _tabBuy);
        _itemEvalutor.UpdateAllPrice(_listSell);
   }



    public void EventDoubleClick(VisualElement slotElement)
   {
        string[] ids = GetUniquePosition(slotElement);
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);
        SlotType slot = _shopCellCreator.DetectedClickPanel(type);
        Move—ellElsePriceType(slot , _listBuy , position);
        RefreshToolTips(slotElement , slot);
   }

    public void RefreshToolTips(VisualElement slotElement , SlotType slot)
    {
        if(SlotType.PriceBuy == slot)
        {
            ToolTipManager.GetInstance().EventLeftClickSlot(slotElement);
        }
        
    }
   

    private string[] GetUniquePosition(VisualElement ve)
    {
        return ve.name.Split('_');
    }

  

    private int positionMove = 0;
    private void Move—ellElsePriceType(SlotType type , List<Product> listBuy, int position)
    {
        if (type == SlotType.PriceBuy)
        {
            if (_listSell == null) return;
            if (!IsIndexValid(_listSell, position)) return;

            Product source = _listSell[position];

            MoveSell(position);
            _listSell = _shopCellCreator.RemoveProduct(_listSell, source);
            _itemEvalutor.UpdateAllPrice(_listSell);
            _itemEvalutor.UpdateWeight(_listSell, _currentDataWeight, _maxWeight);
        }
        else if (type == SlotType.PriceSell)
        {
            if (!IsIndexValid(listBuy, position)) return;
            Product source = listBuy[position];

            if (source == null) return;
            _listSell = _shopCellCreator.AddProduct(_listSell, source);

            if (_listSell.Count != 0) positionMove = GetNewPosition(_listSell);

            ToolTipManager.GetInstance().SetBuyData(_listSell);
            MoveBuy(positionMove, source);
            _itemEvalutor.UpdateAllPrice(_listSell);
            _itemEvalutor.UpdateWeight(_listSell, _currentDataWeight, _maxWeight);
        }
    }

    public bool IsIndexValid(List<Product> _listSell, int index)
    {
        return index >= 0 && index < _listSell.Count;
    }

    private int GetNewPosition(List<Product> listSell)
    {
        //return slots.FirstOrDefault(p =>  p == null | p.IsEmpty == true).Position;
        return listSell.Count - 1;
    }

    

    private void MoveBuy(int newPosition , Product product)
    {
        _inventorySlotsBuy[newPosition].AssignProduct(product);
        _tabBuy.UpdateInventorySlots(_inventorySlotsBuy);
    }
    //We do not move the target from sell -> buy because we believe that there is already something there and re-updating the cell is not required.We simply clear it
    //and shift all positions to the left if we delete an element
    public void MoveSell(int newPosition)
    {
        ShiftElementsLeft(newPosition);
        _tabSell.UpdateInventorySlots(_inventorySlotsSell);
    }

    public void ShiftElementsLeft(int newPosition)
    {
        for(int i =0; i < _inventorySlotsBuy.Length; i++)
        {
            if(i == newPosition)
            {
                var currentSlot = _inventorySlotsBuy[i];
                var nextSlot = GetNextSlot( i, _inventorySlotsBuy);
                HandleCurrentPosition(currentSlot, nextSlot);
            }
            else if (i > newPosition)
            {
                var currentSlot = _inventorySlotsBuy[i];
                var nextSlot = GetNextSlot(i, _inventorySlotsBuy);
                if (nextSlot == null) return;
                HandleSlotsAfterCurrent(currentSlot, nextSlot);
            }
        }
    }



    private void HandleCurrentPosition(InventorySlot currentSlot, InventorySlot nextSlot)
    {
        _inventorySlotsBuy[currentSlot.Position].AssignProduct(nextSlot.product);

        if (nextSlot.IsEmpty)
        {
            nextSlot.ManualHideToolTips();
            _inventorySlotsBuy[currentSlot.Position].AssignEmpty();
        }
    }


    private void HandleSlotsAfterCurrent(InventorySlot currentSlot, InventorySlot nextSlot)
    {
        ElseNextSlotEmpty(nextSlot, currentSlot);
    }


    private void ElseNextSlotEmpty(InventorySlot nextSlot, InventorySlot slot)
    {
        if (nextSlot.IsEmpty)
        {
            _inventorySlotsBuy[slot.Position].AssignEmpty();
        }
        else
        {
            _inventorySlotsBuy[slot.Position].AssignProduct(nextSlot.product);
        }
    }

    private InventorySlot GetNextSlot(int i , InventorySlot[] _inventorySlotsBuy)
    {
        return (i + 1 < _inventorySlotsBuy.Length) ? _inventorySlotsBuy[i + 1] : null;
    }
 

   

    //_weiBar = GetElementById("WeightBg");
    //_weiBarBg = GetElementById("WeightGauge");

    public VisualElement GetWeiBar()
    {
        return _weiBar;
    }

    public VisualElement GetWeiBarBg()
    {
        return _weiBarBg;
    }

    public void UpdatePrice(int allPrice)
    {
        _price.text = ToolTipsUtils.ConvertToPrice(allPrice);
    }
    
    public void UpdateDataWeight(int data)
    {
        _currentDataWeight = data;
    }

    public void UpdateAdena(int adena)
    {
        _adena.text = ToolTipsUtils.ConvertToPrice(adena);
    }

    public void UpdateWeight(double weightPercent)
    {
        _currentWeight.text = weightPercent.ToString() + "%";
    }

    public void UpdateAllWeight(int currentWeight, int maxWeight)
    {
        _itemEvalutor.UpdateWeightBar(this , currentWeight, maxWeight);
    }

    public void UpdateDataMaxWeight(int data)
    {
        _maxWeight = data;
    }

    public InventorySlot[] GetInventorySlotsSell()
    {
        return _inventorySlotsSell;
    }

    public void SetInventorySlotsSell(InventorySlot[] inventorySlotSell)
    {
        _inventorySlotsSell = inventorySlotSell;
    }

    public VisualTreeAsset GetInventorySlotTemplate()
    {
        return _inventorySlotTemplate;
    }

    public VisualElement GetContentSell()
    {
        return _contentSell;
    }

    public void SetTabSell(InventoryTab tabSell)
    {
        _tabSell = tabSell;
    }

    public void UpdateDataForm(int adena, double weightPercent, int currentWeight, int maxWeight)
    {
        _itemEvalutor.UpdateDataForm(adena, weightPercent, currentWeight, maxWeight);
    }

 

    private void CreateInitBuy()
    {
        _tabBuy = new InventoryTab();
        _contentBuy.Clear();
        _tabBuy.Initialize(_windowEle, _contentBuy, _contentBuy);
        _inventorySlotsBuy = _shopCellCreator.CreateEmptyBuy(_tabBuy, _defaultCount, _contentBuy, GetInventorySlotTemplate());
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");

    }


}
