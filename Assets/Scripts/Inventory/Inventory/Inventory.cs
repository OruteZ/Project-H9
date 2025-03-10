using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

[Serializable]
public class Inventory : IInventory
{
    private int _gold;

    private const int INVENTORY_MAX_SIZE = 42;

    // [SerializeField]
    private List<IItem> _weaponItems = new();
    private List<IItem> _consumableItems = new();
    private List<IItem> _elseItems = new();
    private IItem _equippedItem = null;

    public const int WEAPON_COST = 4;
    public const int ITEM_COST = 2;

    public Inventory()
    {
        _weaponItems.Clear();
        _consumableItems.Clear();
        _elseItems.Clear();
        for (int i = 0; i < INVENTORY_MAX_SIZE; i++)
        {
            _weaponItems.Add(null);
            _consumableItems.Add(null);
            _elseItems.Add(null);
        }
    }
    public void InitEquippedItem(Item item) 
    {
        if (_equippedItem is null) 
        {
            _equippedItem = item;
        }
    }
    public IItem GetEquippedItem() 
    {
        return _equippedItem;
    }

    public bool TryAddItem(IItem item)
    {
        if (item is null)
        {
            Debug.LogError("try add null item");
            return false;
        }
        
        List<IItem> itemList = GetCorrectTypeItemList(item.GetData().itemType);
        if (item.GetData().itemMaxStorage > 1)
        {
            for (int i = 0; i < itemList.Count; i++) if(itemList[i] is not null)
            {
                bool isStackFull = (itemList[i].GetData().itemMaxStorage <= itemList[i].GetStackCount());
                if (isStackFull) continue;
                
                bool isSameItem = (itemList[i].GetData().id == item.GetData().id);
                if (isSameItem)
                {
                    itemList[i] = (Item)itemList[i] + (Item)item;
                    IInventory.OnInventoryChanged?.Invoke();
                    IInventory.OnGetItem?.Invoke(item.GetData());
                    return true;
                }
            }
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] is null)
            {
                itemList[i] = item;
                IInventory.OnInventoryChanged?.Invoke();
                IInventory.OnGetItem?.Invoke(item.GetData());

                if (itemList == _weaponItems) 
                {
                    UIManager.instance.gameSystemUI.alarmUI.AddAlarmUI((WeaponItem)item);
                }
                return true;
            }
        }
        Debug.LogError("Inventory is full");
        return false;
    }

    public void DeleteItem(IItem deleteItem, int index, int cnt = 1)
    {
        List<IItem> itemList = GetCorrectTypeItemList(deleteItem.GetData().itemType);
        if (itemList[index].GetData().id != deleteItem.GetData().id) 
        {
            Debug.LogError("Inventory delete error");
            return;
        }
        itemList[index].SetStackCount(itemList[index].GetStackCount() - cnt);
        CollectZeroItem();
        IInventory.OnInventoryChanged?.Invoke();
        return;
    }
    
    public void CollectZeroItem()
    {
        for(int i = 0; i < _weaponItems.Count; i++)
        {
            if (_weaponItems[i] == null) continue;
            if (_weaponItems[i].GetStackCount() <= 0)
            {
                _weaponItems[i] = null;
            }
        }
        
        for(int i = 0; i < _consumableItems.Count; i++)
        {
            if (_consumableItems[i] == null) continue;   
            if (_consumableItems[i].GetStackCount() <= 0)
            {
                _consumableItems[i] = null;
            }
        }
        
        for(int i = 0; i < _elseItems.Count; i++)
        {
            if (_elseItems[i] == null) continue;
            if (_elseItems[i].GetStackCount() <= 0)
            {
                _elseItems[i] = null;
            }
        }
    }

    public void SwapItem(ItemType type, int start, int end)
    {
        List<IItem> itemList = GetCorrectTypeItemList(type);
        (itemList[end], itemList[start]) = (itemList[start], itemList[end]);
    }
    public void EquipItem(ItemType type, int index)
    {
        List<IItem> itemList = GetCorrectTypeItemList(type);
        if (itemList != _weaponItems) return;

        if (((WeaponItem)itemList[index]).TryEquip())
        {
            (_equippedItem, itemList[index]) = (itemList[index], _equippedItem);
        }
    }
    public void UseItem(ItemType type, int index)
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player is null) return;

        List<IItem> itemList = GetCorrectTypeItemList(type);
        if (itemList != _consumableItems) return;

        if (itemList[index].IsUsable())
        {
            var data = itemList[index].GetData();
            
            player.SelectItem(itemList[index]);
            
            IInventory.OnUseItem.Invoke(data);
        }
    }
    public void SellItem(ItemType type, int index)
    {
        List<IItem> itemList = GetCorrectTypeItemList(type);
        if (itemList == _elseItems) return;

        AddGold(itemList[index].GetData().itemPrice);
        DeleteItem(itemList[index], index);
    }

    public IEnumerable<IItem> GetItems()
    {
        return GetAllItemList();
    }
    public IEnumerable<IItem> GetItems(ItemType type)
    {
        return GetCorrectTypeItemList(type);
    }
    public int GetItemCount(IItem item)
    {
        return GetCorrectTypeItemList(item.GetData().itemType).Count(i => i.Equals(item));
    }
    public int GetItemCount(int itemIndex)
    {
        List<IItem> items = GetAllItemList();
        if (itemIndex < 0 || itemIndex >= items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(itemIndex));
        }
        return items.Count(i => i.Equals(items[itemIndex]));
    }
    public bool IsItemExist(ItemType type) 
    {
        var list = GetCorrectTypeItemList(type);
        foreach (var element in list) 
        {
            if (element != null) return true;
        }
        return false;
    }

    public void PrintItems()
    {
        List<IItem> items = GetAllItemList();
        foreach (var item in items)
        {
            Debug.Log(item);
            Debug.Log(item.GetData().id);
        }
    }

    public List<IItem> GetInventory()
    {
        List<IItem> itemList = _weaponItems.ToList<IItem>();
        itemList.AddRange(_consumableItems);
        itemList.AddRange(_elseItems);
        return itemList;
    }
    private List<IItem> GetAllItemList()
    {
        List<IItem> itemList = _weaponItems.ToList<IItem>();
        itemList.Add(_equippedItem);
        itemList.AddRange(_consumableItems);
        itemList.AddRange(_elseItems);
        
        itemList.RemoveAll(i => i == null);
        return itemList;
    }
    private List<IItem> GetCorrectTypeItemList(ItemType type)
    {
        switch (type)
        {
            case ItemType.Revolver:
            case ItemType.Repeater:
            case ItemType.Shotgun:
                {
                    return _weaponItems;
                }
            case ItemType.Heal:
            case ItemType.Damage:
            case ItemType.Cleanse:
            case ItemType.Buff:
            case ItemType.Debuff:
                {
                    return _consumableItems;
                }
        }
        return _elseItems;
    }

    public void AddGold(int reward) 
    {
        _gold += reward;
        PlayerEvents.OnGetMoney?.Invoke(reward);
    }
    public int GetGold() 
    {
        return _gold;
    }
}