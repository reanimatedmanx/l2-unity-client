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

    //private List<ItemInstance> _tempForRemoveAndAdd;
    //private List<ItemInstance> _tempForModified;


    public static PlayerInventory _instance;
    public static PlayerInventory Instance { get { return _instance; } }


    public List<ItemInstance> FilterInventory(List<ItemCategory> filteredCategories)
    {
        if(_playerInventory != null)
        {
                var filteredItemsInventory = _playerInventory
               .Where(kvp => filteredCategories.Contains(kvp.Value.Category))
               .Where(kvp => !kvp.Value.Equipped)
               .Select(kvp => kvp.Value)
               .ToList();

            return filteredItemsInventory;
        }

        return new List<ItemInstance>();
    }
    public Dictionary<int, ItemInstance>  GetPlayerInventory()
    {

        return _playerInventory;
    }

    public List<ItemInstance> GetPlayerInvetoryToList()
    {
        return _playerInventory.Values.ToList();
    }
    public List<ItemInstance> GetPlayerEquipToList()
    {
        return _playerEquipInventory.Values.ToList();
    }
    public Dictionary<int, ItemInstance> GetPlayerEquipInventory()
    {
        return _playerEquipInventory;
    }

    public ItemInstance GetItemEquip(int objId)
    {
        if (_playerEquipInventory.ContainsKey(objId))
        {
            return _playerEquipInventory[objId];
        }
        return null;
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
        //_tempForRemoveAndAdd = new List<ItemInstance>();
        //_tempForModified = new List<ItemInstance>();
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

    public void SetInventory(Dictionary<int, ItemInstance> items, Dictionary<int, ItemInstance> equipItems , bool openInventory , int adenaCount , int usedSlot)
    {
        _playerInventory = items;
        List<ItemInstance> items_collect =  _playerInventory.Values.ToList();
        List<ItemInstance> equip_collect = null;
        if (equipItems != null)
        {
            equip_collect = equipItems.Values.ToList();
            _playerEquipInventory = equipItems;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            InventoryWindow.Instance.SetItemList(items_collect, equip_collect, adenaCount , usedSlot);

            if (openInventory)
            {
                InventoryWindow.Instance.ShowWindow();
            }
        });
    }

    //We remove items from the dictionary before we remove them in the Tab Inventory itself, so we put them in this temporary collection.
    //It will only store items that are removed as a result of moving from the inventory to the equip inventory and back.
    private List<ItemInstance> _obsoleteItemsInventory = new List<ItemInstance>();
    private List<ItemInstance> _obsoleteItemsGear = new List<ItemInstance>();

    private readonly object _lock = new object();
    public void UpdateInventory(Dictionary<int, ItemInstance> items , Dictionary<int, ItemInstance> equipitems)
    {
        lock (_lock)
        {
            _obsoleteItemsInventory.Clear();
            _obsoleteItemsGear.Clear();

            List<ItemInstance> listEquip = equipitems.Values.ToList();

            var items_f = items.Values.ToArray();
            var _tempForRemoveAndAdd = FilterByRemove(items_f);
            var _tempForModified = FilterByModified(items_f);

            try
            {

                UpdatePlayerInventory(_tempForRemoveAndAdd, _tempForModified);
                ModifiedEquip(_playerInventory, _playerEquipInventory, listEquip);
                int adenaCount = GetAdenaCount(_playerInventory.Values.ToList());

                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    InventoryWindow.Instance.UpdateItemList(_tempForRemoveAndAdd, _tempForModified, listEquip, adenaCount, _playerInventory.Count + _playerEquipInventory.Count);
                    InventoryWindow.Instance.RemoveOldEquipItemOrNoQuipItem(_obsoleteItemsInventory , _obsoleteItemsGear);
                });
            

            }
            catch (Exception ex)
            {
                Debug.LogError("PlayerInventory Errors: " + ex.StackTrace);
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
            Modified(_playerInventory, _playerEquipInventory , item_m);
        }
    }

 

    private void AddAndRemove(Dictionary<int, ItemInstance> _playerInventory , ItemInstance item)
    {
        //Debug.Log("Add new Object 1 NoFilter" + item.ItemId + " ObjectID " + item.ObjectId);
        if (item.LastChange == (int)InventoryChange.REMOVED)
        {
            if (_playerInventory.ContainsKey(item.ObjectId))
            {
                //Debug.Log("Remove object id " + item.ObjectId + " remove item id " + item.ItemId);
                _playerInventory.Remove(item.ObjectId);
                RefreshPosition(_playerInventory);
            }
            
        }
        else if(item.LastChange == (int)InventoryChange.ADDED)
        {
            
            if (!_playerInventory.ContainsKey(item.ObjectId))
            {
                AddInventory(item);
            }
            
        }
    }

    private void AddInventory(ItemInstance item)
    {
        //int count = _playerInventory.Count();
        int slot = InventoryWindow.Instance.GetEmptySlot();
        item.SetSlot(slot);
        //Debug.Log("Item add position " + count + " item " + item.Slot + " objectID" + item.ObjectId);
        //Debug.Log("RemoveInventory Add ITEM POSITION " + item.Slot + " objectID" + item.ObjectId);
        
        _playerInventory.Add(item.ObjectId, item);

    }

    private void RefreshPosition(Dictionary<int, ItemInstance> playerInventory)
    {
        int index = 0;
        foreach (var item in playerInventory)
        {
            
            //item.Value.SetSlot(index++);
            //Debug.Log("RemoveInventory Add Remove " + item.Value.ItemId + " position " + index);
        }
    }

    private void Modified(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance item)
    {
        //Debug.Log("Modified data count " + item.Count + " item obj id " + item.ObjectId + " itemid" + item.ItemId + " equiped " + item.Equipped);
        UpdateInventory(playerInventory, playerEquipInventory , item);
        UpdateEquip(playerInventory , playerEquipInventory, item);
    }

    private void ModifiedEquip(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory , List<ItemInstance> modified)
    {
        for(int i =0; i < modified.Count; i++)
        {
            ItemInstance item = modified[i];
            UpdateInventoryEquip(playerInventory, playerEquipInventory, item);
        }
        
    }

    private void UpdateInventory(Dictionary<int, ItemInstance> playerInventory , Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance item)
    {

        if (playerEquipInventory.ContainsKey(item.ObjectId) & !item.Equipped)
        {
            //StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
            ItemInstance del_item = playerEquipInventory[item.ObjectId];
            RemoveGearInventory(playerEquipInventory, del_item);
        }

        //If we move from gear Inventory to items Inventory
        if (!playerInventory.ContainsKey(item.ObjectId))
        {
            if (!item.Equipped)
            {
                //StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
                AddInventory(item);
            }
        }
        else
        {
            
            ItemInstance oldItem = playerInventory[item.ObjectId];
            //Debug.Log("item add position update position " + oldItem.Slot);
            item.SetSlot(oldItem.Slot);
            oldItem.Update(item);
        }
    }

    private void UpdateInventoryEquip(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance item)
    {
     
        if(playerInventory.ContainsKey(item.ObjectId) & item.Equipped == true)
        {
            ItemInstance del_item = playerInventory[item.ObjectId];
            RemoveInventory(playerInventory, del_item);
            //StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
        }
        
        if (playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == false)
        {
            ItemInstance del_item = playerEquipInventory[item.ObjectId];
            RemoveGearInventory(playerEquipInventory, del_item);
            //StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
        }
        else if (!playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == true)
        {
            playerEquipInventory.Add(item.ObjectId, item);
            //StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
        }
    }





    private void RemoveInventory(Dictionary<int, ItemInstance> playerInventory , ItemInstance del_item)
    {
        
        playerInventory.Remove(del_item.ObjectId);
        RefreshPosition(playerInventory);
        //Debug.Log("RemoveInventory Add Remove old itemId " + del_item.ItemId  + " Slot " + del_item.Slot);
        _obsoleteItemsInventory.Add(del_item);
    }

    private void RemoveGearInventory(Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance del_item)
    {
        _obsoleteItemsGear.Add(del_item);
        playerEquipInventory.Remove(del_item.ObjectId);
        //RefreshPosition(playerEquipInventory);
    }


    private void UpdateEquip(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance item)
    {
        ItemsDetectedToGear(playerInventory, item);
        GearDetectedToItems(playerEquipInventory, item);
    }

    private void ItemsDetectedToGear(Dictionary<int, ItemInstance> playerInventory, ItemInstance item)
    {
        if (playerInventory.ContainsKey(item.ObjectId) & item.Equipped)
        {
            RemoveInventory(playerInventory, item);
        }

        if (!playerInventory.ContainsKey(item.ObjectId) & !item.Equipped)
        {
            playerInventory.Add(item.ObjectId, item);
        }

    }
    private void GearDetectedToItems(Dictionary<int, ItemInstance> playerEquipInventory , ItemInstance item)
    {
        if (!playerEquipInventory.ContainsKey(item.ObjectId))
        {
            if (item.Equipped)
            {
                playerEquipInventory.Add(item.ObjectId, item);
            }
        }

        //if we move from gear Invetory to items Inventory we need to delete from Gear so that there is no duplication
        else if (playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == false)
        {
            RemoveGearInventory( playerEquipInventory, item);
        }
        else
        {
            ItemInstance oldItem = playerEquipInventory[item.ObjectId];
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

    public bool UseItem(int objectId)
    {
        //Cache Name for Message Equip
 
        if (_playerInventory.ContainsKey(objectId))
        {

            ItemInstance item = _playerInventory[objectId];
            StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, objectId));
            var sendPaket = CreatorPacketsUser.CreateUseItem(objectId, 0);
            bool enable = GameClient.Instance.IsCryptEnabled();
            SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
            return true;
        }
        else
        {
            Debug.Log("Not found use items");
            return false;
        }

    }

    public void DestroyItem(int objectId, int quantity)
    {

        //Cache Name for Message Equip
       
        if (_playerInventory.ContainsKey(objectId))
        {

            ItemInstance item = _playerInventory[objectId];
            getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, objectId));
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
