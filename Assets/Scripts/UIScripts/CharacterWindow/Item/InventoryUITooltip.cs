using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUITooltip : UIElement
{
    [SerializeField] private GameObject _itemNameText;
    [SerializeField] private GameObject _itemDescriptionText;

    private IItem _item = null;
    public void SetInventoryUITooltip(IItem item, Vector3 pos)
    {
        gameObject.SetActive(item is not null);
        if (!gameObject.activeSelf) return;

        _item = item;
        UIManager.instance.previousLayer = 3;

        GetComponent<RectTransform>().position = pos;

        _itemNameText.GetComponent<TextMeshProUGUI>().text = item.GetData().nameIdx.ToString();
        _itemDescriptionText.GetComponent<TextMeshProUGUI>().text = item.GetData().descriptionIdx.ToString();
    }
}
