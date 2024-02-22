using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : UISystem
{
    [SerializeField] private GameObject _sortButton;
    [SerializeField] private GameObject _moneyText;

    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _inventoryTooltip;
    [SerializeField] private GameObject _draggedElement;
    [SerializeField] private GameObject _equippedElement;

    private Inventory _inventory;

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
        SetInventoryUI();
        //SetInventory를 호출할 이벤트에 AddListner(SetInventory) 추가
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
        _inventory = (Inventory)GameManager.instance.playerInventory;
        if (_inventory is null) return;
        List<IItem> items = (List<IItem>)_inventory.GetItems();
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
                continue;
            }
            _inventoryUI.transform.GetChild(cnt++).GetComponent<InventoryUIElement>().SetInventoryUIElement((Item)items[i]);
            if (cnt >= _inventoryUI.transform.childCount) break;
        }
        ClosePopupWindow();
    }
    private void SetMoneyUI() 
    {

    }
    public void OpenInventoryTooltip(Item item, Vector3 pos)
    {
        _sortButton.GetComponent<InventorySortButtonUI>().CloseTooltip();
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(item, pos);
    }
    public override void ClosePopupWindow()
    {
        _sortButton.GetComponent<InventorySortButtonUI>().CloseTooltip();
        if (!IsMouseOverTooltip(_sortButton.transform.GetChild(1).gameObject))
        {
            //is it necessary?
        }
        if (!IsMouseOverTooltip(_inventoryTooltip))
        {
            _inventoryTooltip.GetComponent<InventoryUITooltip>().CloseUI();
        }
    }
    private bool IsMouseOverTooltip(GameObject tooltip)
    {
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
            if (result.gameObject.TryGetComponent<InventoryDragUI>(out var a))
            {
                continue;
            }
            if (result.gameObject.TryGetComponent<InventoryUIElement>(out var b))
            {
                Item tmpItem = b.item;

                bool isEquipmentCell = (result.gameObject == _equippedElement);
                bool isWeaponItem = (_draggedItem is WeaponItem);
                if (isEquipmentCell && isWeaponItem)
                {
                    if (!_draggedItem.TryEquip())
                    {
                        continue;
                    }
                }
                b.SetInventoryUIElement(_draggedItem);
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
}
