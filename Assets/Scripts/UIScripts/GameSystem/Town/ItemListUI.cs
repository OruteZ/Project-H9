using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemListUI : UIElement
{
    [SerializeField] private GameObject _itemListElementPrefab;
    [SerializeField] private GameObject _itemListElementContainer;
    [SerializeField] private GameObject _listShadow;
    [SerializeField] private GameObject _listItemTooltip;
    [SerializeField] private GameObject _equippedItemTooltip;

    private ItemListPool _listPool = null;

    private ItemType _itemType = ItemType.Revolver;
    private int _townIndex;
    private ItemUI _itemUI;

    private void Awake()
    {
        CloseItemListTooltip();
        if (_listPool == null)
        {
            _listPool = new ItemListPool();
            _listPool.Init("Prefab/UI/Item List UI Element", _itemListElementContainer.transform, 0);
        }
    }
    private void Start()
    {
        transform.Find("Item Type Button/Weapon Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[50];
        transform.Find("Item Type Button/Consumable Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[51];
    }
    public void SetItemListUI(int townIndex, ItemUI itemUI)
    {
        int[] itemIndexes = null;
        foreach (var tileObj in FieldSystem.tileSystem.GetAllTileObjects())
        {
            if (tileObj is Town && tileObj.GetComponent<Town>().GetTownType() == Town.BuildingType.Ammunition && tileObj.GetComponent<Town>().GetTownIndex() == townIndex) 
            {
                itemIndexes = tileObj.GetComponent<Town>().townItemIndexes;
                break;
            } 
        }
        if (itemIndexes is null)
        {
            Debug.LogError("Can't Create Ammunition Item List");
            return;
        }

        List<ItemData> iDatas = new();
        foreach (int idx in itemIndexes) 
        {
            ItemData iData = GameManager.instance.itemDatabase.GetItemData(idx);
            if (iData is null) 
            {
                Debug.LogError("Can't Create Ammunition Item List");
                return;
            }
            if ((_itemType is not ItemType.Revolver) && (iData.itemType is ItemType.Revolver or ItemType.Repeater or ItemType.Shotgun)) continue;
            if ((_itemType is not ItemType.Heal) && !(iData.itemType is ItemType.Revolver or ItemType.Repeater or ItemType.Shotgun)) continue;

            iDatas.Add(iData);
        }

        _listPool.Reset();
        foreach (var i in iDatas)
        {
            var t = _listPool.Set();
            t.Instance.GetComponent<ItemListUIElement>().SetItemListUIElement(i, this);
            t.Instance.transform.SetAsLastSibling();
        }

        _townIndex = townIndex;
        _itemUI = itemUI;
        itemUI.TypeButtonLocalize();
        _listShadow.GetComponent<Image>().enabled = (GetComponent<RectTransform>().sizeDelta.y <= _itemListElementContainer.GetComponent<RectTransform>().sizeDelta.y);
    }

    private void Update()
    {
        Vector3 tmpPos = _listItemTooltip.GetComponent<RectTransform>().position;
        tmpPos.y -= _listItemTooltip.GetComponent<RectTransform>().sizeDelta.y * UIManager.instance.GetCanvasScale();
        _equippedItemTooltip.GetComponent<RectTransform>().position = tmpPos;
    }
    public void SetItemListTooltip(ItemData itemData, Vector3 pos)
    {
        _listItemTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(itemData, pos);
        _listItemTooltip.SetActive(true);

        if (itemData.itemType is ItemType.Revolver or ItemType.Repeater or ItemType.Shotgun)
        {
            _equippedItemTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(GameManager.instance.playerInventory.GetEquippedItem().GetData(), pos, false, true);
            _equippedItemTooltip.SetActive(true);
        }
    }
    public void CloseItemListTooltip()
    {
        _listItemTooltip.SetActive(false);
        _equippedItemTooltip.SetActive(false);
    }

    public void BuyItem(ItemData iData) 
    {
        if (GameManager.instance.playerInventory.GetGold() < iData.itemPrice) return;
        
        if (GameManager.instance.playerInventory.TryAddItem(Item.CreateItem(iData)))
        {
            SoundManager.instance.PlaySFX("UI_SellBuyItem");
            GameManager.instance.playerInventory.AddGold(-iData.itemPrice);

            if (_itemUI is not null)
            {
                if (iData.itemType is ItemType.Revolver or ItemType.Repeater or ItemType.Shotgun) _itemUI.ClickWeaponBtn();
                else if (iData.itemType is ItemType.Etc) _itemUI.ClickOtherBtn();
                else _itemUI.ClickConsumableBtn();
            }
        }
    }

    public void OnClickWeaponBtn()
    {
        SoundManager.instance.PlaySFX("UI_ButtonClick");
        _itemType = ItemType.Revolver;
        SetItemListUI(_townIndex, _itemUI);
    }
    public void OnClickConsumableBtn()
    {
        SoundManager.instance.PlaySFX("UI_ButtonClick");
        _itemType = ItemType.Heal;
        SetItemListUI(_townIndex, _itemUI);
    }
}
