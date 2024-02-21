using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class InventoryUITooltip : UIElement,IPointerExitHandler
{
    [SerializeField] private GameObject _itemNameText;
    [SerializeField] private GameObject _itemDescriptionText;

    private IItem _item = null;

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.ClosePopupWindow();
    }

    public void SetInventoryUITooltip(IItem item, Vector3 pos)
    {
        if (_item == item && GetComponent<RectTransform>().position == pos) return;

        if (item is null)
        {
            CloseUI();
            return;
        }
        _item = item;
        GetComponent<RectTransform>().position = pos;

        UIManager.instance.previousLayer = 3;

        _itemNameText.GetComponent<TextMeshProUGUI>().text = item.GetData().nameIdx.ToString();
        _itemDescriptionText.GetComponent<TextMeshProUGUI>().text = item.GetData().descriptionIdx.ToString();

        gameObject.SetActive(true);
    }
    public override void CloseUI()
    {
        _item = null;
        gameObject.SetActive(false);
    }
}
