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

    private ItemData _data = null;

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.characterUI.itemUI.ClosePopupWindow();
    }

    public void SetInventoryUITooltip(ItemData data, Vector3 pos)
    {
        if (_data == data && GetComponent<RectTransform>().position == pos) return;

        if (data is null)
        {
            CloseUI();
            return;
        }
        _data = data;
        GetComponent<RectTransform>().position = pos;

        UIManager.instance.SetUILayer(3);

        SetInventoryTooltipText(data);

        if (GameManager.instance.CompareState(GameState.Combat))
        {
            if (data.itemType == ItemType.Revolver || data.itemType == ItemType.Repeater || data.itemType == ItemType.Shotgun)
            {
                _itemCostUI.GetComponent<TooltipCostUI>().SetTooltipCostUI(Inventory.WEAPON_COST, 0, false);
            }
            else
            {
                _itemCostUI.GetComponent<TooltipCostUI>().SetTooltipCostUI(Inventory.ITEM_COST, 0, false);
            }
        }
        else
        {
            _itemCostUI.GetComponent<TooltipCostUI>().CloseUI();
        }

        OpenUI();
        StartCoroutine(GetCursorDominion());
    }
    private void SetInventoryTooltipText(ItemData data)
    {
        ItemScript script = GameManager.instance.itemDatabase.GetItemScript(data.nameIdx);
        _itemNameText.GetComponent<TextMeshProUGUI>().text = script.GetName();
        string description = data.GetInventoryTooltipContents();
        _itemDescriptionText.GetComponent<TextMeshProUGUI>().text = description;
    }
    public override void CloseUI()
    {
        _data = null;
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
