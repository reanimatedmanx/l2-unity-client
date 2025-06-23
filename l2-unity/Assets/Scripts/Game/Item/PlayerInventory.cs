using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
    private ChangeInventoryData _changeInventoryData;
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

    public ItemInstance GetItem(int objId)
    {
        if (_playerInventory.ContainsKey(objId))
        {
            return _playerInventory[objId];
        }
        return null;
    }

    public bool IsItemEquipByItemId(int itemId)
    {
        foreach (var item in _playerEquipInventory)
        {
            if(itemId == item.Value.ItemId)
            {
                return true;
            }
            
        }

        return false;
    }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _changeInventoryData = new ChangeInventoryData();
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
        _playerEquipInventory = equipItems;

        List<ItemInstance> items_collect =  _playerInventory.Values.ToList();
        List<ItemInstance> equip_collect = _playerEquipInventory.Values.ToList();

        //if (_playerEquipInventory != null)
       //{
        //    equip_collect = _playerEquipInventory.Values.ToList();
       // }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            InventoryWindow.Instance.SetItemList(items_collect, equip_collect, adenaCount , usedSlot);

            if (openInventory)
            {
                InventoryWindow.Instance.ShowWindow();
            }
        });
    }



    private readonly object _lock = new object();
    public void UpdateInventory(Dictionary<int, ItemInstance> items , Dictionary<int, ItemInstance> equipitems)
    {
        lock (_lock)
        {

            _changeInventoryData.ClearData();

            List<ItemInstance> listEquip = equipitems.Values.ToList();

            var items_f = items.Values.ToArray();
            var _tempForRemoveAndAdd = FilterByRemove(items_f);
            var _tempForModified = FilterByModified(items_f);

            try
            {
                //Debug.Log("start Equip " + _playerEquipInventory.Count + " inventory " + _playerInventory.Count);
                UpdatePlayerInventory(_tempForRemoveAndAdd, _tempForModified , listEquip);
                ModifiedEquip(_playerInventory, _playerEquipInventory, listEquip);
               // Debug.Log("Equip " + equipitems.Count + " inventory " + items.Count);
                //Debug.Log("end Equip " + _playerEquipInventory.Count + " inventory " + _playerInventory.Count);
                int adenaCount = GetAdenaCount(_playerInventory.Values.ToList());
                int useSlot = _playerInventory.Count + _playerEquipInventory.Count;
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    InventoryWindow.Instance.UpdateItemList(_tempForRemoveAndAdd, _tempForModified, listEquip, adenaCount, useSlot, _changeInventoryData);
                    InventoryWindow.Instance.RemoveOldEquipItemOrNoQuipItem(_changeInventoryData);
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
    private void UpdatePlayerInventory(List<ItemInstance> removeAndAdd, List<ItemInstance> modified , List<ItemInstance> listEquip)
    {
        for (int i = 0; i < removeAndAdd.Count; i++)
        {
            ItemInstance item = removeAndAdd[i];
            AddAndRemove(_playerInventory, item);
        }

        for (int i = 0; i < modified.Count; i++)
        {
            ItemInstance item_m = modified[i];
            Modified(_playerInventory, _playerEquipInventory , listEquip , item_m);
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
        int slot = InventoryWindow.Instance.GetEmptySlot();
        item.SetSlot(slot);
        _playerInventory.Add(item.ObjectId, item);

    }

    private void RefreshPosition(Dictionary<int, ItemInstance> playerInventory)
    {
     
    }

    private void Modified(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, List<ItemInstance> listEquip , ItemInstance item)
    {
        //Debug.Log("Modified data count " + item.Count + " item obj id " + item.ObjectId + " itemid" + item.ItemId + " equiped " + item.Equipped);
        UpdateInventory(playerInventory, playerEquipInventory , listEquip , item);
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

    private void UpdateInventory(Dictionary<int, ItemInstance> playerInventory , Dictionary<int, ItemInstance> playerEquipInventory, List<ItemInstance> listEquip ,ItemInstance item)
    {
   
        if (playerEquipInventory.ContainsKey(item.ObjectId) & !item.Equipped)
        {
            //ItemInstance del_item = playerEquipInventory[item.ObjectId];
            ItemInstance del_item_replace = listEquip.FirstOrDefault(itemEquip => itemEquip.EqualsBodyPart(item.BodyPart));
            if (del_item_replace != null)
            {
                GearReplaceSlot(playerEquipInventory, del_item_replace, item);
            }
            else
            {
                ItemInstance del_item = playerEquipInventory[item.ObjectId];
                RemoveGearInventory(playerEquipInventory, del_item);
            }

        }

        //If we move from gear Inventory to items Inventory
        if (!playerInventory.ContainsKey(item.ObjectId))
        {
            if (!item.Equipped)
            {
                
                StorageVariable.getInstance().AddS1Items(new VariableItem(item.Count.ToString(), item.ObjectId));
                StorageVariable.getInstance().AddS2Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
                AddInventory(item); 
            }
        }
        else
        {
            
            ItemInstance oldItem = playerInventory[item.ObjectId];
            int count = item.Count - oldItem.Count;
            item.SetSlot(oldItem.Slot);
            
            StorageVariable.getInstance().AddS1Items(new VariableItem(count.ToString(), item.ObjectId));
            StorageVariable.getInstance().AddS2Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
            oldItem.Update(item);
        }
    }

    private void UpdateInventoryEquip(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance item)
    {
     
        if(playerInventory.ContainsKey(item.ObjectId) & item.Equipped == true)
        {
            ItemInstance del_item = playerInventory[item.ObjectId];
            RemoveInventory(playerInventory, del_item);

        }
        
        if (playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == false)
        {
            ItemInstance del_item = playerEquipInventory[item.ObjectId];
            RemoveGearInventory(playerEquipInventory, del_item);
        }
        else if (!playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == true)
        {
            playerEquipInventory.Add(item.ObjectId, item);
        }
    }





    private void RemoveInventory(Dictionary<int, ItemInstance> playerInventory , ItemInstance del_item)
    {
        
        playerInventory.Remove(del_item.ObjectId);
        RefreshPosition(playerInventory);
        _changeInventoryData.AddRemoveInventory(del_item);
    }

    private void RemoveGearInventory(Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance del_item)
    {
        ///_obsoleteItemsGear.Add(del_item);
        _changeInventoryData.AddRemoveGear(del_item);
       // playerEquipInventory.Remove(del_item.ObjectId);
        if (playerEquipInventory.ContainsKey(del_item.ObjectId)) playerEquipInventory.Remove(del_item.ObjectId);
        //RefreshPosition(playerEquipInventory);
    }

    private void GearReplaceSlot(Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance sourceItem , ItemInstance gearItem)
    {
        if(playerEquipInventory.ContainsKey(gearItem.ObjectId)) playerEquipInventory.Remove(gearItem.ObjectId);
        _changeInventoryData.AddReplaceGear(sourceItem, gearItem);
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
        Debug.Log("����� ����������� ������ ��� �������� �� ������");
        //GameClient.Instance.ClientPacketHandler.UpdateInventoryOrder(orders);
    }

    public bool UseItem(int objectId)
    {
        //Cache Name for Message Equip
 
        if (_playerInventory.ContainsKey(objectId))
        {

            ItemInstance item = _playerInventory[objectId];
            SetVariableInfoS1(item);
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

    private async void SetVariableInfoS1(ItemInstance item)
    {
       // if (item.Category == ItemCategory.Item)
       // {
            StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
            StorageVariable.getInstance().AddS2Items(new VariableItem(item.Count.ToString(), item.ObjectId));
       // }
       // else
       // {
            //StorageVariable.getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
       // }

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
