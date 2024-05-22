using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIEnemyWeapon : InventoryUIBaseElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EnemyInfoUI _infoUI;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _infoUI.OpenInventoryTooltip(gameObject, GetComponent<RectTransform>().position);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _infoUI.CloseInventoryTooltip();
    }
}
