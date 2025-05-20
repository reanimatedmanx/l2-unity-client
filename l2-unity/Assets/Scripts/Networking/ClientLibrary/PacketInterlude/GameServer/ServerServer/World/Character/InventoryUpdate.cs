using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.Progress;

public class InventoryUpdate : ServerPacket
{
    private ItemInstance[] items;
    public InventoryUpdate(byte[] d) : base(d)
    {
        Parse();
    }

    public ItemInstance[] Items { get { return items; } }
    public override void Parse()
    {
        int size =  ReadSh();
        items = new ItemInstance[size];
        for (int i = 0; i < size; i++)
        {
            // Update type : 01-add, 02-modify, 03-remove
            int type = ReadSh();

            int type1 = ReadSh();
            int objectId = ReadI();
            int displayId = ReadI();
            int count = ReadI();
           // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
            int type2 = ReadSh();
            // Filler (always 0)
            int customType1 = ReadSh();
            int equipped = ReadSh();
            int bodyPart = ReadI();
            int enchant = ReadSh();
            int customType2 = ReadSh();
            int augmentationLevel = ReadI();
            int mana = ReadI();

            ItemLocation location = ItemLocation.Inventory;
            ItemCategory category = ItemsType.ParceCategory(type2);
            ItemSlot slot = ItemsType.ParceSlot(bodyPart);

            if (equipped == 1)
            {
                location = ItemLocation.Equipped;
            }

            items[i] = new ItemInstance(objectId, displayId, location, i, count, category, equipped == 1 , slot , enchant, 9999);
            items[i].LastChange = type;
        }
    }
}
