//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 플레이어가 지닌 여러 아이템들을 아이템의 분류에 맞게 한번에 저장하는 클래스
///// </summary>
//public class Inventory
//{
//    public List<Item> items { get; private set; }

//    public List<Item> weaponItems { get; private set; }
//    public List<Item> usableItems { get; private set; }
//    public List<Item> otherItems { get; private set; }

//    public Inventory()
//    {
//        items = new List<Item>();
//        weaponItems = new List<Item>();
//        usableItems = new List<Item>();
//        otherItems = new List<Item>();
//    }

//    /// <summary>
//    /// 인벤토리에 아이템을 추가합니다.
//    /// </summary>
//    /// <param name="item"> 추가할 아이템 </param>
//    public void AddItem(Item item)
//    {
//        items.Add(item);
//        if (item.itemInfo.category == ItemInfo.ItemCategory.Weapon)
//        {
//            weaponItems.Add(item);
//        }
//        else if (item.itemInfo.category == ItemInfo.ItemCategory.Usable)
//        {
//            usableItems.Add(item);
//        }
//        else if (item.itemInfo.category == ItemInfo.ItemCategory.Other)
//        {
//            otherItems.Add(item);
//        }
//        else
//        {
//            Debug.Log("알 수 없는 카테고리의 아이템입니다.");
//        }
//    }

//    /// <summary>
//    /// 인벤토리에서 아이템을 삭제합니다.
//    /// </summary>
//    /// <param name="index"> 삭제할 아이템 </param>
//    public void DeleteItem(Item item) 
//    {
//        DeleteItemAtEachCategory(items, item.itemInfo.index);
//        if (item.itemInfo.category == ItemInfo.ItemCategory.Weapon)
//        {
//            Debug.Log("w");
//            DeleteItemAtEachCategory(weaponItems, item.itemInfo.index);
//        }
//        else if (item.itemInfo.category == ItemInfo.ItemCategory.Usable)
//        {
//            Debug.Log("u");
//            DeleteItemAtEachCategory(usableItems, item.itemInfo.index);
//        }
//        else if (item.itemInfo.category == ItemInfo.ItemCategory.Other)
//        {
//            Debug.Log("o");
//            DeleteItemAtEachCategory(otherItems, item.itemInfo.index);
//        }
//        else
//        {
//            Debug.Log("알 수 없는 카테고리의 아이템입니다.");
//        }
//    }

//    /// <summary>
//    /// 하나의 아이템 분류에서 아이템을 삭제합니다.
//    /// </summary>
//    /// <param name="items"> 삭제할 아이템의 분류 </param>
//    /// <param name="index"> 삭제할 아이템의 고유번호 </param>
//    private void DeleteItemAtEachCategory(List<Item> items, int index)
//    {
//        foreach (Item item in items)
//        {
//            if (item.itemInfo.index == index)
//            {
//                ItemInfo.ItemCategory itemCategory = item.itemInfo.category;
//                items.Remove(item);
//                return;
//            }
//        }
//        Debug.Log("리스트에 존재하지 않는 아이템을 삭제 시도했습니다.");
//    }
//}

///// <summary>
///// 게임에서 사용되는 아이템과 아이템의 획득, 이동 등의 기능을 관리하는 클래스
///// </summary>
//public class ItemManager : Generic.Singleton<ItemManager>
//{
//    private List<ItemInfo> _itemInformations;
//    private Inventory _inventory;
//    public int money { get; private set; }

//    public GameObject inst;

//    private new void Awake()
//    {
//        base.Awake();

//        InitItemInfo();
//        _inventory = new Inventory();
//        money = 10;
//    }
//    private void InitItemInfo()
//    {
//        List<List<string>> _itemTable = FileRead.Read("ItemTable");
//        if (_itemTable == null)
//        {
//            Debug.Log("item table을 읽어오지 못했습니다.");
//            return;
//        }

//        _itemInformations = new List<ItemInfo>();
//        for (int i = 0; i < _itemTable.Count; i++)
//        {
//            ItemInfo _itemInfo = new ItemInfo(_itemTable[i]);
//            _itemInformations.Add(_itemInfo);
//        }
//    }

//    /// <summary>
//    /// 입력된 고유번호가 아이템 테이블에서 읽어온 아이템 중에 존재하는지 확인합니다.
//    /// </summary>
//    /// <param name="index"></param>
//    /// <returns>
//    /// 존재한다면 해당 아이템의 ItemInfo를 반환합니다.
//    /// 존재하지 않는다면 null을 반환합니다.
//    /// </returns>
//    private ItemInfo FindItemInfoByIndex(int index)
//    {
//        for (int i = 0; i < _itemInformations.Count; i++)
//        {
//            if (index == _itemInformations[i].index)
//            {
//                return _itemInformations[i];
//            }
//        }

//        Debug.Log("해당 인덱스의 아이템을 찾지 못했습니다.");
//        return null;
//    }

//    /// <summary>
//    /// 플레이어의 인벤토리에 아이템을 추가합니다.
//    /// </summary>
//    /// <param name="index"> 추가할 아이템의 고유번호 </param>
//    public void AddItem(int index) 
//    {
//        ItemInfo fountItemInfo = FindItemInfoByIndex(index);
//        if (fountItemInfo == null)
//        {
//            Debug.Log("아이템 획득에 실패했습니다.");
//            return;
//        }

//        Item item = new Item(fountItemInfo);
//        _inventory.AddItem(item);


//    }

//    /// <summary>
//    /// 플레이어의 인벤토리에서 아이템을 삭제합니다.
//    /// </summary>
//    /// <param name="index"> 삭제할 아이템의 고유번호 </param>
//    public void DeleteItem(int index)
//    {
//        ItemInfo fountItemInfo = FindItemInfoByIndex(index);
//        if (fountItemInfo == null)
//        {
//            Debug.Log("아이템 제거에 실패했습니다. item index: " + index);
//            return;
//        }

//        Item item = new Item(fountItemInfo);
//        _inventory.DeleteItem(item);
//    }

//    /// <summary>
//    /// 플레이어의 인벤토리에서 아이템을 찾아서 반환합니다.
//    /// </summary>
//    /// <param name="index"> 찾고자 하는 아이템의 고유번호 </param>
//    /// <returns>
//    /// 아이템이 인벤토리에 존재할 경우 해당 아이템들의 배열을 반환합니다.
//    /// 존재하지 않을 경우 null을 반환합니다.
//    /// </returns>
//    public List<Item> GetItem(int index)
//    {
//        List<Item> findItems = new List<Item>();
//        for (int i = 0; i < _inventory.items.Count; i++)
//        {
//            if (_inventory.items[i].itemInfo.index == index)
//            {
//                findItems.Add(_inventory.items[i]);
//            }
//        }

//        if (findItems.Count != 0)
//        {
//            return findItems;
//        }
//        Debug.Log("해당 인덱스의 아이템을 찾지 못했습니다. 인덱스: " + index);
//        return null;
//    }

//    /// <summary>
//    /// 플레이어의 인벤토리를 반환합니다.
//    /// </summary>
//    /// <returns>
//    /// 플레이어의 인벤토리가 존재할 경우 인벤토리를 반환합니다.
//    /// 존재하지 않을 경우 null을 반환합니다. - 오류 발생
//    /// </returns>
//    public Inventory GetInventory()
//    {
//        //Debug.Log("inventory test");
//        //Debug.Log("전체 아이템 개수:" + _inventory.items.Count);
//        //foreach (Item item in _inventory.items)
//        //{
//        //    Debug.Log(item.itemInfo.index);
//        //}
//        //Debug.Log("무기 아이템 개수:" + _inventory.weaponItems.Count);
//        //foreach (Item item in _inventory.weaponItems)
//        //{
//        //    Debug.Log(item.itemInfo.index);
//        //}
//        //Debug.Log("소비 아이템 개수:" + _inventory.usableItems.Count);
//        //foreach (Item item in _inventory.usableItems)
//        //{
//        //    Debug.Log(item.itemInfo.index);
//        //}
//        //Debug.Log("기타 아이템 개수:" + _inventory.otherItems.Count);
//        //foreach (Item item in _inventory.otherItems)
//        //{
//        //    Debug.Log(item.itemInfo.index);
//        //}

//        if (_inventory == null) 
//        {
//            Debug.LogError("인벤토리가 존재하지 않습니다.");
//            return null;
//        }
//        return _inventory;
//    }

//    /// <summary>
//    /// 해당 고유번호의 아이템 정보를 반환합니다.
//    /// </summary>
//    /// <param name="index"> 찾고자 하는 아이템 정보의 고유번호 </param>
//    /// <returns>
//    /// ItemTable에서 읽어온 아이템 정보에 해당 고유번호의 아이템 정보가 존재할 경우 아이템 정보를 반환합니다.
//    /// 존재하지 않을 경우 null을 반환합니다. - 오류 발생
//    /// </returns>
//    public ItemInfo GetItemInfo(int index) 
//    {
//        foreach (ItemInfo info in _itemInformations) 
//        {
//            if (info.index == index) 
//            {
//                return info;
//            }
//        }
//        Debug.LogError("해당 인덱스의 아이템 정보를 찾을 수 없습니다. index: " + index);
//        return null;
//    }

//    /// <summary>
//    /// 플레이어의 아이템을 사용합니다. - 가구현
//    /// 사용하고자 하는 아이템의 분류에 맞는 함수가 호출됩니다.
//    /// </summary>
//    /// <param name="index"> 사용하고자 하는 아이템 고유번호 </param>
//    public void UseItem(int index)
//    {
//        List<Item> item = GetItem(index);
//        if (item == null) 
//        {
//            Debug.LogError("사용하고자 하는 아이템이 플레이어에게 존재하지 않습니다.");
//            return;
//        }

//        if (item[0].itemInfo.category == ItemInfo.ItemCategory.Weapon) 
//        {
//            EquipWeaponItem(item[0]);
//        }
//        else if (item[0].itemInfo.category == ItemInfo.ItemCategory.Usable)
//        {
//            UseUsableItem(item[0]);
//        }
//        else
//        {
//        }
//    }
//    private void EquipWeaponItem(Item item) 
//    {
//        //Item currentWeapon = GetCurrnetWeapon();
//        //EquipWeapon(item);
//        //AddItem(currentWeapon);
//    }
//    private void UseUsableItem(Item item)
//    {
//    }

//    /// <summary>
//    /// 아이템을 판매합니다. 플레이어의 인벤토리에서 해당 아이템 1개를 삭제하고 판매한 아이템의 가치만큼 머니가 증가합니다.
//    /// </summary>
//    /// <param name="index"> 판매하고자 하는 아이템의 고유번호 </param>
//    public void SellItem(int index)
//    {
//        GetInventory();
//        List<Item> item = GetItem(index);
//        money += item[0].itemInfo.price;
//        DeleteItem(index);
//    }

//    /// <summary>
//    /// 아이템을 삭제합니다. 플레이어의 아이템에서 해당 아이템 1개가 삭제됩니다.
//    /// </summary>
//    /// <param name="index"> 삭제하고자 하는 아이템의 고유번호 </param>
//    public void DiscardItem(int index)
//    {
//        DeleteItem(index);
//    }
//}
