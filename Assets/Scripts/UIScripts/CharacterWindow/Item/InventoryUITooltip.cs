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

        gameObject.SetActive(true);
    }
    private void SetInventoryTooltipText(IItem item)
    {

        ItemData iData = item.GetData();
        _itemNameText.GetComponent<TextMeshProUGUI>().text = iData.nameIdx.ToString();
        string description = "";
        if (item is WeaponItem)
        {
            ItemData wData = GameManager.instance.itemDatabase.GetItemData(GameManager.instance.playerWeaponIndex);
            string weaponTypeText = wData.itemType.ToString();
            string weaponDamageText = wData.weaponDamage.ToString() + " Damage";
            string weaponRangeText = wData.itemRange.ToString() + " Range";
            string weaponEffect = "Effect: " + iData.nameIdx.ToString();
            description = weaponTypeText + "\n" + weaponDamageText + "\n" + weaponRangeText + "\n\n" + weaponEffect;
        }

        _itemDescriptionText.GetComponent<TextMeshProUGUI>().text = description;
        _itemDescriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        GetComponent<ContentSizeFitter>().SetLayoutVertical();
    }
    public override void CloseUI()
    {
        _item = null;
        gameObject.SetActive(false);
    }
}
