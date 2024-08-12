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

        Player player = FieldSystem.unitSystem.GetPlayer();
        if (item is WeaponItem)
        {
            if (player.currentActionPoint < Inventory.WEAPON_COST) return;
            GameManager.instance.playerInventory.EquipItem(item.GetData().itemType, UIManager.instance.combatUI.combatActionUI.GetInventoryUIIndex(gameObject));
        }
        else
        {
            if (player.currentActionPoint < Inventory.ITEM_COST) return;
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
