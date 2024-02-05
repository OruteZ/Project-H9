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
        //gameObject.SetActive(_item != item);
        if (!gameObject.activeSelf) return;
        
        UIManager.instance.previousLayer = 3;
        _item = item;

        GetComponent<RectTransform>().position = pos;

        _itemNameText.GetComponent<TextMeshProUGUI>().text = item.GetData().nameIdx.ToString();
        _itemDescriptionText.GetComponent<TextMeshProUGUI>().text = item.GetData().descriptionIdx.ToString();
    }
}
