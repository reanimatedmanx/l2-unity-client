using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

[System.Serializable]
public class InventoryGearTab : L2Tab
{
    private Dictionary<int, ItemSlot> _gearCellIds;
    private Dictionary<ItemSlot, GearItem> _gearAnchors;
    [SerializeField] private int _selectedSlot = -1;

    public InventoryGearTab()
    {
        _gearAnchors = new Dictionary<ItemSlot, GearItem> ();
        _gearCellIds = new Dictionary<int, ItemSlot>();
    }

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader , bool initEmpty, bool isSwitchTab)
    {
        base.Initialize(chatWindowEle, tabContainer, tabHeader , initEmpty, isSwitchTab);

        _selectedSlot = -1;

        _gearAnchors?.Clear();
        InitEmptyGear();

    }

  
    private void InitEmptyGear()
    {
        _gearAnchors = new Dictionary<ItemSlot, GearItem>
        {
             { ItemSlot.hat1, new GearItem(_windowEle.Q<VisualElement>("Hat1")  , ItemSlot.hat1 ) },
            { ItemSlot.hat2, new GearItem(_windowEle.Q<VisualElement>("Hat2")  , ItemSlot.hat2 ) },
            { ItemSlot.head, new GearItem(_windowEle.Q<VisualElement>("Helmet")  , ItemSlot.head ) },
            { ItemSlot.gloves, new GearItem(_windowEle.Q<VisualElement>("Gloves"), ItemSlot.gloves) },
            { ItemSlot.chest, new GearItem(_windowEle.Q<VisualElement>("Torso"), ItemSlot.chest) },
            { ItemSlot.feet, new GearItem(_windowEle.Q<VisualElement>("Boots"), ItemSlot.feet) },
            { ItemSlot.underwear, new GearItem(_windowEle.Q<VisualElement>("Underwear"), ItemSlot.underwear) },
            { ItemSlot.legs, new GearItem(_windowEle.Q<VisualElement>("Legs"), ItemSlot.legs) },
            { ItemSlot.belt, new GearItem(_windowEle.Q<VisualElement>("Belt"), ItemSlot.belt) },
            { ItemSlot.rhand, new GearItem(_windowEle.Q<VisualElement>("Rhand"), ItemSlot.rhand) },
            { ItemSlot.lhand, new GearItem(_windowEle.Q<VisualElement>("Lhand"), ItemSlot.lhand) },
            { ItemSlot.agation, new GearItem(_windowEle.Q<VisualElement>("Agathion"), ItemSlot.agation) },
            { ItemSlot.neck, new GearItem(_windowEle.Q<VisualElement>("Neck"), ItemSlot.neck) },
            { ItemSlot.rear, new GearItem(_windowEle.Q<VisualElement>("Rear"), ItemSlot.rear) },
            { ItemSlot.lear, new GearItem(_windowEle.Q<VisualElement>("Lear"), ItemSlot.lear) },
            { ItemSlot.boots, new GearItem(_windowEle.Q<VisualElement>("Boots"), ItemSlot.boots) },
            { ItemSlot.rfinger, new GearItem(_windowEle.Q<VisualElement>("Rring"), ItemSlot.rfinger) },
            { ItemSlot.lfinger, new GearItem(_windowEle.Q<VisualElement>("Lring"), ItemSlot.lfinger) },
             { ItemSlot.pendant, new GearItem(_windowEle.Q<VisualElement>("Pendant"), ItemSlot.pendant) }

        };

        CreateGearSlot(_gearAnchors);
    }

    private void CreateGearSlot(Dictionary<ItemSlot, GearItem> _gearAnchors)
    {
        List<GearItem> gearList = _gearAnchors.Values.ToList();
        for (int i = 0; i < gearList.Count; i++)
        {
            GearItem gearItem = gearList[i];
            _gearCellIds.Add(i, gearItem.GetSlotType());
            GearSlot slot = new GearSlot(i, gearItem.GetElement(), this, L2Slot.SlotType.Gear);
            gearItem.SetGearSlot(slot);
        }
    }

    public void UpdateEquipList(List<ItemInstance> equipItems)
    {
        Equip(equipItems);

        //Debug.Log("Update gear slots");

        // Clean up slot callbacks and manipulators
        //if (_gearSlots != null)
        //{
        //foreach (KeyValuePair<ItemSlot, GearSlot> kvp in _gearSlots)
        // {
        //if (kvp.Value != null)
        //{
        //    kvp.Value.UnregisterClickableCallback();
        //    kvp.Value.ClearManipulators();
        //}
        // }
        // _gearSlots.Clear();
        //}

        //_gearSlots = new Dictionary<ItemSlot, GearSlot>();
        // Clean up gear anchors from any child visual element
        //foreach (KeyValuePair<ItemSlot, VisualElement> kvp in _gearAnchors)
        //{
        // if (kvp.Value == null)
        // {
        //   Debug.LogWarning($"Inventory gear slot {kvp.Key} is null.");
        //   continue;
        // }

        // Clear gear slots
        //kvp.Value.Clear();

        // Create gear slots
        //VisualElement slotElement = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
        //kvp.Value.Add(slotElement);

        //GearSlot slot = new GearSlot((int)kvp.Key, slotElement, this, L2Slot.SlotType.Gear);
        //_gearSlots.Add(kvp.Key, slot);
        //}



        if (_selectedSlot != -1)
        {
            SelectSlot(_selectedSlot);
        }
    }

    private void Equip(List<ItemInstance> equipItems)
    {
        equipItems.ForEach(item =>
        {
            if (item.Equipped)
            {
                //Debug.Log("Equip item: " + item);
                switch (item.BodyPart)
                {
                    case ItemSlot.lrhand:
                        AddGearSlotAssign(ItemSlot.lhand, item);
                        AddGearSlotAssign(ItemSlot.rhand, item);
                        break;

                    case ItemSlot.fullarmor:
                        AddGearSlotAssign(ItemSlot.chest, item);
                        AddGearSlotAssign(ItemSlot.legs, item);
                        break;

                    case ItemSlot.lfinger:
                        if (GearSlotIsEmpty(item.BodyPart))
                        {
                            AddGearSlotAssign(ItemSlot.lfinger, item);
                        }
                        else
                        {
                            AddGearSlotAssign(ItemSlot.rfinger, item);
                        }
                        break;

                    case ItemSlot.lear:
                        
                        if (GearSlotIsEmpty(item.BodyPart))
                        {
                            AddGearSlotAssign(ItemSlot.lear, item);
                        }
                        else
                        {
                            AddGearSlotAssign(ItemSlot.rear, item);
                        }
                        break;

                    default:
                        //Debug.Log("Item id >>>>> " + item.ItemId);
                        //Debug.Log("Slot ID  >>>>> " + item.Slot);
                        //Debug.Log("BodyPart ID  >>>> " + item.BodyPart);
                        ItemSlot slot = item.BodyPart;
                        if (slot != ItemSlot.none)
                        {
                            if (_gearAnchors.ContainsKey(slot))
                            {
                                AddGearSlotAssign(slot , item);
                            }
                            else
                            {
                                Debug.LogError("GearSlots reInitialize not found assigned slots " + slot);
                            }
                        }
                        else
                        {
                            Debug.LogError("Can't equip item, assigned slot is " + slot);
                        }
                        break;
                }
            }
        });
    }


    public void AddGearSlotAssign(ItemSlot slotType , ItemInstance item)
    {
        if (_gearAnchors.ContainsKey(slotType)){
            _gearAnchors[slotType].Assign(item);
        }
    }

    public bool GearSlotIsEmpty(ItemSlot slotType)
    {
        if (_gearAnchors.ContainsKey(slotType)){
            return _gearAnchors[slotType].IsEmptyGearSlot();
        }
        return false;
    }

    public override void SelectSlot(int slotPosition)
    {
        GearItem gearItem = ConvertIdToGearType(slotPosition);

        if (gearItem != null)
        {
            if (_selectedSlot != -1)
            {
                GearItem gearLastSelect = ConvertIdToGearType(_selectedSlot);
                gearLastSelect.UnSelectGear();
            }

            gearItem.SetSelectGear();
            _selectedSlot = slotPosition;
        }

    }

  
    private GearItem ConvertIdToGearType(int slotPosition)
    {
        if (_gearCellIds.ContainsKey(slotPosition))
        {
            ItemSlot slotSelect = _gearCellIds[slotPosition];
            GearItem gearitem =  _gearAnchors[slotSelect];
            return gearitem;
        }
        return null;
    }
}
