using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.Progress;

public class ChooseInventoryItem : ServerPacket
{

    private int _itemId;
    private ItemInstance _item;

    public ItemInstance Item { get { return _item; } }
    public ChooseInventoryItem(byte[] d) : base(d)
    {
        Parse();
    }
    public override void Parse()
    {

        _itemId = ReadI();
        _item =  new ItemInstance(-1, _itemId, ItemLocation.Inventory, -1, 1, ItemCategory.Item, false, ItemSlot.none, 0, 9999);
    }
}
