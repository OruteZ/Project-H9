using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryInteractionUI : UIElement
{
    [SerializeField] private GameObject _useBtn;
    public bool isEqipable { get; private set; }

    private void Awake()
    {
        isEqipable = true;
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
                    isEqipable = true;
                    _useBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Eqip";
                    break;
                }
            case ItemType.Heal:
            case ItemType.Damage:
            case ItemType.Cleanse:
            case ItemType.Buff:
            case ItemType.Debuff:
                {
                    isEqipable = false;
                    _useBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Use";
                    break;
                }
            default: 
                {
                    break;
                }
        }

        GetComponent<RectTransform>().position = pos;
        UIManager.instance.currentLayer = 3;
        UIManager.instance.SetUILayer();
        OpenUI();
    }
}
