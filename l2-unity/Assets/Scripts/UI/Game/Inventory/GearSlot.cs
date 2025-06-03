
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GearSlot : InventorySlot
{
    public GearSlot(int position, VisualElement slotElement, InventoryGearTab tab, SlotType slotType) : base(position, slotElement, tab, slotType , false)
    {
    }

    protected override void HandleRightClick()
    {
        UseItem();
    }

    public override void UseItem()
    {
        //if (!_empty)
        //{
            ItemInstance item = PlayerInventory.Instance.GetItemEquip(ObjectId);
            if (item != null)
            {
                AddCacheName(item, ObjectId);

                var sendPaket = CreatorPacketsUser.CreateUseItem(ObjectId, 0);
                bool enable = GameClient.Instance.IsCryptEnabled();
                SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
            }

        //}
    }

    private void AddCacheName(ItemInstance item , int objectId)
    {
        if (item != null)
        {
            StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, objectId));
        }
    }
}
