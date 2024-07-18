using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//Draggable items
public class InventoryUIElement : InventoryUIBaseElement, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemUI _itemUI;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (item == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _itemUI.StartDragInventoryElement(this.gameObject);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item is EtcItem) return;
            //if (GameManager.instance.CompareState(GameState.World)) return;

            _itemUI.OpenInventoryInteraction(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _itemUI.StopDragInventoryElement();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemFrame == null) return;
        _itemFrame.SetActive(true);
        _itemUI.OpenInventoryTooltip(gameObject, GetComponent<RectTransform>().position);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_itemFrame == null) return;
        _itemFrame.SetActive(false);
        _itemUI.ClosePopupWindow();
    }
}
