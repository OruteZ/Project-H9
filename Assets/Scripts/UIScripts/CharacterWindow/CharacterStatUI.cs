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
    [SerializeField] private GameObject _weaponStatTextName;
    [SerializeField] private GameObject _weaponStatTextContents;

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
        string addDmg = playerStat.revolverAdditionalDamage.ToString() + " / " + playerStat.repeaterAdditionalDamage.ToString() + " / " + playerStat.shotgunAdditionalDamage.ToString();
        string addRng = playerStat.revolverAdditionalRange.ToString() + " / " + playerStat.repeaterAdditionalRange.ToString() + " / " + playerStat.shotgunAdditionalRange.ToString();
        string criDmg = playerStat.revolverCriticalDamage.ToString() + " / " + playerStat.repeaterCriticalDamage.ToString() + " / " + playerStat.shotgunCriticalDamage.ToString();

        string text = playerStat.maxHp.ToString() + '\n' +
                      playerStat.concentration.ToString() + '\n' +
                      playerStat.sightRange.ToString() + '\n' +
                      playerStat.speed.ToString() + '\n' +
                      playerStat.actionPoint.ToString() + '\n' +
                      playerStat.additionalHitRate.ToString() + '%' + '\n' +
                      ((int)(playerStat.criticalChance * 100)).ToString() + '%' + '\n' +
                      "" + '\n' + 
                      addDmg + '\n' +
                      addRng + '\n' +
                      criDmg;

        _characterStatText.GetComponent<TextMeshProUGUI>().text = text;
    }
    private void SetWeaponStatText(Weapon weapon)
    {
        string nameText = "Name:\nAmmo:\nDamage:\nRange:\n";
        string contentsText = /*weapon.weaponName +*/'\n' +
                                weapon.maxAmmo.ToString() + '\n' +
                                weapon.weaponDamage.ToString() + '\n' +
                                weapon.weaponRange.ToString() + '\n';

        string additionalNameText = "";
        string additionalContentsText = "";

        float hitRate = weapon.hitRate;
        float criChance = weapon.criticalChance;
        float criDamage = weapon.criticalDamage;
        if (hitRate != 0)
        {
            additionalNameText += "Additional Hit Rate:\n";
            additionalContentsText += hitRate.ToString() + '%' + '\n';
        }
        if (criChance != 0)
        {
            additionalNameText += "Critical Chance:\n";
            additionalContentsText += criChance.ToString() + '%' + '\n';
        }
        if (criDamage != 0)
        {
            additionalNameText += "Critical Damage:\n";
            additionalContentsText += criDamage.ToString() + '\n';
        }

        nameText += additionalNameText;
        contentsText += additionalContentsText;
        _weaponStatTextName.GetComponent<TextMeshProUGUI>().text = nameText;
        _weaponStatTextContents.GetComponent<TextMeshProUGUI>().text = contentsText;
    }
}
