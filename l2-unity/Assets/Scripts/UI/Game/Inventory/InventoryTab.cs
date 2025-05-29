using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using static PlayerInventory;
using static UnityEditor.Progress;

[System.Serializable]
public class InventoryTab : L2Tab
{
    private InventorySlot[] _inventorySlots;
    [SerializeField] private int _selectedSlot = -1;
    private int _itemCount = 0;

    private VisualElement _contentContainer;
    public List<ItemCategory> _filteredCategories;
    public bool MainTab { get; internal set; }

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader, bool initEmpty, bool isSwitchTab)
    {
        base.Initialize(chatWindowEle, tabContainer, tabHeader, initEmpty, isSwitchTab);

        _selectedSlot = -1;
        _contentContainer = tabContainer.Q<VisualElement>("Content");
        CreateEmptyInventory(initEmpty);
    }

    public void UpdateInventorySlots(InventorySlot[] inventory)
    {
        _inventorySlots = null;
        _inventorySlots = inventory;
    }


    private void CreateEmptyInventory(bool initEmty)
    {
        if (_contentContainer != null && initEmty)
        {

            _contentContainer.Clear();

            int slotCount = InventoryWindow.PLAYER_INVENTORY_SIZE;
            _inventorySlots = new InventorySlot[slotCount];
            SlotType slotType = GetSlotType(MainTab);

            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                if (i == _inventorySlots.Length - 1)
                {
                   // Debug.Log(" slot int i " + i);
                    VisualElement disabledElement = CreateVisualElementDisabled();
                    _contentContainer.Add(disabledElement);
                }
                else
                {
                    VisualElement slotElement = CretaVisualElement();
                    InventorySlot slot = CreateInventorySlot(i, slotElement, slotType);
                    _contentContainer.Add(slotElement);
                   _inventorySlots[i] = slot;
                }

            }
            UpdateInventorySlots(_inventorySlots);
        }

    }

    private VisualElement CretaVisualElement()
    {
        return InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
    }

    private VisualElement CreateVisualElementDisabled()
    {
        VisualElement slotElement = CretaVisualElement();
        slotElement.AddToClassList("inventory-slot");
        slotElement.AddToClassList("disabled");
        return slotElement;
    }
    private InventorySlot CreateInventorySlot(int i , VisualElement slotElement , SlotType slotType)
    {
        return new InventorySlot(i, slotElement, this, slotType);
    }

    private SlotType GetSlotType(bool mainTab)
    {
        L2Slot.SlotType slotType = L2Slot.SlotType.Inventory;

        if (!mainTab)
        {
            slotType = L2Slot.SlotType.InventoryBis;
        }
        return slotType;
    }
    //temporarily disabling
    private void UpdatePadSlots()
    {
        int slotCount = InventoryWindow.Instance.SlotCount;

        // Add disabled slot to fill up the window
        int rowLength = 9;
        if (InventoryWindow.Instance.Expanded)
        {
            rowLength = 12;
        }

        int padSlot = 0;
        if (slotCount < 8 * rowLength)
        {
            padSlot = 8 * rowLength - slotCount;
        }
        else if (slotCount % rowLength != 0)
        {
            padSlot = rowLength - slotCount % rowLength;
        }

        for (int i = 0; i < padSlot; i++)
        {
            VisualElement slotElement = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
            slotElement.AddToClassList("inventory-slot");
            slotElement.AddToClassList("disabled");
            _contentContainer.Add(slotElement);
        }
    }
    public void SetItemList(List<ItemInstance> allItems)
    {

        if(allItems != null && allItems.Count > 0)
        {
            AddDataInventory(allItems);
            AddAllEmptyInventory(allItems.Count);
        }
        else
        {
            AddAllEmptyInventory(0);
        }


       // Debug.Log(" AssignItem конец итерации размер +++++++++++++++++++++++++>" + allItems.Count);
    }

    private void AddDataInventory(List<ItemInstance> allItems)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemInstance item = allItems[i];
            item.SetSlot(i);
            Debug.Log("AssignItem Set Inventory>>>> " + item.ItemId + " ObjectId " + item.ObjectId + " Add Slot " + item.Slot);
            _inventorySlots[i].AssignItem(item);
        }
    }
    private void AddAllEmptyInventory(int size)
    {
        int start_i = size;

        for (int i = start_i; i < _inventorySlots.Length; i++)
        {
            var slot = _inventorySlots[i];

            if (slot != null)
            {
                slot.AssignEmpty();
            }
               
        }
    }
    public void UpdateItemList(List<ItemInstance> removeAndAdd , List<ItemInstance> modified)
    {
        AddAndRemove(removeAndAdd);
        Modified(modified);

        // Clear slots
        // if (_inventorySlots != null)
        // {
        //  foreach (InventorySlot slot in _inventorySlots)
        // {
        //  if(slot != null)
        //  {
        //      slot.UnregisterClickableCallback();
        //      slot.ClearManipulators();
        // }

        // }
        // }


        //_itemCount = 0;

        // Assign items to slots
        //items.ForEach(item =>
        // {
        // if (item.Location == ItemLocation.Inventory)
        // {
        // if (_filteredCategories == null || _filteredCategories.Count == 0)
        // {
        //   _inventorySlots[item.Slot].AssignItem(item);
        //   _itemCount++;
        //}
        //else if (_filteredCategories.Contains(item.Category))
        // {
        //    _inventorySlots[_itemCount++].AssignItem(item);
        //}
        // }
        //});

        if (_selectedSlot != -1)
        {
            SelectSlot(_selectedSlot);
        }
    }


    private void AddAndRemove(List<ItemInstance> removeAndAdd)
    {

        for (int i = 0; i < removeAndAdd.Count; i++)
        {
            ItemInstance item = removeAndAdd[i];
            if (item.LastChange == (int)InventoryChange.ADDED)
            {   
                if (!PlayerInventory.Instance.IsContaineInventory(item.ObjectId))
                {
                    //Debug.Log("ADD Inventory Slot slot-position update inventory <<<<< " + item.Slot + " position " + item + " object id " + item.ObjectId);

                    InventorySlot i_slot = _inventorySlots[item.Slot];

                    if (i_slot != null)
                    {
                        i_slot.AssignItem(item);
                    }
         
                }
                else
                {
                    Debug.Log("ADD And Rmove Count Not ADD ObjectID " + item.ObjectId);
                }
          
            }
            else if(item.LastChange == (int)InventoryChange.REMOVED)
            {
                InventorySlot slot = GetInventorySlot(item.ObjectId);
                if (slot != null)
                {
                    //Debug.Log("Remove new Object 1 Inventory Tab " + item.ItemId + " objId " + slot.ObjectId + " posiion " + slot.Position);
                    _inventorySlots[slot.Position].AssignEmpty();
                    //Debug.Log("use Shift elements 1 position  " + slot.Position);
                    ShiftElements.ShiftElementsLeft(_inventorySlots, slot.Position);
                }
            }
        }

    }

    private void Modified(List<ItemInstance> modified)
    {
        for (int i = 0; i < modified.Count; i++)
        {
            ItemInstance item = modified[i];
            if (item.LastChange == (int)InventoryChange.MODIFIED)
            {
                Debug.Log("Update inventory 1 Inventory Tab Modified " + item.ItemId);
                InventorySlot slot = GetInventorySlot(item.ObjectId);
                if (slot != null)
                {
                    _inventorySlots[slot.Position].AssignItem(item);
                }
            }

        }
    }

    public InventorySlot GetInventorySlot(int objectId)
    {
        return _inventorySlots?
                               .Where(slot => slot != null && slot.ObjectId == objectId)
                               .FirstOrDefault();
    }

    public InventorySlot GetFreeSlot()
    {
        //return _inventorySlots.FirstOrDefault(slot => slot.ObjectId == 0);

        return _inventorySlots?
                               .Where(slot => slot != null && slot.ObjectId == 0)
                               .FirstOrDefault();
    }

    public override void SelectSlot(int slotPosition)
    {
        if (_selectedSlot != -1)
        {
            //It happens that the panel will become smaller and the index may be larger than the current panel, so you need to check
            if (IsValidIndex(_inventorySlots, _selectedSlot)){
                _inventorySlots[_selectedSlot].UnSelect();
            }

        }
        _inventorySlots[slotPosition].SetSelected();
        _selectedSlot = slotPosition;
    }

    public void UnSelectSlot()
    {
        if (_selectedSlot != -1)
        {
            _inventorySlots[_selectedSlot].UnSelect();
        }
    }

    public void ResetSelectSlot()
    {
        _selectedSlot = -1;
    }

    private bool IsValidIndex(Array array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    protected override void OnGeometryChanged()
    {
    }

    protected override void OnSwitchTab()
    {
        if (InventoryWindow.Instance.SwitchTab(this))
        {
            AudioManager.Instance.PlayUISound("window_open");
        }
    }

    protected override void RegisterAutoScrollEvent()
    {
    }
}
