using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterUI : Generic.Singleton<CharacterUI>
{
    //Character Stat
    [Header("Character Stat UI")]
    public GameManager gameManager; //unit system? game manager?
    [SerializeField] private GameObject _characterStatText;
    [SerializeField] private GameObject _weaponStatText;
    public Image _characterImage;
    public Image _weaponImage;

    //Learned Skill UI
    [Header("Learned Skill UI")]
    [SerializeField] private SkillManager _skillManager;
    public GameObject skillIconPrefab;
    private List<GameObject> _skillIconUIs = new List<GameObject>();
    [SerializeField] private GameObject _iconScrollContents;
    private readonly Vector3 ICON_INIT_POSITION = new Vector3(235, 280, 0);
    private const float ICON_INTERVAL = 100;

    //Item UI
    [Header("Item UI")]
    [SerializeField] private GameObject _ItemPanel;
    [SerializeField] private GameObject _weaponItemPanel;
    [SerializeField] private GameObject _usableItemPanel;
    [SerializeField] private GameObject _otherItemPanel;
    [SerializeField] private ItemManager _itemManager;
    public GameObject ItemListPrefab;
    private List<GameObject> _itemLists = new List<GameObject>();
    [SerializeField] private GameObject _weaponItemListScrollContents;
    [SerializeField] private GameObject _usableItemListScrollContents;
    [SerializeField] private GameObject _otherItemListScrollContents;
    public const int ITEM_LIST_INIT_COUNT = 20;
    private readonly Vector3 ITEM_LIST_INIT_POSITION = new Vector3(1347.5f, 690, 0);
    private const float ITEM_LIST_INTERVAL = 100;

    [SerializeField] private GameObject _itemUseWindow;
    private int _currentItemIndex;

    private ItemInfo.ItemCategory _currentItemUIStatus = ItemInfo.ItemCategory.Weapon;

    [SerializeField] private GameObject _moneyText;

    // Start is called before the first frame update
    void Start()
    {
        //Skill icon object pooling
        List<Skill> _skills = _skillManager.GetAllSkills();
        for (int i = 0; i < _skills.Count; i++) 
        {
            Vector3 pos = ICON_INIT_POSITION;
            pos.x += i * ICON_INTERVAL;
            GameObject skillIcon = Instantiate(skillIconPrefab, pos, Quaternion.identity);
            skillIcon.transform.SetParent(_iconScrollContents.transform);

            skillIcon.SetActive(false);
            _skillIconUIs.Add(skillIcon);
        }

        SetMoneyText();
        ShowWeaponItems();
        //Item list object pooling
        ExpandItemLists();

        _currentItemIndex = -1;
    }

    public void OpenCharacterUI() 
    {
        SetStatText();
        SetLearnedSkiilInfoUI();
        SetItemLists();
    }

    private void SetStatText()
    {
        UnitStat playerStat = gameManager.playerStat;
        Weapon weapon = gameManager.playerWeapon;
        WeaponType weaponType;
        //in test development
        if (weapon == null) { weaponType = WeaponType.Repeater; }
        else { weaponType = weapon.GetWeaponType(); }

        SetCharacterStatText(playerStat);
        SetWeaponStatText(playerStat, weaponType);
    }
    private void SetCharacterStatText(UnitStat playerStat)
    {
        string text = playerStat.hp.ToString() + '\n' +
                      playerStat.concentration.ToString() + '\n' +
                      playerStat.sightRange.ToString() + '\n' +
                      playerStat.speed.ToString() + '\n' +
                      playerStat.actionPoint.ToString() + '\n' +
                      playerStat.additionalHitRate.ToString() + '\n' +
                      playerStat.criticalChance.ToString();

        _characterStatText.GetComponent<TextMeshProUGUI>().text = text;
    }
    private void SetWeaponStatText(UnitStat playerStat, WeaponType weaponType)
    {
        string text = "";
        switch (weaponType) 
        {
            case WeaponType.Repeater: 
                {
                    text += playerStat.repeaterAdditionalDamage.ToString() + '\n' +
                            playerStat.repeaterAdditionalRange.ToString() + '\n' +
                            playerStat.repeaterCriticalDamage.ToString();
                    break;
                }
            case WeaponType.Revolver:
                {
                    text += playerStat.revolverAdditionalDamage.ToString() + '\n' +
                            playerStat.revolverAdditionalRange.ToString() + '\n' +
                            playerStat.revolverCriticalDamage.ToString();
                    break;
                }
            case WeaponType.Shotgun:
                {
                    text += playerStat.shotgunAdditionalDamage.ToString() + '\n' +
                            playerStat.shotgunAdditionalRange.ToString() + '\n' +
                            playerStat.shotgunCriticalDamage.ToString();
                    break; 
                }
        }

        _weaponStatText.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SetLearnedSkiilInfoUI()
    {
        for (int i = 0; i < _skillIconUIs.Count; i++)
        {
            _skillIconUIs[i].SetActive(false);
        }

        List<Skill> _skills = _skillManager.GetAllSkills();
        int cnt = 0;
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].isLearned)
            {
                Vector3 pos = ICON_INIT_POSITION;
                pos.x += cnt * ICON_INTERVAL;
                _skillIconUIs[i].transform.position = pos;
                _skillIconUIs[i].SetActive(true);

                cnt++;
            }
        }
        _iconScrollContents.GetComponent<RectTransform>().sizeDelta = new Vector2(cnt * ICON_INTERVAL + 25, 100);
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
            _itemLists[i].transform.SetParent(_ItemPanel.transform);
        }
        int itemListCount = 0;
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
        _itemLists[index].transform.position = pos;
        _itemLists[index].transform.SetParent(parents.transform);
        _itemLists[index].GetComponent<ItemListElement>().SetItemListElement(item, this);

    }
    private void ExpandItemLists() 
    {
        for (int i = 0; i < ITEM_LIST_INIT_COUNT; i++)
        {
            Vector3 pos = ITEM_LIST_INIT_POSITION;
            GameObject itemList = Instantiate(ItemListPrefab, pos, Quaternion.identity);
            itemList.transform.SetParent(_ItemPanel.transform);

            itemList.SetActive(false);
            _itemLists.Add(itemList);
        }
    }

    public void ClickCharacterUIButton(int index) 
    {
        //Debug.Log("클릭한 아이템 인덱스: " + index);
        if (!_itemUseWindow.activeSelf)
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
        _itemUseWindow.transform.position = Input.mousePosition;
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
        _itemUseWindow.SetActive(true);
    }

    public void CloseItemUseWindow() 
    {
        _currentItemIndex = -1;
        _itemUseWindow.SetActive(false);
    }

    public void OnClickTestBtn(int i)
    {
        //for test
        _itemManager.AddItem(i);
        SetItemLists();
    }
    public void UseItem() 
    {
        _itemManager.UseItem(_currentItemIndex);
        SetItemLists();
        CloseItemUseWindow();
    }
    public void SellItem()
    {
        _itemManager.SellItem(_currentItemIndex);
        SetItemLists();
        CloseItemUseWindow();

        SetMoneyText();
    }
    public void DiscardItem()
    {
        _itemManager.DiscardItem(_currentItemIndex);
        SetItemLists();
        CloseItemUseWindow();
    }

    private void SetMoneyText() 
    {
        _moneyText.GetComponent<TextMeshProUGUI>().text = "Money: " + _itemManager.money.ToString() + "$";
    }
}
