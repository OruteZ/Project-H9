using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryInteractionUI : UIElement
{
    [SerializeField] private GameObject _useBtn;
    [SerializeField] private GameObject _sellBtn;
    [SerializeField] private GameObject _removeBtn;
    public bool isEquipable { get; private set; }

    private void Awake()
    {
        isEquipable = true;
        CloseUI();
    }

    public void SetInventoryInteractionUI(Item item, Vector3 pos) 
    {
        if (item is null) 
        {
            CloseUI();
            return;
        }

        _useBtn.SetActive(true);
        switch (item.GetData().itemType)
        {
            case ItemType.Revolver:
            case ItemType.Repeater:
            case ItemType.Shotgun:
                {
                    isEquipable = true;
                    _useBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[40];
                    break;
                }
            case ItemType.Heal:
            case ItemType.Damage:
            case ItemType.Cleanse:
            case ItemType.Buff:
            case ItemType.Debuff:
                {
                    isEquipable = false;
                    _useBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[39];
                    if (GameManager.instance.CompareState(GameState.WORLD) && item.GetData().itemType != ItemType.Heal)
                    {
                        _useBtn.SetActive(false);
                    }
                    break;
                }
            default: 
                {
                    break;
                }
        }
        _sellBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[41] + "(" + item.GetData().itemPrice + ")";
        _sellBtn.SetActive(UIManager.instance.gameSystemUI.townUI.IsAmmuntionOpen());

        _removeBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[42];

        GetComponent<RectTransform>().position = pos;
        UIManager.instance.SetUILayer(3);
        OpenUI();
    }
    public override void CloseUI()
    {
        //UIManager.instance.SetUILayer(2);
        base.CloseUI();
    }
}
