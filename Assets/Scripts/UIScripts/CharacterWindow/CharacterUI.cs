using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterUI : UISystem
{
    public CharacterStatUI characterStatUI { get; private set; }
    public LearnedSkillUI learnedSkillUI { get; private set; }
    public ItemListUI itemListUI { get; private set; }

    [SerializeField] private GameObject _moneyText;

    // Start is called before the first frame update
    void Start()
    {
        characterStatUI = GetComponent<CharacterStatUI>();
        learnedSkillUI = GetComponent<LearnedSkillUI>();
        itemListUI = GetComponent<ItemListUI>();
        SetMoneyText();
    }
    public override void OpenUI()
    {
        base.OpenUI();
        characterStatUI.OpenUI();
        learnedSkillUI.OpenUI();
        itemListUI.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        characterStatUI.CloseUI();
        learnedSkillUI.CloseUI();
        itemListUI.CloseUI();
    }

    public void SetMoneyText()
    {
        string moneyText = ItemManager.instance.money.ToString();
        _moneyText.GetComponent<TextMeshProUGUI>().text = "Money: " + moneyText + "$";
    }

    public override void ClosePopupWindow()
    {
        itemListUI.ClosePopupWindow();
    }
}
