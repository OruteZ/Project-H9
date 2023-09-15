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

    [SerializeField] private GameObject _characterStatTexts;
    [SerializeField] private GameObject _weaponStatTexts;

    public GameObject _characterStatTooltip;

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

        SetCharacterStatText(player.GetStat());
        SetWeaponStatText(player.weapon);
    }
    private void SetCharacterStatText(UnitStat playerStat)
    {
        string addDmg = playerStat.revolverAdditionalDamage.ToString() + " / " + playerStat.repeaterAdditionalDamage.ToString() + " / " + playerStat.shotgunAdditionalDamage.ToString();
        string addRng = playerStat.revolverAdditionalRange.ToString() + " / " + playerStat.repeaterAdditionalRange.ToString() + " / " + playerStat.shotgunAdditionalRange.ToString();
        string criDmg = playerStat.revolverCriticalDamage.ToString() + " / " + playerStat.repeaterCriticalDamage.ToString() + " / " + playerStat.shotgunCriticalDamage.ToString();

        string[,] texts =
        {
            {"HP:",                     playerStat.maxHp.ToString() },
            {"Concentration:",          playerStat.concentration.ToString() },
            {"Sight Range:",            playerStat.sightRange.ToString() },
            {"Speed:",                  playerStat.speed.ToString() },
            {"Action Point:",           playerStat.actionPoint.ToString() },
            {"Additional Hit Rate:",    playerStat.additionalHitRate.ToString() + '%' },
            {"Critical Chance:",        ((int)(playerStat.criticalChance * 100)).ToString() + '%' },
            {"", "" },
            {"Additional Damage:",      addDmg },
            {"Additional Range:",       addRng },
            {"Critical Damage:",        criDmg },
            {"", "" },
        };

        for (int i = 0; i < _characterStatTexts.transform.childCount; i++) 
        {
            _characterStatTexts.transform.GetChild(i)
                .GetComponent<CharacterStatTextElement>().SetCharacterStatText(texts[i, 0], texts[i, 1]);
        }
    }
    private void SetWeaponStatText(Weapon weapon)
    {
        string hitRateText = "Additional Hit Rate:";
        string criChanceText = "Critical Chance:";
        string criDamageText = "Critical Damage:";
        string hitRateValue = weapon.hitRate.ToString() + '%';
        string criChanceValue = weapon.criticalChance.ToString() + '%';
        string criDamageValue = weapon.criticalDamage.ToString();
        if (weapon.hitRate == 0)
        {
            hitRateText = "";
            hitRateValue = "";
        }
        if (weapon.criticalChance == 0)
        {
            criChanceText = "";
            criChanceValue = "";
        }
        if (weapon.criticalDamage == 0)
        {
            criDamageText = "";
            criDamageValue = "";
        }

        string[,] texts =
        {
            {"Name:",                 /*weapon.weaponName +*/"" },
            {"Ammo:",                   weapon.maxAmmo.ToString() },
            {"Damage:",                 weapon.weaponDamage.ToString() },
            {"Range:",                  weapon.weaponRange.ToString() },
            {hitRateText,               hitRateValue },
            {criChanceText,             criChanceValue },
            {criDamageText,             criDamageValue }
        };

        for (int i = 0; i < _weaponStatTexts.transform.childCount; i++)
        {
            _weaponStatTexts.transform.GetChild(i)
                .GetComponent<CharacterStatTextElement>().SetCharacterStatText(texts[i, 0], texts[i, 1]);
        }
    }
}

//HP:
//Concentration:
//Sight Range:
//Speed:
//Action Point:
//Additional Hit Rate:
//Critical Chance:

//Additional Damage:
//Additional Range:
//Critical Damage:

//Name:
//Ammo:
//Damage:
//Range:
//Additional Hit Rate:
//Critical Chance:
//Critical Damage: