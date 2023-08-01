using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemListUI : UISystem
{
    private ItemManager _itemManager;
    private ItemInfo.ItemCategory _currentItemUIStatus = ItemInfo.ItemCategory.Weapon;
    private int _currentItemIndex;

    private List<GameObject> _itemLists = new List<GameObject>();
    private const int ITEM_LIST_INIT_COUNT = 20;
    private readonly Vector3 ITEM_LIST_INIT_POSITION = new Vector3(364, -60, 0);
    private const float ITEM_LIST_INTERVAL = 100;

    public GameObject ItemListPrefab;
    [SerializeField] private GameObject _ItemListPrefabs;
    [SerializeField] private GameObject _weaponItemPanel;
    [SerializeField] private GameObject _usableItemPanel;
    [SerializeField] private GameObject _otherItemPanel;
    [SerializeField] private GameObject _weaponItemListScrollContents;
    [SerializeField] private GameObject _usableItemListScrollContents;
    [SerializeField] private GameObject _otherItemListScrollContents;
    [SerializeField] private GameObject _itemTooltipWindow;

    // Start is called before the first frame update
    void Start()
    {
        _itemManager = ItemManager.instance;
        _currentItemIndex = -1;

        ShowWeaponItems();
        ExpandItemLists();
    }
    public override void OpenUI()
    {
        base.OpenUI();
        SetItemLists();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }

    public void ChangeItemUIStatus(ItemInfo.ItemCategory status)
    {
        if (_currentItemUIStatus != status)
        {
            if (status == ItemInfo.ItemCategory.Weapon)
            {
                ShowWeaponItems();
            }
            else if (status == ItemInfo.ItemCategory.Usable)
            {
                ShowUsableItems();
            }
            else if (status == ItemInfo.ItemCategory.Other)
            {
                ShowOtherItems();
            }
            _currentItemUIStatus = status;
        }
    }
    private void ShowWeaponItems()
    {
        _weaponItemPanel.SetActive(true);
        _usableItemPanel.SetActive(false);
        _otherItemPanel.SetActive(false);
    }
    private void ShowUsableItems()
    {
        _weaponItemPanel.SetActive(false);
        _usableItemPanel.SetActive(true);
        _otherItemPanel.SetActive(false);
    }
    private void ShowOtherItems()
    {
        _weaponItemPanel.SetActive(false);
        _usableItemPanel.SetActive(false);
        _otherItemPanel.SetActive(true);
    }

    private void SetItemLists()
    {
        for (int i = 0; i < _itemLists.Count; i++)
        {
            _itemLists[i].SetActive(false);
            _itemLists[i].transform.SetParent(_ItemListPrefabs.transform);
        }
        int itemListCount = 0;  //static?
        Inventory inventory = _itemManager.GetInventory();

        SetEachItemList(itemListCount, inventory.weaponItems, _weaponItemListScrollContents);
        itemListCount += inventory.weaponItems.Count;
        SetEachItemList(itemListCount, inventory.usableItems, _usableItemListScrollContents);
        itemListCount += inventory.usableItems.Count;
        SetEachItemList(itemListCount, inventory.otherItems, _otherItemListScrollContents);


    }
    private void SetEachItemList(int cnt, List<Item> items, GameObject scrollContents)
    {
        int itemListCount = cnt;
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 pos = ITEM_LIST_INIT_POSITION;
            pos.y -= i * ITEM_LIST_INTERVAL;
            SetItemList(itemListCount, pos, scrollContents, items[i]);
            itemListCount++;
        }
        if (scrollContents == null) return;
        scrollContents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, items.Count * ITEM_LIST_INTERVAL);
    }

    private void SetItemList(int index, Vector3 pos, GameObject parents, Item item)
    {
        if (index >= _itemLists.Count)
        {
            ExpandItemLists();
        }
        _itemLists[index].SetActive(true);
        _itemLists[index].transform.SetParent(parents.transform);
        _itemLists[index].transform.localPosition = pos;
        _itemLists[index].GetComponent<ItemListElement>().SetItemListElement(item, this);

    }
    private void ExpandItemLists()
    {
        for (int i = 0; i < ITEM_LIST_INIT_COUNT; i++)
        {
            GameObject itemList = Instantiate(ItemListPrefab, ITEM_LIST_INIT_POSITION, Quaternion.identity, _ItemListPrefabs.transform);

            itemList.SetActive(false);
            _itemLists.Add(itemList);
        }
    }

    public void ClickCharacterUIButton(int index)
    {
        //Debug.Log("클릭한 아이템 인덱스: " + index);
        if (!_itemTooltipWindow.activeSelf)
        {
            SetItemUseWindow(index);
        }
        else
        {
            CloseItemUseWindow();
        }
    }
    private void SetItemUseWindow(int index)
    {
        _itemTooltipWindow.transform.position = Input.mousePosition;
        ItemInfo itemInfo = _itemManager.GetItemInfo(index);
        if (itemInfo.category == ItemInfo.ItemCategory.Weapon)
        {
        }
        else if (itemInfo.category == ItemInfo.ItemCategory.Usable)
        {
        }
        else
        {
        }

        _currentItemIndex = index;

        OpenPopupWindow();
    }
    public override void OpenPopupWindow()
    {
        UIManager.instance.previousLayer = 3;
        _itemTooltipWindow.SetActive(true);
    }
    public override void ClosePopupWindow()
    {
        UIManager.instance.previousLayer = 2;
        CloseItemUseWindow();
    }

    public void CloseItemUseWindow()
    {
        _currentItemIndex = -1;
        _itemTooltipWindow.SetActive(false);
    }

    public void OnClickTestBtn(int i)
    {
        //for test
        _itemManager.AddItem(i);
        SetItemLists();
    }
    public void ClickUseItem()
    {
        _itemManager.UseItem(_currentItemIndex);
        SetItemLists();
        CloseItemUseWindow();
    }
    public void ClickSellItem()
    {
        _itemManager.SellItem(_currentItemIndex);
        SetItemLists();
        CloseItemUseWindow();

        GetComponent<CharacterUI>().SetMoneyText();
    }
    public void ClickDiscardItem()
    {
        _itemManager.DiscardItem(_currentItemIndex);
        SetItemLists();
        CloseItemUseWindow();
    }
}
