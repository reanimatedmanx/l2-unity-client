using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemList : ServerPacket
{
    private ItemInstance[] items;
    private bool showWindow;

    public bool ShowWindow {  get { return showWindow; } }
    public ItemInstance[] Items { get { return items; } }
    public ItemList(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        showWindow = ReadSh() == 1;
        int size = ReadSh();
        items = new ItemInstance[size];
        for(int i = 0; i < size; i++)
        {
            
            int type1 = ReadSh();
            int objectId = ReadI();
            int displayId = ReadI();// ItemId
            int count = ReadI(); // Quantity
            int type2 = ReadSh();// Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
            int customType = ReadSh();// Filler (always 0)
            int equipped = ReadSh();// Equipped : 00-No, 01-yes
            int bodyPart = ReadI();// Slot : 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
            int enchant = ReadSh();// Enchant level (pet level shown in control item)
            int custonType2 = ReadSh();// Pet name exists or not shown in control item
            int augmentationBonus = ReadI();
            int mana = ReadI();
            ItemLocation location = ItemLocation.Inventory;
            ItemCategory category = ItemsType.ParceCategory(type2);
            ItemSlot slot = ItemsType.ParceSlot(bodyPart);
            //Debug.Log("ITEMMM SLOT " + slot.ToString() + "Dispaly ID ");

            if (equipped == 1)
            {
                location = ItemLocation.Equipped;
            }

            items[i] = new ItemInstance(objectId , displayId , location , i, count , category, equipped == 1, slot , enchant, 9999);
        }
    }

    
}
