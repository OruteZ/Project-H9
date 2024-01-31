using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUI : UISystem
{
    [SerializeField] private GameObject _sortButton;
    [SerializeField] private GameObject _moneyText;

    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _inventoryTooltip;

    private IInventory _inventory;

    private void Start()
    {
        for (int i = 0; i < _inventoryUI.transform.childCount; i++)
        {
            _inventoryUI.transform.GetChild(i).GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        }
        ClosePopupWindow();
        SetInventoryUI();
        //SetInventory를 호출할 이벤트에 AddListner(SetInventory) 추가
    }

    private void SetInventoryUI() 
    {
        if (_inventoryUI is null) return;
        //_inventory 불러오기
        _inventory = null;
        if (_inventory is null) return;

        int cnt = 0;
        IEnumerable<IItem> items = _inventory.GetItems();
        while (true) 
        {
            if (items is null) break;
            if (cnt > _inventoryUI.transform.childCount) break;

            _inventoryUI.transform.GetChild(cnt).GetComponent<InventoryUIElement>().SetInventoryUIElement(items.GetEnumerator().Current);

            items.GetEnumerator().MoveNext();
            cnt++;
        }
        for (int i = cnt; i < _inventoryUI.transform.childCount; i++)
        {
            _inventoryUI.transform.GetChild(i).GetComponent<InventoryUIElement>().ClearInventoryUIElement();
        }
    }
    private void SetMoneyUI() 
    {

    }
    public void OpenInventoryTooltip(IItem item, Vector3 pos)
    {
        _inventoryTooltip.GetComponent<InventoryUITooltip>().SetInventoryUITooltip(item, pos);
    }
    public override void ClosePopupWindow()
    {
        _inventoryTooltip.SetActive(false);
        _sortButton.GetComponent<InventorySortButtonUI>().CloseTooltip();
    }
}
