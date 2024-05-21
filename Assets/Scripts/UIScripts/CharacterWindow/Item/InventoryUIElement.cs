using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//Draggable items
public class InventoryUIElement : InventoryUIBaseElement, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.instance.characterUI.itemUI.StartDragInventoryElement(this.gameObject);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item is EtcItem) return;
            //if (GameManager.instance.CompareState(GameState.World)) return;
            UIManager.instance.characterUI.itemUI.OpenInventoryInteraction(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.StopDragInventoryElement();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemFrame == null) return;
        _itemFrame.SetActive(true);
        UIManager.instance.characterUI.itemUI.OpenInventoryTooltip(gameObject, GetComponent<RectTransform>().position);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_itemFrame == null) return;
        _itemFrame.SetActive(false);
        UIManager.instance.characterUI.itemUI.ClosePopupWindow();
    }
}
