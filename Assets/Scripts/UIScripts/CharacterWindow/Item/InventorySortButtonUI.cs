using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySortButtonUI : UIElement
{
    [SerializeField] private GameObject _sortTooltip;
    public void ClickInventorySortBtn()
    {
        if (_sortTooltip.activeSelf) return;
        _sortTooltip.SetActive(!_sortTooltip.activeSelf);

        UIManager.instance.previousLayer = 3;
    }
    public void ClickSortByNameBtn() 
    {
        IInventory inven = null;
    }
    public void CloseTooltip()
    {
        _sortTooltip.SetActive(false);
    }
}
