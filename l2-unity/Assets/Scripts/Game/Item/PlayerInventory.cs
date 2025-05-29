using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

using static StorageVariable;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    public enum InventoryChange
    {
        UNCHANGED = 0, ADDED = 1, REMOVED = 3, MODIFIED = 2
    }

    private Dictionary<int , ItemInstance> _playerInventory;
    private Dictionary<int, ItemInstance> _playerEquipInventory;

    private List<ItemInstance> _tempForRemoveAndAdd;
    private List<ItemInstance> _tempForModified;


    public static PlayerInventory _instance;
    public static PlayerInventory Instance { get { return _instance; } }

    //public Dictionary<int, ItemInstance> GetPlayerInventory {  get { return _playerInventory; } }


    public Dictionary<int, ItemInstance>  GetPlayerInventory()
    {
        //printPacket(_playerInventory);
        return _playerInventory;
    }
    public Dictionary<int, ItemInstance> GetPlayerEquipInventory()
    {
        //printPacket(_playerInventory);
        return _playerEquipInventory;
    }
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
        _tempForRemoveAndAdd = new List<ItemInstance>();
        _tempForModified = new List<ItemInstance>();
        _playerInventory = new Dictionary<int, ItemInstance>();
    }

    private void Start()
    {
        _playerInventory.Clear();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public void SetInventory(Dictionary<int, ItemInstance> items, Dictionary<int, ItemInstance> equipItems , bool openInventory , int adenaCount)
    {
        _playerInventory = items;
        List<ItemInstance> items_collect =  _playerInventory.Values.ToList();
        List<ItemInstance> equip_collect = null;
        if (equipItems != null)
        {
            equip_collect = equipItems.Values.ToList();
            _playerEquipInventory = equipItems;
        }
        Debug.Log("Add new Object 2 Item ID Filter OK SET LIST COUNT " + items_collect.Count);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            InventoryWindow.Instance.SetItemList(items_collect, equip_collect, adenaCount);

            if (openInventory)
            {
                //EventProcessor.Instance.QueueEvent(() => InventoryWindow.Instance.ShowWindow());
                InventoryWindow.Instance.ShowWindow();
            }
        });
        

      
    }

 
    private readonly object _lock = new object();
    public void UpdateInventory(Dictionary<int, ItemInstance> items , Dictionary<int, ItemInstance> equipitems)
    {
        lock (_lock)
        {
            List<ItemInstance> listEquip = equipitems.Values.ToList();
            var items_f = items.Values.ToArray();
            var _tempForRemoveAndAdd = FilterByRemove(items_f);
            var _tempForModified = FilterByModified(items_f);

            try
            {

                UpdatePlayerInventory(_tempForRemoveAndAdd, _tempForModified);

                int adenaCount = GetAdenaCount(_playerInventory.Values.ToList());

                UnityMainThreadDispatcher.Instance().Enqueue(() => InventoryWindow.Instance.UpdateItemList(_tempForRemoveAndAdd, _tempForModified, adenaCount, _playerInventory.Count));
            

            }
            catch (Exception ex)
            {
                Debug.LogError("PlayerInventory Errors: " + ex.Message);
            }
        }
        
  
    }
    private int GetAdenaCount(List<ItemInstance> modified)
    {
        ItemInstance item =  modified.FirstOrDefault(o => o.Category == ItemCategory.Adena);
        return (item != null) ? item.Count : 0;
    }
    private void UpdatePlayerInventory(List<ItemInstance> removeAndAdd, List<ItemInstance> modified)
    {
        for (int i = 0; i < removeAndAdd.Count; i++)
        {
            ItemInstance item = removeAndAdd[i];
            AddAndRemove(_playerInventory, item);
        }

        for (int i = 0; i < modified.Count; i++)
        {
            ItemInstance item_m = modified[i];
            Modified(_playerInventory, item_m);
        }
    }

    private void AddAndRemove(Dictionary<int, ItemInstance> _playerInventory , ItemInstance item)
    {
        //Debug.Log("Add new Object 1 NoFilter" + item.ItemId + " ObjectID " + item.ObjectId);
        if (item.LastChange == (int)InventoryChange.REMOVED)
        {
            if (_playerInventory.ContainsKey(item.ObjectId))
            {
                Debug.Log("Remove object id " + item.ObjectId + " remove item id " + item.ItemId);
                _playerInventory.Remove(item.ObjectId);
                RefreshPosition(_playerInventory);
            }
            
        }
        else if(item.LastChange == (int)InventoryChange.ADDED)
        {
            
            if (!_playerInventory.ContainsKey(item.ObjectId))
            {

                int count = _playerInventory.Count();
                item.SetSlot(count);
                Debug.Log("item add position " + count + " item " + item.Slot + " objectID" + item.ObjectId);
                _playerInventory.Add(item.ObjectId, item);
            }
            
        }
    }

    private void RefreshPosition(Dictionary<int, ItemInstance> _playerInventory)
    {
        int index = 0;
        foreach (var item in _playerInventory)
        {
            item.Value.SetSlot(index++);
        }
    }

    private void Modified(Dictionary<int, ItemInstance> _playerInventory, ItemInstance item)
    {
        Debug.Log("Modified data count " + item.Count + " item obj id " + item.ObjectId + " itemid" + item.ItemId);
        if (!_playerInventory.ContainsKey(item.ObjectId))
        {
            _playerInventory.Add(item.ObjectId, item);
        }
        else
        {
            ItemInstance oldItem = _playerInventory[item.ObjectId];
            item.SetSlot(oldItem.Slot);
            oldItem.Update(item);
        }
    }
    private List<ItemInstance> FilterByRemove(ItemInstance[] items)
    {
        return items.Where(item =>
        item.LastChange == (int)InventoryChange.REMOVED ||
        item.LastChange == (int)InventoryChange.ADDED).ToList();
    }

    private List<ItemInstance> FilterByModified(ItemInstance[] items)
    {
        return items.Where(item =>
        item.LastChange == (int)InventoryChange.MODIFIED).ToList();
    }
    //public ItemInstance GetItemByObjectId(int objectId)
    //{
     //   foreach (ItemInstance item in _playerInventory)
     //   {
          //  if (item.ObjectId == objectId)
           // {
           //     return item;
           // }
       // }

     //   return null;
    //}

    //public ItemInstance GetItemBySlot(int slot)
    //{
        //foreach (ItemInstance item in _playerInventory)
        //{
            //if (item.Slot == slot && !item.Equipped)
            //{
            //    return item;
            //}
        //}

       // return null;
    //}

    public void ChangeItemOrder(int fromSlot, int toSlot)
    {
        //ItemInstance fromItem = GetItemBySlot(fromSlot);
        //ItemInstance toItem = GetItemBySlot(toSlot);

        //if (fromItem == null)
        //{
         //   Debug.LogError($"Can't change item order, item not found at slot {fromSlot}.");
        //    return;
        //}

        //List<InventoryOrder> orders = new List<InventoryOrder>();
        //orders.Add(new InventoryOrder(fromItem.ObjectId, toSlot));

        //if (toItem != null)
        //{
        //    orders.Add(new InventoryOrder(toItem.ObjectId, fromSlot));
        //}

        //InventoryWindow.Instance.SelectSlot(toSlot);
        Debug.Log("Нужно реализовать пакеты для отпарвки на сервер");
        //GameClient.Instance.ClientPacketHandler.UpdateInventoryOrder(orders);
    }

    public void UseItem(int objectId)
    {
        //Cache Name for Message Equip
        //StorageVariable.getInstance().AddS1Items(new VariableItem(GetItemByObjectId(objectId).ItemData.ItemName.Name , objectId));

        //var sendPaket = CreatorPacketsUser.CreateUseItem(objectId, 0);
       // bool enable = GameClient.Instance.IsCryptEnabled();
        //SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
    }

    public void DestroyItem(int objectId, int quantity)
    {

        //Cache Name for Message Equip
       
        if (_playerInventory.ContainsKey(objectId))
        {
            Debug.Log("Destroy item found delete " + objectId);
            ItemInstance item = _playerInventory[objectId];
            StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, objectId));

            //AudioManager.Instance.PlayEquipSound("trash_basket");
            var sendPaket = CreatorPacketsUser.CreateDestroyItem(objectId, quantity);
            bool enable = GameClient.Instance.IsCryptEnabled();
            SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
        }
        else
        {
            Debug.Log("Destroy item not found delete " + objectId);
        }
    }

    public bool IsContaineInventory(int objectId)
    {
        return _playerInventory.ContainsKey(objectId);
    }

}
