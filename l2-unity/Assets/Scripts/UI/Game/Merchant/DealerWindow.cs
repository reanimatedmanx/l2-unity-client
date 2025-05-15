
using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

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

    private bool _isModifySell = false;

    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }
    private ShopItemEvaluator _itemEvalutor;
    private ShopInventoryController _shopCellCreator;

    private Label _panelSellHeaderName;
    private Label _panelBuyHeaderName;
    private int _listId;
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

        _panelSellHeaderName = (Label)GetElementById("SellH1");
        _panelBuyHeaderName = (Label)GetElementById("BuyH1");

        var succesBtn = (Button)GetElementById("SuccessButton");
        succesBtn.RegisterCallback<ClickEvent>((evt) => HandleSuccessButtonClick());
        RegisterCloseWindowEventByName("CloseButton");
        _panelSellHeaderName.text = "Sell";
        _panelSellHeaderName.text = "Buy";

        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        _weiBar = GetElementById("WeightBg");
        _weiBarBg = GetElementById("WeightGauge");
        OnCenterScreen(root);
        CreateInitBuy();
    }


   public void UpdateBuyData(List<Product>listBuy ,bool  isModifySell , int listId)
   {
        _listId = listId;
        _listBuy = listBuy;
        _isModifySell = isModifySell;
        if(_listSell != null) _listSell.Clear();

        _shopCellCreator.ClearSell(_tabSell);
        _shopCellCreator.CreateSell(listBuy, _tabSell, _contentSell , _windowEle);
        _shopCellCreator.ClearBuy(_inventorySlotsBuy, _tabBuy);
        _itemEvalutor.UpdateAllPrice(_listSell);
   }

   public void SetHeaderNameSellPanel(string headerName)
   {
        _panelSellHeaderName.text = headerName;
   }

    public void SetHeaderNameBuyPanel(string headerName)
    {
        _panelBuyHeaderName.text = headerName;
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

  


    private void Move—ellElsePriceType(SlotType type , List<Product> listServer, int position)
    {
        if (type == SlotType.PriceBuy)
        {
            if (_listSell == null) return;
            if (!IsIndexValid(_listSell, position)) return;

            Product source = _listSell[position];

            ModifyBuy(_isModifySell, _listBuy, source);
            ShiftBuy(position);

            _listSell = _shopCellCreator.RemoveProduct(_listSell, source);
            _itemEvalutor.UpdateAllPrice(_listSell);
            _itemEvalutor.UpdateWeight(_listSell, _currentDataWeight, _maxWeight);
        }
        else if (type == SlotType.PriceSell)
        {
            if (!IsIndexValid(listServer, position)) return;
            Product source = listServer[position];
            
            if (source == null) return;
            _listSell = _shopCellCreator.AddProduct(_listSell, source);

            RefreshToolTips(_listSell);

            if (_listSell.Count != 0)
            {
                Move(_inventorySlotsBuy, _tabBuy, GetNewPosition(_listSell), source);
            }

            ModifySell(_isModifySell, position, listServer, source);

            _itemEvalutor.UpdateAllPrice(_listSell);
            _itemEvalutor.UpdateWeight(_listSell, _currentDataWeight, _maxWeight);
            
        }
    }

    private void ModifySell(bool isModify , int position , List<Product> listServer , Product source)
    {
        if (isModify)
        {
            ShiftSell(position);
            _listBuy = _shopCellCreator.RemoveProduct(listServer, source);
        }
   
    }

    private void ModifyBuy(bool isModify, List<Product> listBuy, Product source)
    {
        if (isModify)
        {
            if (_listSell.Count != 0)
            {
                listBuy = _shopCellCreator.AddProduct(listBuy, source);
                Move(_inventorySlotsSell, _tabSell, GetNewPosition(listBuy), source);
            }
        }

    }

    private void RefreshToolTips(List<Product> listSell)
    {
        ToolTipManager.GetInstance().SetBuyData(listSell);
    }

    public bool IsIndexValid(List<Product> _listSell, int index)
    {
        return index >= 0 && index < _listSell.Count;
    }

    private int GetNewPosition(List<Product> list)
    {
        //if (list.Count == 0) return 0;
        return list.Count - 1;
    }





    private void Move(InventorySlot[] inventorySlots , InventoryTab tab , int newPosition , Product product)
    {
        inventorySlots[newPosition].AssignProduct(product);
        tab.UpdateInventorySlots(inventorySlots);
    }

    private void ShiftSell(int newPosition)
    {
        ShiftElementsLeft(_inventorySlotsSell, newPosition);
        _tabBuy.UpdateInventorySlots(_inventorySlotsBuy);
    }
    //We do not move the target from sell -> buy because we believe that there is already something there and re-updating the cell is not required.We simply clear it
    //and shift all positions to the left if we delete an element
    public void ShiftBuy(int newPosition)
    {
        ShiftElementsLeft(_inventorySlotsBuy , newPosition);
        _tabSell.UpdateInventorySlots(_inventorySlotsSell);
    }

    public void ShiftElementsLeft(InventorySlot[] inventorySlots , int newPosition )
    {
        for(int i =0; i < inventorySlots.Length; i++)
        {
            if(i == newPosition)
            {
                var currentSlot = inventorySlots[i];
                var nextSlot = GetNextSlot( i, inventorySlots);
                HandleCurrentPosition(inventorySlots , currentSlot, nextSlot);
            }
            else if (i > newPosition)
            {
                var currentSlot = inventorySlots[i];
                var nextSlot = GetNextSlot(i, inventorySlots);
                if (nextSlot == null) return;
                HandleSlotsAfterCurrent(inventorySlots , currentSlot, nextSlot);
            }
        }
    }



    private void HandleCurrentPosition(InventorySlot[] inventorySlots , InventorySlot currentSlot, InventorySlot nextSlot)
    {
        inventorySlots[currentSlot.Position].AssignProduct(nextSlot.product);

        if (nextSlot.IsEmpty)
        {
            nextSlot.ManualHideToolTips();
            inventorySlots[currentSlot.Position].AssignEmpty();
        }
    }


    private void HandleSlotsAfterCurrent(InventorySlot[] inventorySlots , InventorySlot currentSlot, InventorySlot nextSlot)
    {
        ElseNextSlotEmpty(inventorySlots , nextSlot, currentSlot);
    }


    private void ElseNextSlotEmpty(InventorySlot[] inventorySlots , InventorySlot nextSlot, InventorySlot slot)
    {
        if (nextSlot.IsEmpty)
        {
            inventorySlots[slot.Position].AssignEmpty();
        }
        else
        {
            inventorySlots[slot.Position].AssignProduct(nextSlot.product);
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

    //var sendPaket = CreatorPacketsUser.CreateDestroyItem(objectId, quantity);
    //bool enable = GameClient.Instance.IsCryptEnabled();
    //SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);

    //not found adena
    // Charge buyer and add tax to castle treasury if not owned by npc clan
    //if (subTotal< 0 || !player.reduceAdena("Buy", (int) subTotal, player.getCurrentFolk(), false))
    //{
    //sendPacket(SystemMessage.getSystemMessage(SystemMessageId.YOU_NOT_ENOUGH_ADENA));
    //return;
    //}

    /**
 * ID: 279<br>
 * Message: You do not have enough adena.
 */
    private void HandleSuccessButtonClick()
    {
        HideWindow();

        if (!_isModifySell)
        {
            SetAddCount1(_listSell);
            var sendPaket = CreatorPacketsUser.CreateRequestBuyItem(_listId, _listSell);
            SendGameDataQueue.Instance().AddItem(sendPaket, GameClient.Instance.IsCryptEnabled(), GameClient.Instance.IsCryptEnabled());
        }
       
    }

    private void SetAddCount1(List<Product> _listSell)
    {
        foreach (var item in _listSell)
        {
            item.SetCount(1);
        }
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");

    }


}
