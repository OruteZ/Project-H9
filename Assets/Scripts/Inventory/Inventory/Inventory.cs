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

    public Inventory() 
    {
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

    public void AddItem(IItem additem)
    {
        List<IItem> itemList = GetCorrectTypeItemList(additem.GetData().itemType);
        if (additem.GetData().itemMaxStorage > 1)
        {
            //stack operation?
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] is null)
            {
                itemList[i] = additem;
                IInventory.OnInventoryChanged?.Invoke();
                return;
            }
        }
        Debug.LogError("Inventory is full");
    }
    public void DeleteItem(IItem deleteItem)
    {
        List<IItem> itemList = GetCorrectTypeItemList(deleteItem.GetData().itemType);
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].GetData().id == deleteItem.GetData().id)
            {
                itemList[i] = null;
                IInventory.OnInventoryChanged?.Invoke();
                return;
            }
        }
        Debug.LogError("Can't find item id: " + deleteItem.GetData().id);
    }
    public void SwapItem(ItemType type, int start, int end)
    {
        Debug.Log(start + " <-> " + end);
        List<IItem> itemList = GetCorrectTypeItemList(type);
        IItem tmpItem = itemList[end];
        itemList[end] = itemList[start];
        itemList[start] = tmpItem;
    }
    public void EqipItem(ItemType type, int index)
    {
        if (type is not ItemType.Revolver && type is not ItemType.Repeater && type is not ItemType.Shotgun) return;

        Debug.Log(index + " postion eqipped, " + _equippedItem.GetData().id +" unequipped");
        List<IItem> itemList = GetCorrectTypeItemList(type);
        IItem tmpItem = _equippedItem;
        _equippedItem = itemList[index];
        itemList[index] = tmpItem;
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
    
    public void PrintItems()
    {
        List<IItem> items = GetAllItemList();
        foreach (var item in items)
        {
            Debug.Log(item);
            Debug.Log(item.GetData().id);
        }
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

    public int GetMoney() 
    {
        return _gold;
    }
}