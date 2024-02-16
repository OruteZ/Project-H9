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
    /// 캐릭터의 패시브 스킬을 표시하는 기능
    /// </summary>
    public PassiveSkillListUI passiveSkillListUI { get; private set; }
    /// <summary>
    /// 캐릭터의 인벤토리 및 아이템을 표시하는 기능
    /// </summary>
    public ItemUI itemUI { get; private set; }
    public EquipmentUI equipmentUI { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        characterStatUI = GetComponent<CharacterStatUI>();
        passiveSkillListUI = GetComponent<PassiveSkillListUI>();
        itemUI = GetComponent<ItemUI>();
        equipmentUI = GetComponent<EquipmentUI>();
        SetMoneyText();

        uiSubsystems.Add(characterStatUI);
        uiSubsystems.Add(passiveSkillListUI);
        uiSubsystems.Add(itemUI);
        uiSubsystems.Add(equipmentUI);
    }

    public override void ClosePopupWindow()
    {
        itemUI.ClosePopupWindow();
    }

    /// <summary>
    /// 플레이어의 소지금을 UI로 표시합니다.
    /// </summary>
    public void SetMoneyText()
    {
        //string moneyText = ItemManager.instance.money.ToString();
        //_moneyText.GetComponent<TextMeshProUGUI>().text = "Money: " + moneyText + "$";
    }
}
