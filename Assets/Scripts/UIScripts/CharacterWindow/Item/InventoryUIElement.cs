using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUIElement : UIElement, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject _itemIcon;
    [SerializeField] protected GameObject _itemCountText;

    public Item item { get; private set; }
    public int idx;
    private void Awake()
    {
        item = null;
    }

    public void SetInventoryUIElement(Item item) 
    {
        if (item is null) 
        {
            Debug.Log("InventoryUI: item is null");
            return;
        }
        this.item = item;
        idx = item.GetData().id;
        Texture2D texture = item.GetData().icon;
        _itemIcon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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
        if (item is null) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.instance.characterUI.itemUI.StartDragInventoryElement(this.gameObject);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            UIManager.instance.characterUI.itemUI.OpenInventoryTooltip(item, GetComponent<RectTransform>().position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.StopDragInventoryElement();
    }
}
