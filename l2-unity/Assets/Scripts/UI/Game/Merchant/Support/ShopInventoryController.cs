using System.Collections.Generic;
using UnityEngine;
using static L2Slot;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class ShopInventoryController
{
    private DealerWindow _dealerWindow;
    private int _defaultCount;
    public ShopInventoryController(DealerWindow dealerWindow, int defaultCount)
    {
        _dealerWindow = dealerWindow;
        _defaultCount = defaultCount;
    }
    public void CreateSell(List<Product> listBuy , InventoryTab tabSell , VisualElement contentSell , VisualElement windowEle)
    {
        if (tabSell == null)
        {
            tabSell = new InventoryTab();
            _dealerWindow.SetTabSell(tabSell);
            tabSell.Initialize(windowEle, contentSell, contentSell);
        }

        contentSell.Clear();
        

        ToolTipManager.GetInstance().SetSellData(listBuy);

        var inventorySlotSell = _dealerWindow.GetInventorySlotsSell();
        var inventorySlotTemplate = _dealerWindow.GetInventorySlotTemplate();

        if (listBuy == null | listBuy.Count == 0)
        {
            CreateEmptySell(tabSell , contentSell, inventorySlotSell, inventorySlotTemplate);
        }
        else
        {
            CreateNoEmptySell(listBuy, tabSell , inventorySlotSell, inventorySlotTemplate,  contentSell );
        }
    }

    private void CreateEmptySell(InventoryTab tabSell, VisualElement contentSell , InventorySlot[] inventorySlotsSell , VisualTreeAsset inventorySlotTemplate)
    {
        inventorySlotsSell = new InventorySlot[_defaultCount];

        for (int i = 0; i < inventorySlotsSell.Length; i++)
        {
            VisualElement slotElement = inventorySlotTemplate.Instantiate()[0];
            contentSell.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, slotElement, tabSell, SlotType.PriceSell);
            inventorySlotsSell[i] = slot;
        }
        _dealerWindow.SetInventorySlotsSell(inventorySlotsSell);
        tabSell.UpdateInventorySlots(inventorySlotsSell);
    }

    private void CreateNoEmptySell(List<Product> listBuy, InventoryTab tabSell , InventorySlot[] inventorySlotsSell, VisualTreeAsset inventorySlotTemplate, VisualElement contentSell)
    {
            int count =  GetCountCells(listBuy.Count);
            Debug.Log("Create cout " + count);
            inventorySlotsSell = InitEmpty(count, tabSell, contentSell, inventorySlotTemplate);

            for (int i = 0; i < listBuy.Count; i++)
            {
                inventorySlotsSell[i].AssignProduct(listBuy[i]);
            }
 
        _dealerWindow.SetInventorySlotsSell(inventorySlotsSell);

    }
    //24 minimal cells count
    private int GetCountCells(int countBuy)
    {
        return (countBuy <= _defaultCount) ? _defaultCount : GetNextMultipleOfSix(countBuy);
    }

    private InventorySlot[] InitEmpty(int count, InventoryTab tabSell , VisualElement contentSell , VisualTreeAsset inventorySlotTemplate)
    {
        var _inventorySlotsSell = _dealerWindow.GetInventorySlotsSell();

        if (_inventorySlotsSell == null)
        {

            _inventorySlotsSell = CreateNewArray(count);

            Debug.Log("_inventorySlotsSell 1 " + _inventorySlotsSell.Length);

            for (int i = 0; i < _inventorySlotsSell.Length; i++)
            {
                InventorySlot slot = CreateSlot(contentSell, inventorySlotTemplate, tabSell, i);
                _inventorySlotsSell[i] = slot;
            }
        }
        else
        {

            _inventorySlotsSell =  ReCreateElseChangeCount(_inventorySlotsSell, count);
            Debug.Log("_inventorySlotsSell 2 " + _inventorySlotsSell.Length);
            Debug.Log("_inventorySlotsSell contentSell 1 " + contentSell.childCount);


            for (int i = 0; i < _inventorySlotsSell.Length; i++)
            {
                if(_inventorySlotsSell[i] == null)
                {
                    InventorySlot slot = CreateSlot(contentSell, inventorySlotTemplate, tabSell, i);
                    _inventorySlotsSell[i] = slot;
                }
                else
                {
                    _inventorySlotsSell[i].AssignEmpty();
                    contentSell.Add(_inventorySlotsSell[i].SlotElement);
                    //_inventorySlotsSell[i].RefreshPosition(i);
                }
            }
        }

        tabSell.UpdateInventorySlots(_inventorySlotsSell);
        Debug.Log("_inventorySlotsSell contentSell 2 " + contentSell.childCount);
        return _inventorySlotsSell;
    }

    private InventorySlot CreateSlot(VisualElement contentSell , VisualTreeAsset inventorySlotTemplate , InventoryTab tabSell , int i)
    {
        VisualElement slotElement = inventorySlotTemplate.Instantiate()[0];
        contentSell.Add(slotElement);

        InventorySlot slot = new InventorySlot(i, slotElement, tabSell, SlotType.PriceSell);
        return slot;
    }

    private InventorySlot[] CreateNewArray(int count)
    {
            return  new InventorySlot[count];       
    }

    private InventorySlot[] ReCreateElseChangeCount(InventorySlot[] _inventorySlotsSell, int count)
    {
        if (count > _inventorySlotsSell.Length)
        {
            InventorySlot[] newArray = new InventorySlot[count];
            return newArray;
        }else if (count < _inventorySlotsSell.Length)
        {
            InventorySlot[] newArray = new InventorySlot[count];
            return newArray;
        }

        return _inventorySlotsSell;
    }



    public static InventorySlot[] ExpandArray(InventorySlot[] originalArray, int additionalCount)
    {

        if (originalArray == null)
        {
            throw new ArgumentNullException(nameof(originalArray), "Original array cannot be null.");
        }

        if (additionalCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(additionalCount), "Additional count cannot be negative.");
        }


        InventorySlot[] newArray = new InventorySlot[originalArray.Length + additionalCount];

        return newArray;
    }

    public int GetNextMultipleOfSix(int number)
    {

        int remainder = number % 6;


        if (remainder == 0)
        {
            return number;
        }


        return number + (6 - remainder);
    }

    public SlotType DetectedClickPanel(int type)
    {

        if (type == (int)SlotType.PriceBuy)
        {
            return SlotType.PriceBuy;
        }
        else if (type == (int)SlotType.PriceSell)
        {
            return SlotType.PriceSell;
        }
        return SlotType.Other;
    }


    public List<Product> AddProduct(List<Product> listSell, Product source)
    {
        if (listSell == null)
        {
            return new List<Product>() { source };
        }


        listSell.Add(source);


        return listSell;
    }

    public List<Product> RemoveProduct(List<Product> listSell, Product source)
    {
        if (listSell == null)
        {
            return new List<Product>() { source };
        }

        listSell.Remove(source);
        return listSell;
    }

    public void ClearSell(InventoryTab tabSell)
    {
        if (tabSell != null)
        {
            tabSell.UnSelectSlot();
            tabSell.ResetSelectSlot();
        }

    }

    public void ClearBuy(InventorySlot[] inventorySlotsBuy, InventoryTab tabBuy)
    {
        if (inventorySlotsBuy != null && inventorySlotsBuy.Count() > 0)
        {
            for (int i = 0; i < inventorySlotsBuy.Count(); i++)
            {
                InventorySlot i_slot = inventorySlotsBuy[i];
                i_slot.AssignEmpty();
            }
        }
        tabBuy.UnSelectSlot();
        tabBuy.ResetSelectSlot();
        tabBuy.UpdateInventorySlots(inventorySlotsBuy);

    }

    public InventorySlot[] CreateEmptyBuy(InventoryTab tabBuy , int defaultCount , VisualElement contentBuy , VisualTreeAsset InventorySlotTemplate)
    {
        var inventorySlotsBuy = new InventorySlot[defaultCount];
        for (int i = 0; i < inventorySlotsBuy.Count(); i++)
        {
            VisualElement slotElement = InventorySlotTemplate.Instantiate()[0];
            contentBuy.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, slotElement, tabBuy, SlotType.PriceBuy);
            inventorySlotsBuy[i] = slot;
        }

        tabBuy.UpdateInventorySlots(inventorySlotsBuy);

        return inventorySlotsBuy;
    }

}
