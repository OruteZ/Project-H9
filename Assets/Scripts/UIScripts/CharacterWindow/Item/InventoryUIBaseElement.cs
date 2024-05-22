using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIBaseElement : UIElement
{
    [SerializeField] private GameObject _itemIcon;
    [SerializeField] protected GameObject _itemCountText;
    [SerializeField] protected GameObject _itemFrame = null;

    [SerializeField] private Sprite _nullImage;

    public Item item { get; private set; }
    private void Awake()
    {
        item = null;
    }

    public void SetInventoryUIElement(Item item)
    {
        if (item is null)
        {
            return;
        }
        this.item = item;
        Sprite texture = item.GetData().icon;
        if (texture == null)
        {
            texture = _nullImage;
        }
        _itemIcon.GetComponent<Image>().sprite = texture;
        _itemIcon.GetComponent<Image>().color = Color.white;

        string countText = item.GetStackCount().ToString();
        if (item.GetStackCount() == 0 || item is WeaponItem)
        {
            countText = "";
        }
        if (_itemCountText != null)
        {
            _itemCountText.GetComponent<TextMeshProUGUI>().text = countText;
        }
    }
    public void ClearInventoryUIElement()
    {
        item = null;
        _itemIcon.GetComponent<Image>().sprite = null;
        _itemIcon.GetComponent<Image>().color = UICustomColor.TransparentColor;
        _itemCountText.GetComponent<TextMeshProUGUI>().text = "";
    }
}
