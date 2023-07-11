using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatUI : UISystem
{
    //Character Stat
    [Header("Character Stat UI")]
    [SerializeField] private GameObject _characterStatText;
    [SerializeField] private GameObject _weaponStatText;
    public Image _characterImage;
    public Image _weaponImage;

    public override void OpenUI()
    {
        SetStatText();
    }
    public override void CloseUI()
    {
    }
    private void SetStatText()
    {
        UnitStat playerStat = GameManager.instance.playerStat;
        Weapon weapon = GameManager.instance.playerWeapon;
        WeaponType weaponType;
        //in test development
        if (weapon == null) { weaponType = WeaponType.Repeater; }
        else { weaponType = weapon.GetWeaponType(); }

        SetCharacterStatText(playerStat);
        SetWeaponStatText(playerStat, weaponType);
    }
    private void SetCharacterStatText(UnitStat playerStat)
    {
        string text = playerStat.maxHp.ToString() + '\n' +
                      playerStat.concentration.ToString() + '\n' +
                      playerStat.sightRange.ToString() + '\n' +
                      playerStat.speed.ToString() + '\n' +
                      playerStat.actionPoint.ToString() + '\n' +
                      playerStat.additionalHitRate.ToString() + '\n' +
                      playerStat.criticalChance.ToString();

        _characterStatText.GetComponent<TextMeshProUGUI>().text = text;
    }
    private void SetWeaponStatText(UnitStat playerStat, WeaponType weaponType)
    {
        string text = "";
        switch (weaponType)
        {
            case WeaponType.Repeater:
                {
                    text += playerStat.repeaterAdditionalDamage.ToString() + '\n' +
                            playerStat.repeaterAdditionalRange.ToString() + '\n' +
                            playerStat.repeaterCriticalDamage.ToString();
                    break;
                }
            case WeaponType.Revolver:
                {
                    text += playerStat.revolverAdditionalDamage.ToString() + '\n' +
                            playerStat.revolverAdditionalRange.ToString() + '\n' +
                            playerStat.revolverCriticalDamage.ToString();
                    break;
                }
            case WeaponType.Shotgun:
                {
                    text += playerStat.shotgunAdditionalDamage.ToString() + '\n' +
                            playerStat.shotgunAdditionalRange.ToString() + '\n' +
                            playerStat.shotgunCriticalDamage.ToString();
                    break;
                }
        }

        _weaponStatText.GetComponent<TextMeshProUGUI>().text = text;
    }

}
