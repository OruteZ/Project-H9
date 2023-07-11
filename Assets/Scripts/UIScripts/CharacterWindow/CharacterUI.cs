using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterUI : UISystem
{
    public CharacterStatUI _characterStatUI { get; private set; }
    public LearnedSkillUI _learnedSkillUI { get; private set; }
    public ItemListUI _itemListUI { get; private set; }

    [SerializeField] private GameObject _moneyText;

    // Start is called before the first frame update
    void Start()
    {
        _characterStatUI = GetComponent<CharacterStatUI>();
        _learnedSkillUI = GetComponent<LearnedSkillUI>();
        _itemListUI = GetComponent<ItemListUI>();
        SetMoneyText();
    }
    public override void OpenUI()
    {
        _characterStatUI.OpenUI();
        _learnedSkillUI.OpenUI();
        _itemListUI.OpenUI();
    }
    public override void CloseUI()
    {
        _characterStatUI.CloseUI();
        _learnedSkillUI.CloseUI();
        _itemListUI.CloseUI();
    }

    public void SetMoneyText()
    {
        string moneyText = ItemManager.instance.money.ToString();
        _moneyText.GetComponent<TextMeshProUGUI>().text = "Money: " + moneyText + "$";
    }

    public override void ClosePopupWindow()
    {
        UIManager.instance.previousLayer = 2;
        _itemListUI.CloseItemUseWindow();
    }
}
