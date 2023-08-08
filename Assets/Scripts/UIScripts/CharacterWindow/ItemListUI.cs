using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private static int _itemListCount = 0;

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
        Inventory inventory = _itemManager.GetInventory();

        SetEachItemList(inventory.weaponItems, _weaponItemListScrollContents);
        SetEachItemList(inventory.usableItems, _usableItemListScrollContents);
        SetEachItemList(inventory.otherItems, _otherItemListScrollContents);


    }
    private void SetEachItemList(List<Item> items, GameObject scrollContents)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 pos = ITEM_LIST_INIT_POSITION;
            pos.y -= i * ITEM_LIST_INTERVAL;
            SetItemList(_itemListCount, pos, scrollContents, items[i]);
            _itemListCount++;
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
        SetItemPopupWindow(_itemManager.GetItemInfo(index));

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

    private void SetItemPopupWindow(ItemInfo itemInfo)
    {
        int cnt = 0;
        if (itemInfo.category == ItemInfo.ItemCategory.Weapon)
        {
            SetItemPopupWindowButton(cnt++, "Eqipment");
        }
        else if (itemInfo.category == ItemInfo.ItemCategory.Usable)
        {
            SetItemPopupWindowButton(cnt++, "Use");
        }
        else
        {
        }
        SetItemPopupWindowButton(cnt++, "Sell");
        SetItemPopupWindowButton(cnt++, "Discard");
    }
    private void SetItemPopupWindowButton(int order, string type) 
    {
        Button btn = _itemTooltipWindow.transform.GetChild(order).GetComponent<Button>();
        TextMeshProUGUI text = btn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        switch (type) 
        {
            case "Eqipment": 
                {
                    btn.onClick.AddListener(() => ClickUseItem());
                    break;
                }
            case "Use":
                {
                    btn.onClick.AddListener(() => ClickUseItem());
                    break;
                }
            case "Sell":
                {
                    btn.onClick.AddListener(() => ClickSellItem());
                    break;
                }
            case "Discard":
                {
                    btn.onClick.AddListener(() => ClickDiscardItem());
                    break;
                }
        }
        text.text = type;
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
