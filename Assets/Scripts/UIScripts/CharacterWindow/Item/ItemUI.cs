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
    [SerializeField] private GameObject _inventoryButtons;
    [SerializeField] private GameObject _draggedElement;
    [SerializeField] private GameObject _equippedElement;

    private ItemType _displayInventoryType = ItemType.Revolver;

    private GameObject _originalDraggedElement;
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
        GameManager.instance.playerInventory.InitEquippedItem(startItem);
        SetInventoryUI();

        IInventory.OnInventoryChanged.AddListener(SetInventoryUI);
    }
    public override void OpenUI()
    {
        base.OpenUI();
        ClosePopupWindow();
    }

    public void SetInventoryUI() 
    {
        //_inventory 불러오기
        Inventory inventory = (Inventory)GameManager.instance.playerInventory;
        if (inventory is null) return;
        List<IItem> items = (List<IItem>)inventory.GetItems(_displayInventoryType);
        if (items is null) return;

        _equippedItem = _equippedElement.GetComponent<InventoryUIElement>().item;
        for (int i = 0; i < _inventoryUI.transform.childCount; i++)
        {
            _inventoryUI.transform.GetChild(i).GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        }
        int cnt = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == _equippedItem)
            {
                //continue;
            }
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
    public void OpenInventoryTooltip(Item item, Vector3 pos)
    {
        if (_inventoryButtons.gameObject.activeSelf is true) return;
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(item, pos);
    }
    public void OpenInventoryInteraction(Item item, Vector3 pos)
    {
        if (item == _equippedItem) return;
        _inventoryButtons.GetComponent<InventoryInteractionUI>().SetInventoryInteractionUI(item, pos);
        pos.x += _inventoryButtons.GetComponent<RectTransform>().sizeDelta.x;
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(item, pos);
    }
    public override void ClosePopupWindow()
    {
        if (UIManager.instance.currentLayer != 3)
        {
            _inventoryButtons.GetComponent<InventoryInteractionUI>().CloseUI();
        }
        if (!IsMouseOverTooltip(_inventoryTooltip) && _inventoryButtons.gameObject.activeSelf is false)
        {
            _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        }
    }
    private bool IsMouseOverTooltip(GameObject tooltip)
    {
        if (tooltip.gameObject.activeSelf is false) return false;
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        foreach (RaycastResult r in results)
        {
            if (r.gameObject == tooltip)
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
    }

    private int GetInventoryUIIndex(GameObject element)
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
}
