using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 캐릭터 정보 창에서 캐릭터의 스텟 및 장비하고 있는 무기의 스텟 정보를 표시하는 기능을 구현한 클래스
/// </summary>
public class CharacterStatUI : UISystem
{
    //Character Stat
    [Header("Character Stat UI")]
    [SerializeField] private GameObject _characterStatText;
    [SerializeField] private GameObject _characterLevelText;
    [SerializeField] private GameObject _weaponStatText1Contents;
    [SerializeField] private GameObject _weaponStatText2Contents;
    [SerializeField] private GameObject _weaponStatText3Name;
    [SerializeField] private GameObject _weaponStatText3Contents;

    private Image _characterImage;
    private Image _weaponImage;

    public override void OpenUI()
    {
        base.OpenUI();

        SetStatText();
    }
    public override void CloseUI()
    {
        base.CloseUI();
    }
    private void SetStatText()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();

        SetCharacterStatText(player);
        SetWeaponStatText(player.weapon);
    }
    private void SetCharacterStatText(Player player)
    {
        UnitStat playerStat = player.GetStat();
        string text = playerStat.maxHp.ToString() + '\n' +
                      playerStat.concentration.ToString() + '\n' +
                      playerStat.sightRange.ToString() + '\n' +
                      playerStat.speed.ToString() + '\n' +
                      playerStat.actionPoint.ToString() + '\n' +
                      playerStat.additionalHitRate.ToString() + '\n' +
                      playerStat.criticalChance.ToString();

        _characterStatText.GetComponent<TextMeshProUGUI>().text = text;
    }
    private void SetWeaponStatText(Weapon weapon)
    {
        string text1 = /*weapon.weaponName +*/'\n' +
                       weapon.weaponDamage.ToString();
        _weaponStatText1Contents.GetComponent<TextMeshProUGUI>().text = text1;

        string text2 = weapon.currentAmmo.ToString() + " / " + weapon.maxAmmo.ToString() + '\n' +
                       weapon.weaponRange.ToString();
        _weaponStatText2Contents.GetComponent<TextMeshProUGUI>().text = text2;

        string text3 = "", text4 = "";
        float hitRate = weapon.hitRate;
        float criChance = weapon.criticalChance;
        float criDamage = weapon.criticalDamage;
        if (hitRate != 0)
        {
            text3 += "Hit Rate:" + '\n';
            text4 += hitRate.ToString() + '\n';
        }
        if (criChance != 0)
        {
            text3 += "Critical Chance:" + '\n';
            text4 += criChance.ToString() + '\n';
        }
        if (criDamage != 0)
        {
            text3 += "Critical Damage:" + '\n';
            text4 += criDamage.ToString() + '\n';
        }
        _weaponStatText3Name.GetComponent<TextMeshProUGUI>().text = text3;
        _weaponStatText3Contents.GetComponent<TextMeshProUGUI>().text = text4;
    }
}
