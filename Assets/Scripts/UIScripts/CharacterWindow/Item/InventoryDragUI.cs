using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryDragUI : InventoryUIElement, IPointerUpHandler
{
    private bool _isDragging = false;
    private void Start()
    {
        _itemFrame = null;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (_isDragging)
        {
            Vector2 currentPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.position = currentPos;
        }
    }
    public new void SetInventoryUIElement(Item item) 
    {
        base.SetInventoryUIElement(item);
        _itemCountText.SetActive(false);
        GetComponent<Image>().color = UICustomColor.TransparentColor;
    }
    public void StartDragging()
    {
        gameObject.SetActive(true);
        _isDragging = true;
    }
    public void StopDragging()
    {
        gameObject.SetActive(false);
        _isDragging = false;
    }
}
