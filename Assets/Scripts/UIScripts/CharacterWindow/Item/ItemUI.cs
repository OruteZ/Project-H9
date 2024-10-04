using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemUI : UISystem
{
    [SerializeField] private GameObject _moneyText;
    [SerializeField] private GameObject _itemTypeButton;

    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _inventoryTooltip;
    [SerializeField] private GameObject _equippedTooltip;
    [SerializeField] private GameObject _inventoryInteractionButtons;
    [SerializeField] private GameObject _draggedElement;
    [SerializeField] private GameObject _equippedElement;

    private ItemType _displayInventoryType = ItemType.Revolver;

    private GameObject _currentMouseOverElement = null;
    private GameObject _interactionElement = null;
    private GameObject _originalDraggedElement = null;
    private Item _interactionItem = null;
    private int _interactionIndex = -1;
    private Item _draggedItem = null;
    private Item _equippedItem = null;

    private void Start()
    {
        _draggedElement.GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        _equippedElement.GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        for (int i = 0; i < _inventoryUI.transform.childCount; i++)
        {
            _inventoryUI.transform.GetChild(i).GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        }
        ClosePopupWindow();

        Item startItem = Item.CreateItem(GameManager.instance.itemDatabase.GetItemData(GameManager.instance.playerWeaponIndex));
        _equippedElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(startItem);
        //GameManager.instance.playerInventory.InitEquippedItem(startItem);
        SetInventoryUI();

        IInventory.OnInventoryChanged.AddListener(SetInventoryUI);
    }
    private void Update()
    {
        float inventoryTooltipYSize = _inventoryTooltip.GetComponent<RectTransform>().sizeDelta.y * UIManager.instance.GetCanvasScale();

        Vector3 tmpPos = _inventoryTooltip.GetComponent<RectTransform>().position;
        tmpPos.y -= inventoryTooltipYSize;
        tmpPos = ScreenOverCorrector.GetCorrectedUIPosition(GetComponent<Canvas>(), tmpPos, _equippedTooltip);

        _equippedTooltip.GetComponent<RectTransform>().position = tmpPos;
        tmpPos.y += inventoryTooltipYSize;
        _inventoryTooltip.GetComponent<RectTransform>().position = tmpPos;
    }
    public override void OpenUI()
    {
        TypeButtonLocalize();
        int idx = _displayInventoryType switch
        {
            ItemType.Null => -1,
            ItemType.Etc => 2,
            ItemType.Character => 0,
            ItemType.Revolver => 0,
            ItemType.Repeater => 0,
            ItemType.Shotgun => 0,
            ItemType.Heal => 1,
            ItemType.Damage => 1,
            ItemType.Cleanse => 1,
            ItemType.Buff => 1,
            ItemType.Debuff => 1,
            _ => throw new NotImplementedException()
        };
        _itemTypeButton.transform.GetChild(idx).GetComponent<Button>().Select();
        base.OpenUI();
        ClosePopupWindow();
        SetInventoryUI();
    }
    public void TypeButtonLocalize()
    {
        _itemTypeButton.transform.Find("Weapon Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[50];
        _itemTypeButton.transform.Find("Consumable Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[51];
        _itemTypeButton.transform.Find("Other Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[52];
    }

    public void SetInventoryUI() 
    {
        //_inventory 불러오기
        Inventory inventory = GameManager.instance.playerInventory;
        if (inventory is null) return;
        List<IItem> items = (List<IItem>)inventory.GetItems(_displayInventoryType);
        if (items is null) return;

        _equippedItem = (Item)inventory.GetEquippedItem();
        _equippedElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(_equippedItem);
        for (int i = 0; i < _inventoryUI.transform.childCount; i++)
        {
            _inventoryUI.transform.GetChild(i).GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        }
        int cnt = 0;
        for (int i = 0; i < items.Count; i++)
        {
            _inventoryUI.transform.GetChild(cnt++).GetComponent<InventoryUIElement>().SetInventoryUIElement((Item)items[i]);
            if (cnt >= _inventoryUI.transform.childCount) break;
        }

        SetMoneyUI();
        ClosePopupWindow();
    }
    private void SetMoneyUI() 
    {
        _moneyText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.playerInventory.GetGold().ToString() + "¢";
    }
    public void OpenInventoryTooltip(GameObject ui, Vector3 pos)
    {
        if (_inventoryInteractionButtons.gameObject.activeSelf is true) return;
        _currentMouseOverElement = ui;
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(ui, pos, false, ui == _equippedElement);


        Item item = ui.GetComponent<InventoryUIBaseElement>().item;
        bool isItemExist = (item != null);
        bool isItemEquipped = (ui == _equippedElement);
        if (!isItemExist || isItemEquipped) return;

        bool isItemWeapon = (item.GetData().itemType is ItemType.Revolver or ItemType.Repeater or ItemType.Shotgun);
        if(isItemWeapon) _equippedTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(_equippedItem.GetData(), pos, false, true);

    }

    public void OpenInventoryInteraction(GameObject ui)
    {
        SoundManager.instance.PlaySFX("UI_ButtonClick");
        _interactionElement = ui;
        for (int i = 0; i < _inventoryUI.transform.childCount; i++) 
        {
            if (_inventoryUI.transform.GetChild(i).gameObject == ui) 
            {
                _interactionIndex = i;
                break;
            }
        }

        _interactionItem = ui.GetComponent<InventoryUIElement>().item;
        Vector3 pos = ui.GetComponent<RectTransform>().position;
        if (_interactionItem == _equippedItem) return;
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().SetInventoryInteractionUI(_interactionItem, pos);
        pos.x += _inventoryInteractionButtons.GetComponent<RectTransform>().sizeDelta.x;
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(_interactionItem.GetData(), pos);
    }
    public override void ClosePopupWindow()
    {
        if (UIManager.instance.currentLayer != 3)
        {
            _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        }
        if (!IsMouseOverUI(_inventoryTooltip) && !IsMouseOverUI(_equippedTooltip) && _inventoryInteractionButtons.gameObject.activeSelf is false)
        {
            _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
            _equippedTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        }
    }
    private bool IsMouseOverUI(GameObject ui)
    {
        if (ui is null) return false;
        if (ui.gameObject.activeSelf is false) return false;

        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        foreach (RaycastResult r in results)
        {
            if (r.gameObject == ui)
            {
                return true;
            }
        }
        return false;
    }

    public void StartDragInventoryElement(GameObject element) 
    {
        if (element == _equippedElement) return;
        _draggedItem = element.GetComponent<InventoryUIElement>().item;
        if (_draggedItem is null) 
        {
            ClosePopupWindow();
            return;
        }
        _originalDraggedElement = element;
        element.GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        _draggedElement.GetComponent<InventoryDragUI>().SetInventoryUIElement(_draggedItem);
        _draggedElement.GetComponent<InventoryDragUI>().StartDragging();
        _draggedElement.GetComponent<RectTransform>().position = element.GetComponent<RectTransform>().position;
        ClosePopupWindow();
        if (_draggedItem is WeaponItem)
        {
            GetComponent<EquipmentUI>().SetEquipmentParticle();
        }
    }

    public void StopDragInventoryElement()
    {
        if (_draggedItem is null) return;
        bool isDropped = false;
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent<InventoryDragUI>(out var drag))
            {
                continue;
            }
            if (result.gameObject.TryGetComponent<InventoryUIElement>(out var droppedPosition))
            {
                Item tmpItem = droppedPosition.item;

                bool isEquipmentCell = (droppedPosition.gameObject == _equippedElement);
                bool isWeaponItem = (_draggedItem is WeaponItem);
                if (isEquipmentCell)
                {
                    if (GameManager.instance.CompareState(GameState.COMBAT) && FieldSystem.unitSystem.GetPlayer().currentActionPoint < Inventory.WEAPON_COST) break;
                    if (!isWeaponItem || !_draggedItem.TryEquip()) continue;
                    GameManager.instance.playerInventory.EquipItem(_draggedItem.GetData().itemType, GetInventoryUIIndex(_originalDraggedElement));
                }
                else
                {
                    GameManager.instance.playerInventory.SwapItem(_draggedItem.GetData().itemType, GetInventoryUIIndex(_originalDraggedElement), GetInventoryUIIndex(droppedPosition.gameObject));
                }
                //swap item
                droppedPosition.SetInventoryUIElement(_draggedItem);
                _originalDraggedElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(tmpItem);
                isDropped = true;
                break;
            }
        }

        if (!isDropped)
        {
            _originalDraggedElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(_draggedItem);
        }
        _draggedItem = null;
        _originalDraggedElement = null;
        _draggedElement.GetComponent<InventoryDragUI>().ClearInventoryUIElement();
        _draggedElement.GetComponent<InventoryDragUI>().StopDragging();
        _draggedElement.GetComponent<RectTransform>().position = Vector3.zero;

        SetInventoryUI();
        GetComponent<EquipmentUI>().ClearEquipmentParticle();
    }

    public int GetInventoryUIIndex(GameObject element)
    {
        for (int i = 0; i < _inventoryUI.transform.childCount; i++)
        {
            if (_inventoryUI.transform.GetChild(i).gameObject == element) 
            {
                return i;
            }
        }
        Debug.LogError("Can't find inventory ui index");
        return -1;
    }

    public void ClickWeaponBtn()
    {
        SoundManager.instance.PlaySFX("UI_ButtonClick");
        _displayInventoryType = ItemType.Revolver;
        SetInventoryUI();
    }
    public void ClickConsumableBtn()
    {
        SoundManager.instance.PlaySFX("UI_ButtonClick");
        _displayInventoryType = ItemType.Heal;
        SetInventoryUI();
    }
    public void ClickOtherBtn()
    {
        SoundManager.instance.PlaySFX("UI_ButtonClick");
        _displayInventoryType = ItemType.Etc;
        SetInventoryUI();
    }

    public void ClickUseItemBtn()
    {
        SoundManager.instance.PlaySFX("UI_ButtonClick");
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (_inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().isEquipable)
        {
            if (GameManager.instance.CompareState(GameState.COMBAT) && (player.currentActionPoint < Inventory.WEAPON_COST)) return;
            //Item tmpItem = _interactionItem;
            //_interactionElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(_equippedItem);
            //_equippedElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(tmpItem);

            GameManager.instance.playerInventory.EquipItem(_interactionItem.GetData().itemType, GetInventoryUIIndex(_interactionElement));
        }
        else
        {
            if (GameManager.instance.CompareState(GameState.COMBAT) && (player.currentActionPoint < Inventory.ITEM_COST)) return;
            GameManager.instance.playerInventory.UseItem(_interactionItem.GetData().itemType, GetInventoryUIIndex(_interactionElement));
            UIManager.instance.SetUILayer(1);
            //UIManager.instance.gameSystemUI.OnCharacterBtnClick();
        }
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        _equippedTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        SetInventoryUI();
    }
    public void ClickSellItemBtn()
    {
        SoundManager.instance.PlaySFX("UI_SellBuyItem");
        GameManager.instance.playerInventory.SellItem(_interactionItem.GetData().itemType, GetInventoryUIIndex(_interactionElement));
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        _equippedTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        SetInventoryUI();
    }
    public void ClickRemoveItemBtn()
    {
        SoundManager.instance.PlaySFX("UI_RemoveItem");
        GameManager.instance.playerInventory.DeleteItem(_interactionItem, _interactionIndex);
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        _equippedTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        SetInventoryUI();
    }
}
