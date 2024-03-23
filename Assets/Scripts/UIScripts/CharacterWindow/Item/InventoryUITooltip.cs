using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class InventoryUITooltip : UIElement,IPointerExitHandler
{
    [SerializeField] private GameObject _itemNameText;
    [SerializeField] private GameObject _itemDescriptionText;
    [SerializeField] private GameObject _itemCostUI;
    private const float UI_DOMINION_TIME = 0.3f;

    private IItem _item = null;

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.ClosePopupWindow();
    }

    public void SetInventoryUITooltip(IItem item, Vector3 pos)
    {
        if (_item == item && GetComponent<RectTransform>().position == pos) return;

        if (item is null)
        {
            CloseUI();
            return;
        }
        _item = item;
        GetComponent<RectTransform>().position = pos;

        UIManager.instance.previousLayer = 3;

        SetInventoryTooltipText(item);

        if (GameManager.instance.CompareState(GameState.Combat))
        {
            if (item is WeaponItem)
            {
                _itemCostUI.GetComponent<TooltipCostUI>().SetTooltipCostUI(Inventory.WEAPON_COST, 0);
            }
            else
            {
                _itemCostUI.GetComponent<TooltipCostUI>().SetTooltipCostUI(Inventory.ITEM_COST, 0);
            }
        }
        else 
        {
            _itemCostUI.GetComponent<TooltipCostUI>().CloseUI();
        }

        OpenUI();
        StartCoroutine(GetCursorDominion());
    }
    private void SetInventoryTooltipText(IItem item)
    {
        ItemScript script = GameManager.instance.itemDatabase.GetItemScript(item.GetData().nameIdx);
        _itemNameText.GetComponent<TextMeshProUGUI>().text = script.GetName();
        string description = item.GetInventoryTooltipContents();
        _itemDescriptionText.GetComponent<TextMeshProUGUI>().text = description;
    }
    public override void CloseUI()
    {
        _item = null;
        StopAllCoroutines();

        gameObject.SetActive(false);
    }

    private IEnumerator GetCursorDominion()
    {
        GetComponent<Image>().raycastTarget = false;
        yield return new WaitForSeconds(UI_DOMINION_TIME);

        GetComponent<Image>().raycastTarget = true;
        yield break;
    }
}
