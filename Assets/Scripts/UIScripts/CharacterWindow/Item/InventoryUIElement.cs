using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUIElement : UIElement, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _itemIcon;
    [SerializeField] protected GameObject _itemCountText;
    [SerializeField] protected GameObject _itemFrame = null;

    public Item item { get; private set; }
    //public int idx;
    private void Awake()
    {
        item = null;
    }

    public void SetInventoryUIElement(Item item) 
    {
        //_itemIcon.GetComponent<Image>().sprite = ?
        if (item is null) 
        {
            return;
        }
        this.item = item;
        //idx = item.GetData().id;
        Sprite texture = item.GetData().icon;
        if (texture is not null)
        {
            _itemIcon.GetComponent<Image>().sprite = texture;
        }
        _itemIcon.GetComponent<Image>().color = Color.white;

        var data = item.GetData();
        
        string countText = item.GetStackCount().ToString();
        if (item.GetStackCount() == 0 || item is WeaponItem) 
        {
            countText = "";
        }
        _itemCountText.GetComponent<TextMeshProUGUI>().text = countText;
    }
    public void ClearInventoryUIElement() 
    {
        item = null;
        _itemIcon.GetComponent<Image>().sprite = null;
        _itemIcon.GetComponent<Image>().color = UICustomColor.TransparentColor;
        _itemCountText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.instance.characterUI.itemUI.StartDragInventoryElement(this.gameObject);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            UIManager.instance.characterUI.itemUI.OpenInventoryInteraction(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.StopDragInventoryElement();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemFrame is null) return;
        _itemFrame.SetActive(true);
        UIManager.instance.characterUI.itemUI.OpenInventoryTooltip(item, GetComponent<RectTransform>().position);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_itemFrame is null) return;
        _itemFrame.SetActive(false);
        UIManager.instance.characterUI.itemUI.ClosePopupWindow();
    }
}
