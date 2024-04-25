using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUICombatElement : InventoryUIBaseElement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
        
        if (item is EtcItem)
        {
            Debug.LogError("etc item trying to use");
            return;
        }

        if (item is WeaponItem)
        {
            GameManager.instance.playerInventory.EquipItem(item.GetData().itemType, UIManager.instance.combatUI.combatActionUI.GetInventoryUIIndex(gameObject));
        }
        else
        {
            GameManager.instance.playerInventory.UseItem(item.GetData().itemType, UIManager.instance.combatUI.combatActionUI.GetInventoryUIIndex(gameObject));
        }
        UIManager.instance.combatUI.combatActionUI.SeleteUsingItem();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemFrame == null) return;
        _itemFrame.SetActive(true);
        UIManager.instance.combatUI.combatActionUI.OpenInventoryTooltip(gameObject, GetComponent<RectTransform>().position);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_itemFrame == null) return;
        _itemFrame.SetActive(false);
        UIManager.instance.combatUI.combatActionUI.ClosePopupWindow();
    }
}
