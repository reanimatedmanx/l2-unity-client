using NUnit.Framework;
using System.Collections.Generic;
using static UnityEditor.Progress;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;


public class BuyList : ServerPacket
{
    private int _money;
    private int _listId;
    public int _size;
    private List<Product> _listProduct;
    public List<Product> Products { get => _listProduct; }
    public int CurrentMoney { get => _money; }
    public int  ListID { get => _listId; }
    public BuyList(byte[] d) : base(d)
    {
        _listProduct = new List<Product>();
        Parse();
    }

    public override void Parse()
    {
        _money = ReadI();
        _listId = ReadI();
        _size = ReadSh();

        for(int i = 0; i < _size; i++)
        {
            int itemType1 = ReadSh();
            int objId = ReadI();
            int itemId = ReadI();
            int count = ReadI();

            int itemType2 = ReadSh();
            int isEquip = ReadSh();

            int bodyPart = ReadI();
            int enchant = ReadSh();
            int unknow1 = ReadSh();
            int unknow2 = ReadSh();

            int price = ReadI();

            _listProduct.Add(new Product(itemType1, objId, count, itemType2, isEquip, bodyPart, enchant, price , itemId));
        }

    }

    
}

//public static final int TYPE1_WEAPON_RING_EARRING_NECKLACE = 0;
//public static final int TYPE1_SHIELD_ARMOR = 1;
//public static final int TYPE1_ITEM_QUESTITEM_ADENA = 4;

//public static final int TYPE2_WEAPON = 0;
//public static final int TYPE2_SHIELD_ARMOR = 1;
//public static final int TYPE2_ACCESSORY = 2;
//public static final int TYPE2_QUEST = 3;
//public static final int TYPE2_MONEY = 4;
//public static final int TYPE2_OTHER = 5;

public enum EnumType2{
    TYPE2_WEAPON = 0,
    TYPE2_SHIELD_ARMOR = 1,
    TYPE2_ACCESSORY = 2,
    TYPE2_QUEST = 3,
    TYPE2_MONEY = 4,
    TYPE2_OTHER = 5,
    None = 6,
}


public class Product
{
    private int _itemType1;
    private int _objId;
    private int _count;
    private int _itemType2;
    private int _isEquip;
    private int _bodyPart;
    private int _enchant;
    private int _price;
    private int _itemId;
    public int ItemId { get { return _itemId; } }

    public int Count { get { return _count; } }

    public int Price { get { return _price; } }

    public int Type1 { get { return _itemType1; } }

    public int Type2 { get { return _itemType2; } }

    public int ObjId { get { return _objId; } }

    public void SetCount(int count)
    {
        _count = count;
    }

    public Product(int itemType1, int objId, int count, int itemType2, int isEquip, int bodyPart, int enchant, int price, int itemId)
    {
        _itemType1 = itemType1;
        _objId = objId;
        _count = count;
        _itemType2 = itemType2;
        _isEquip = isEquip;
        _bodyPart = bodyPart;
        _enchant = enchant;
        _price = price;
        _itemId = itemId;

       // Debug.Log("item id " + _itemId + " count " + _count + " _itemType2 " + _itemType2 + " _itemType1" + _itemType1);
    }


    public Product Clone()
    {
        return  new Product(_itemType1, _objId, _count, _itemType2, _isEquip,  _bodyPart,  _enchant, _price, _itemId);
    }

    public int GetWeight()
    {
        if(GetTypeItem() == EnumType2.TYPE2_WEAPON)
        {
            return WeapongrpTable.Instance.GetWeapon(_itemId).Weight;
        }
        else if (GetTypeItem() ==  EnumType2.TYPE2_ACCESSORY)
        {
            Armorgrp armpr = GetArmor();

            if (armpr != null)
            {
                return armpr.Weight;
            }
        }

        return 0;
    }

    public Abstractgrp GetOAbstractItem()
    {
         var armor =  GetArmor();
         var weapon = GetWeapon();
         if (armor != null) return armor;
         if (weapon != null) return weapon;

         return null;
    }

    public Armorgrp GetArmor()
    {
        return ArmorgrpTable.Instance.GetArmor(_itemId);
    }
    public Weapongrp GetWeapon()
    {
        return WeapongrpTable.Instance.GetWeapon(_itemId);
    }

