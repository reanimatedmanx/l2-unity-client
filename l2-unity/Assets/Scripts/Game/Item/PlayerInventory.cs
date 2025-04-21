using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using System;
using static StorageVariable;

public class PlayerInventory : MonoBehaviour
{
    public enum InventoryChange
    {
        UNCHANGED = 0, ADDED = 1, REMOVED = 3, MODIFIED = 2
    }

    private List<ItemInstance> _playerInventory;

    public List<ItemInstance> Items { get { return _playerInventory; } }

    public static PlayerInventory _instance;
    public static PlayerInventory Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }

        _playerInventory = new List<ItemInstance>();
    }

    private void Start()
    {
        _playerInventory.Clear();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public void SetInventory(ItemInstance[] items, bool openInventory)
    {
        _playerInventory = items.ToList();

        InventoryWindow.Instance.UpdateItemList(_playerInventory);

        if (openInventory)
        {
            InventoryWindow.Instance.ShowWindow();
        }
    }

    public void UpdateInventory(ItemInstance[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            ItemInstance item = items[i];
            if (item.LastChange == (int)InventoryChange.ADDED)
            {
                _playerInventory.Add(item);
            }
            else if (item.LastChange == (int)InventoryChange.MODIFIED)
            {
                ItemInstance oldItem = GetItemByObjectId(item.ObjectId);
                if (oldItem == null)
                {
                    //count info  to message system
                    StorageVariable.getInstance().AddS1Items(new VariableItem(item.Count.ToString(), item.ObjectId));
                    _playerInventory.Add(item);
                }
                else
                {
                    //count info  to message system
                    int newCount = item.Count - oldItem.Count;
                    StorageVariable.getInstance().AddS1Items(new VariableItem(newCount.ToString(), item.ObjectId));
                    StorageVariable.getInstance().ResumeShowDelayMessage((int)MessageID.ADD_INVENTORY);
                    oldItem.Update(item);
                }
            }
            else if (item.LastChange == (int)InventoryChange.REMOVED)
            {
                ItemInstance oldItem = GetItemByObjectId(item.ObjectId);
                _playerInventory.Remove(oldItem);
            }
        }
        StorageItems.getInstance().AddItems(_playerInventory.ToArray());
        InventoryWindow.Instance.UpdateItemList(_playerInventory);
    }

    public ItemInstance GetItemByObjectId(int objectId)
    {
        foreach (ItemInstance item in _playerInventory)
        {
            if (item.ObjectId == objectId)
            {
                return item;
            }
        }

        return null;
    }

    public ItemInstance GetItemBySlot(int slot)
    {
        foreach (ItemInstance item in _playerInventory)
        {
            if (item.Slot == slot && !item.Equipped)
            {
                return item;
            }
        }

        return null;
    }

    public void ChangeItemOrder(int fromSlot, int toSlot)
    {
        ItemInstance fromItem = GetItemBySlot(fromSlot);
        ItemInstance toItem = GetItemBySlot(toSlot);

        if (fromItem == null)
        {
            Debug.LogError($"Can't change item order, item not found at slot {fromSlot}.");
            return;
        }

        List<InventoryOrder> orders = new List<InventoryOrder>();
        orders.Add(new InventoryOrder(fromItem.ObjectId, toSlot));

        if (toItem != null)
        {
            orders.Add(new InventoryOrder(toItem.ObjectId, fromSlot));
        }

        InventoryWindow.Instance.SelectSlot(toSlot);
        Debug.Log("Нужно реализовать пакеты для отпарвки на сервер");
        //GameClient.Instance.ClientPacketHandler.UpdateInventoryOrder(orders);
    }

    public void UseItem(int objectId)
    {
        //Cache Name for Message Equip
        StorageVariable.getInstance().AddS1Items(new VariableItem(GetItemByObjectId(objectId).ItemData.ItemName.Name , objectId));

        var sendPaket = CreatorPacketsUser.CreateUseItem(objectId, 0);
        bool enable = GameClient.Instance.IsCryptEnabled();
        SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
    }

    public void DestroyItem(int objectId, int quantity)
    {
        //Cache Name for Message Equip
        StorageVariable.getInstance().AddS1Items(new VariableItem(GetItemByObjectId(objectId).ItemData.ItemName.Name, objectId));

        AudioManager.Instance.PlayEquipSound("trash_basket");
        var sendPaket = CreatorPacketsUser.CreateDestroyItem(objectId, quantity);
        bool enable = GameClient.Instance.IsCryptEnabled();
        SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
        //GameClient.Instance.ClientPacketHandler.DestroyItem(objectId, quantity);
    }
}
