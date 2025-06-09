using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemInstance
{
    [SerializeField] AbstractItem _itemData;
    [SerializeField] private int _objectId;
    [SerializeField] private int _itemId;
    [SerializeField] private ItemLocation _location;
    [SerializeField] private int _slot;
    [SerializeField] private int _count;
    [SerializeField] private ItemCategory _category;
    [SerializeField] private bool _equipped;
    [SerializeField] private ItemSlot _bodyPart;
    [SerializeField] private int _enchantLevel;
    [SerializeField] private long _remainingTime;
    [SerializeField] private int _lastChange;

    public AbstractItem ItemData { get { return _itemData; } }
    public int ObjectId { get { return _objectId; } }
    public int ItemId { get { return _itemId; } }
    public ItemLocation Location { get { return _location; } }
    public bool Equipped { get { return _equipped; } }
    public int Slot { get { return _slot; } }
    public int Count { get { return _count; } }
    public ItemCategory Category { get { return _category; } }
    public ItemSlot BodyPart { get { return _bodyPart; } }
    public int EnchantLevel { get { return _enchantLevel; } }
    public long RemainingTime { get { return _remainingTime; } }
    public int LastChange { get { return _lastChange; } set { _lastChange = value; } }

    public ItemInstance(int objectId, int itemId, ItemLocation location, int slot, int count, ItemCategory category, bool equipped, ItemSlot bodyPart, int enchantLevel, long remainingTime)
    {
        _objectId = objectId;
        _itemId = itemId;
        _location = location;
        _slot = slot;
        _count = count;
        _category = category;
        _equipped = equipped;
        _bodyPart = bodyPart;
        _remainingTime = remainingTime;
        _enchantLevel = enchantLevel;


        if (_category == ItemCategory.Weapon)
        {
            _itemData = ItemTable.Instance.GetWeapon(_itemId);
        }
        else if (_category == ItemCategory.ShieldArmor || _category == ItemCategory.Jewel)
        {
            if (bodyPart != ItemSlot.lhand)
            {
                _itemData = ItemTable.Instance.GetArmor(_itemId);
            }
            else
            {
                _itemData = ItemTable.Instance.GetWeapon(_itemId);
            }
        }
        else
        {
            _itemData = ItemTable.Instance.GetEtcItem(_itemId);
        }

        //Debug.Log(this.ToString());
    }
    public void SetSlot(int slot)
    {
        _slot = slot;
    }
    public void Update(ItemInstance newItem)
    {
        _location = newItem.Location;
        _slot = newItem.Slot;
        _count = newItem.Count;
        _remainingTime = newItem.RemainingTime;
        _enchantLevel = newItem.EnchantLevel;

        if (_equipped == false && newItem.Equipped == true
        || _equipped == true && newItem.Equipped == false)
        {
            //AudioManager.Instance.PlayEquipSound(_itemData.Itemgrp.EquipSound);
        }

        _equipped = newItem.Equipped;
        _objectId = newItem.ObjectId;
        _bodyPart = newItem.BodyPart;
    }

    public override string ToString()
    {
        return $"New item: ServerId:{_objectId} ItemId:{_itemId} Location:{_location} Slot:{_slot} Count:{_count} " +
        $"Cat:{_category} Equipped:{_equipped} Bodypart:{_bodyPart} Change:{_lastChange}";
    }

    public override bool Equals(object obj)
    {
        // Проверяем, является ли объект тем же самым экземпляром
        if (ReferenceEquals(this, obj)) return true;

        // Проверяем, является ли объект того же типа
        if (obj is ItemInstance other)
        {
            // Сравниваем ключевые поля для определения равенства
            return _objectId == other._objectId && _itemId == other._itemId;
        }

        return false;
    }

    public override int GetHashCode()
    {

        unchecked 
        {
            int hash = 17; 
            hash = hash * 23 + _objectId.GetHashCode(); 
            hash = hash * 23 + _itemId.GetHashCode(); 
            return hash;
        }
    }

    public bool EqualsBodyPart(ItemSlot bodyPartSource)
    {
        if (IsLRHand(bodyPartSource))
        {
            return true;
        }

        if (IsFullPlate(bodyPartSource))
        {
            return true;
        }

        if (_bodyPart == bodyPartSource)
        {
            return true;
        }

        return false;
    }



    private bool IsLRHand(ItemSlot bodyPartSource)
    {
        if (_bodyPart == ItemSlot.lrhand)
        {
            if (bodyPartSource == ItemSlot.rhand | bodyPartSource == ItemSlot.lhand)
            {
                return true;
            }
        }else if (bodyPartSource == ItemSlot.lrhand)
        {
            if (_bodyPart == ItemSlot.rhand | _bodyPart == ItemSlot.lhand)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsFullPlate(ItemSlot bodyPartSource)
    {
        if (_bodyPart == ItemSlot.chest | _bodyPart == ItemSlot.legs)
        {
            if (bodyPartSource == ItemSlot.fullarmor)
            {
                return true;
            }
        }
        else if (bodyPartSource == ItemSlot.chest | bodyPartSource == ItemSlot.legs)
        {
            if (_bodyPart == ItemSlot.fullarmor)
            {
                return true;
            }
        }

        return false;
    }

    public string GetName()
    {
        if (_category == ItemCategory.Weapon)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;

        }
        else if (_category == ItemCategory.ShieldArmor)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        else if (_category == ItemCategory.Jewel)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        else if (_category == ItemCategory.Item)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        else if (_category == ItemCategory.Adena)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        return "";
    }

    public string GetDescription()
    {
        return "";
    }

    public string GetItemDescription()
    {
        ItemName item = ItemNameTable.Instance.GetItemName(_itemId);
        if (string.IsNullOrEmpty(item.Description)) return "Not Found description";
        return item.Description;
    }

}
