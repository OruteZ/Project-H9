using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 캐릭터 정보 창의 여러 기능을 묶어서 관리하는 클래스
/// </summary>
public class CharacterUI : UISystem
{
    /// <summary>
    /// 캐릭터의 스텟 및 무기 스텟을 표시하는 기능
    /// </summary>
    public CharacterStatUI characterStatUI { get; private set; }
    /// <summary>
    /// 캐릭터가 습득한 스킬을 캐릭터 창에 표시하는 기능
    /// </summary>
    public LearnedSkillUI learnedSkillUI { get; private set; }
    /// <summary>
    /// 캐릭터의 인벤토리 및 아이템을 표시하는 기능
    /// </summary>
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
    public override void ClosePopupWindow()
    {
        itemListUI.ClosePopupWindow();
    }

    /// <summary>
    /// 플레이어의 소지금을 UI로 표시합니다.
    /// 아이템 매니저에서 소지금이 변동할 때 작동합니다.
    /// </summary>
    public void SetMoneyText()
    {
        string moneyText = ItemManager.instance.money.ToString();
        _moneyText.GetComponent<TextMeshProUGUI>().text = "Money: " + moneyText + "$";
    }
}
