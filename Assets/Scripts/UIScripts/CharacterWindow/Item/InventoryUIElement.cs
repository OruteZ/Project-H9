using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUIElement : UIElement, IPointerClickHandler
{
    [SerializeField] private GameObject _itemIcon;
    [SerializeField] private GameObject _itemCountText;

    private IItem _item = null;

    public void SetInventoryUIElement(IItem item) 
    {
        if (item is null)
        {
            return;
        }
        
        _item = item;
        //_itemIcon.GetComponent<Image>().sprite = ?
        _itemIcon.GetComponent<Image>().color = Color.white;

        var data = item.GetData();
        
        string countText = item.GetStackCount().ToString();
        if (item.GetStackCount() == 0) 
        {
            countText = "";
        }
        _itemCountText.GetComponent<TextMeshProUGUI>().text = countText;
    }
    public void ClearInventoryUIElement() 
    {
        _item = null;
        _itemIcon.GetComponent<Image>().sprite = null;
        _itemIcon.GetComponent<Image>().color = UICustomColor.TransparentColor;
        _itemCountText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.OpenInventoryTooltip(_item, GetComponent<RectTransform>().position);
    }
}