    public EtcItemgrp GetEtcItem()
    {
        return EtcItemgrpTable.Instance.GetEtcItem(_itemId);
    }

    public EnumType2 GetTypeItem()
    {
        if (_itemType2 == (int)EnumType2.TYPE2_WEAPON)
        {
            return EnumType2.TYPE2_WEAPON;

        }
        else if (_itemType2 == (int)EnumType2.TYPE2_SHIELD_ARMOR)
        {
            return EnumType2.TYPE2_SHIELD_ARMOR;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_ACCESSORY)
        {
            return EnumType2.TYPE2_ACCESSORY;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_QUEST)
        {
            return EnumType2.TYPE2_QUEST;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_MONEY)
        {
            return EnumType2.TYPE2_MONEY;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_OTHER)
        {
            return EnumType2.TYPE2_OTHER;
        }
        return EnumType2.None;
    }
    public string GetName()
    {
        if(_itemType2 == (int)EnumType2.TYPE2_WEAPON)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;

        }else if (_itemType2 == (int)EnumType2.TYPE2_SHIELD_ARMOR)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_ACCESSORY)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_QUEST)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_MONEY)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        else if (_itemType2 == (int)EnumType2.TYPE2_OTHER)
        {
            return ItemNameTable.Instance.GetItemName(_itemId).Name;
        }
        return "";
    }

    public string GetTypeAccessoriesName()
    {
        Armorgrp armor = ArmorgrpTable.Instance.GetArmor(_itemId);

        if(armor != null)
        {
            if (armor.BodyPart == ItemSlot.neck)
            {
                return "Necklace";
            }
            else if (armor.BodyPart == ItemSlot.rear | armor.BodyPart == ItemSlot.lear)
            {
                return "Earring";
            }
            else if (armor.BodyPart == ItemSlot.rfinger | armor.BodyPart == ItemSlot.lfinger)
            {
                return "Ring";
            }
        }
     

        return "";
    }

    public string GetTypeWeaponName()
    {
        Weapongrp weapon = WeapongrpTable.Instance.GetWeapon(_itemId);

        if (weapon != null)
        {
            if (weapon.BodyPart == ItemSlot.lhand_shield)
            {
                return "Shield";
            }
        }


        return "";
    }


    public string GetTypeArmorName()
    {
        Armorgrp armor = ArmorgrpTable.Instance.GetArmor(_itemId);
        Debug.Log("Body part test > " + armor.BodyPart);
        if (armor != null)
        {
            if (armor.BodyPart == ItemSlot.legs)
            {
                if(armor.ArmorType == ArmorType.light) return "Lower Body / Light";
                if(armor.ArmorType == ArmorType.heavy) return "Lower Body / Heavy";

            }
            else if (armor.BodyPart == ItemSlot.chest)
            {
                if (armor.ArmorType == ArmorType.light) return "Upper Body / Light";
                if (armor.ArmorType == ArmorType.heavy) return "Upper Body / Heavy";
            }
            else if (armor.BodyPart == ItemSlot.boots)
            {
                return "Boots";
            }
            else if (armor.BodyPart == ItemSlot.feet)
            {
                return "Boots";
            }
            else if (armor.BodyPart == ItemSlot.head)
            {
                return "Helmet";
            }
            else if (armor.BodyPart == ItemSlot.gloves)
            {
                return "Gloves";
            }
        }


        return "";
    }

    public string GetDescription()
    {
        return _price.ToString();
    }

    public string GetItemDescription()
    {
        ItemName item = ItemNameTable.Instance.GetItemName(_itemId);
        if (string.IsNullOrEmpty(item.Description)) return "Not Found description";
        return item.Description;
    }

    public ItemName[] GetSets()
    {
        ItemName item = ItemNameTable.Instance.GetItemName(_itemId);
        return item.GetSetsName();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Product other = (Product)obj;
        return  other.ItemId == _itemId && other.ObjId == _objId;
    }

    public override int GetHashCode()
    {
        int _objId1 = _objId.GetHashCode();
        int _itemId1 = _itemId.GetHashCode();
        return _objId1 ^ _itemId1; 
    }


}


