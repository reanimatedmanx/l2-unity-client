using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsType 
{
    public static ItemCategory ParceCategory(int type2)
    {
        if (type2 == 0)
        {
            return ItemCategory.Weapon;
        }
        else if (type2 == 1)
        {
            return ItemCategory.ShieldArmor;
        }
        else if (type2 == 2)
        {
            return ItemCategory.Jewel;
        }
        else if (type2 == 3)
        {
            return ItemCategory.Quest;
        }
        else if (type2 == 4)
        {
            return ItemCategory.Adena;
        }
        else if (type2 == 5)
        {
            return ItemCategory.Item;
        }

        return ItemCategory.Item;

    }
    // Slot : 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
    public static ItemSlot ParceSlot(int slot)
    {
        if (slot == 8)
        {
            return ItemSlot.neck;
            //l2j
        }
        else if (slot == 64)
        {
            return ItemSlot.head;
        }
        else if (slot == 100)
        {
            return ItemSlot.lhand;
        } //l2j
        else if (slot == 512)
        {
            return ItemSlot.gloves;
        }//l2j
        else if (slot == 1024)
        {
            return ItemSlot.chest;
        }//l2j
        else if (slot == 128)
        {
            return ItemSlot.rhand;
        }//l2j
        else if (slot == 6)
        {
            return ItemSlot.lear;
        }
        else if (slot == 1000)
        {
            return ItemSlot.feet;
        }//l2j
        else if (slot == 2048)
        {
            return ItemSlot.legs;
        }//l2j
        else if (slot == 4096)
        {
            return ItemSlot.boots;
        }
        //l2j
        else if (slot == 48)
        {
            return ItemSlot.lfinger;
        }
        //l2j
        else if (slot == 256)
        {
            return ItemSlot.lhand;
        }

        return ItemSlot.none;

    }
}
