using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : UISystem
{
    [SerializeField] private GameObject _moneyText;

    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _inventoryTooltip;
    [SerializeField] private GameObject _inventoryInteractionButtons;
    [SerializeField] private GameObject _draggedElement;
    [SerializeField] private GameObject _equippedElement;

    private ItemType _displayInventoryType = ItemType.Revolver;

    private GameObject _currentMouseOverElement = null;
    private GameObject _interactionElement = null;
    private GameObject _originalDraggedElement = null;
    private Item _interactionItem = null;
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

        Item startItem = Item.CreateItem(GameManager.instance.itemDatabase.GetItemData(GameManager.instance.PlayerWeaponIndex));
        _equippedElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(startItem);
        GameManager.instance.playerInventory.InitEquippedItem(startItem);
        SetInventoryUI();

        IInventory.OnInventoryChanged.AddListener(SetInventoryUI);
    }
    public override void OpenUI()
    {
        base.OpenUI();
        ClosePopupWindow();
        SetInventoryUI();
    }

    public void SetInventoryUI() 
    {
        //_inventory 불러오기
        Inventory inventory = GameManager.instance.playerInventory;
        if (inventory is null) return;
        List<IItem> items = (List<IItem>)inventory.GetItems(_displayInventoryType);
        if (items is null) return;

        _equippedItem = (Item)inventory.GetEqippedItem();
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
        _moneyText.GetComponent<TextMeshProUGUI>().text = GameManager.instance.playerInventory.GetMoney().ToString() + "$";
    }
    public void OpenInventoryTooltip(GameObject ui, Vector3 pos)
    {
        if (_inventoryInteractionButtons.gameObject.activeSelf is true) return;
        _currentMouseOverElement = ui;
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(ui.GetComponent<InventoryUIElement>().item, pos);
    }
    public void OpenInventoryInteraction(GameObject ui)
    {
        _interactionElement = ui;
        _interactionItem = ui.GetComponent<InventoryUIElement>().item;
        Vector3 pos = ui.GetComponent<RectTransform>().position;
        if (_interactionItem == _equippedItem) return;
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().SetInventoryInteractionUI(_interactionItem, pos);
        pos.x += _inventoryInteractionButtons.GetComponent<RectTransform>().sizeDelta.x;
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(_interactionItem, pos);
    }
    public override void ClosePopupWindow()
    {
        if (UIManager.instance.currentLayer != 3)
        {
            _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        }
        if (!IsMouseOverUI(_inventoryTooltip) && _inventoryInteractionButtons.gameObject.activeSelf is false)
        {
            _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
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
                if (isEquipmentCell && isWeaponItem)
                {
                    if (!_draggedItem.TryEquip())
                    {
                        continue;
                    }
                    GameManager.instance.playerInventory.EqipItem(_draggedItem.GetData().itemType, GetInventoryUIIndex(_originalDraggedElement));
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
        _displayInventoryType = ItemType.Revolver;
        SetInventoryUI();
    }
    public void ClickConsumableBtn()
    {
        _displayInventoryType = ItemType.Heal;
        SetInventoryUI();
    }
    public void ClickOtherBtn()
    {
        _displayInventoryType = ItemType.Etc;
        SetInventoryUI();
    }

    public void ClickUseItemBtn()
    {
        if (_inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().isEqipable)
        {
            //Item tmpItem = _interactionItem;
            //_interactionElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(_equippedItem);
            //_equippedElement.GetComponent<InventoryUIElement>().SetInventoryUIElement(tmpItem);

            GameManager.instance.playerInventory.EqipItem(_interactionItem.GetData().itemType, GetInventoryUIIndex(_interactionElement));
        }
        else 
        {
            GameManager.instance.playerInventory.UseItem(_interactionItem.GetData().itemType, GetInventoryUIIndex(_interactionElement));
            UIManager.instance.gameSystemUI.OnCharacterBtnClick();
        }
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        SetInventoryUI();
    }
    public void ClickSellItemBtn()
    {
        GameManager.instance.playerInventory.SellItem(_interactionItem.GetData().itemType, GetInventoryUIIndex(_interactionElement));
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        SetInventoryUI();
    }
    public void ClickRemoveItemBtn()
    {
        GameManager.instance.playerInventory.DeleteItem(_interactionItem);
        _inventoryInteractionButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        SetInventoryUI();
    }
}
