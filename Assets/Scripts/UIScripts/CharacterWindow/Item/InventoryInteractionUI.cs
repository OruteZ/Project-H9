using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryInteractionUI : UIElement
{
    [SerializeField] private GameObject _useBtn;
    private bool _isEqipable = true;

    private void Awake()
    {
        CloseUI();
    }

    public void SetInventoryInteractionUI(Item item, Vector3 pos) 
    {
        if (item is null) 
        {
            CloseUI();
            return;
        }
        switch (item.GetData().itemType)
        {
            case ItemType.Revolver:
            case ItemType.Repeater:
            case ItemType.Shotgun:
                {
                    _isEqipable = true;
                    _useBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Eqip";
                    break;
                }
            case ItemType.Heal:
            case ItemType.Damage:
            case ItemType.Cleanse:
            case ItemType.Buff:
            case ItemType.Debuff:
                {
                    _isEqipable = false;
                    _useBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Use";
                    break;
                }
            default: 
                {
                    return;
                }
        }

        GetComponent<RectTransform>().position = pos;
        UIManager.instance.currentLayer = 3;
        UIManager.instance.SetUILayer();
        OpenUI();
    }

    public void ClickUseItemBtn() 
    {
        Debug.Log("1");
    }
    public void ClickSellItemBtn()
    {
        Debug.Log("2");
    }
    public void ClickRemoveItemBtn()
    {
        Debug.Log("3");
    }
}
