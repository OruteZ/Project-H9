using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainStatUI : UIElement
{
    [SerializeField] private GameObject _atkText;
    [SerializeField] private GameObject _sightText;
    [SerializeField] private GameObject _speedText;
    [SerializeField] private GameObject _addHRText;
    [SerializeField] private GameObject _critChanceText;
    [SerializeField] private GameObject _critDmgText;

    [SerializeField] private GameObject _statTooltip;

    private void Start()
    {
        HideMainStatTooltip();
    }
    public void SetMainStatUI() 
    {
        UnitStat stat = GameManager.instance.playerStat;
        if (FieldSystem.unitSystem.GetPlayer() == null) return;
        Weapon weapon = FieldSystem.unitSystem.GetPlayer().weapon;
        int dmg = weapon.weaponDamage;
        float criDmg = 100;
        ItemType wT = weapon.GetWeaponType();
        if (wT == ItemType.Revolver) 
        {
            dmg += stat.revolverAdditionalDamage;
            criDmg += stat.revolverCriticalDamage;
        }
        else if (wT == ItemType.Repeater)
        {
            dmg += stat.repeaterAdditionalDamage;
            criDmg += stat.repeaterCriticalDamage;
        }
        else if (wT == ItemType.Shotgun)
        {
            dmg += stat.shotgunAdditionalDamage;
            criDmg += stat.shotgunCriticalDamage;
        }
        _atkText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Damage", dmg.ToString());
        _sightText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Sight Range", stat.sightRange.ToString());
        _speedText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Speed", stat.speed.ToString());
        _addHRText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Bonus Hit Rate", stat.additionalHitRate.ToString() + "%");
        _critChanceText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Critical Chance", stat.criticalChance.ToString() + "%");
        _critDmgText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Critical Damage", (criDmg / 100.0f).ToString() + "x");
    }
    public void ShowMainStatTooltip(GameObject textObj) 
    {
        _statTooltip.GetComponent<PlayerMainStatTooltip>().SetPlayerMainStatTooltip(textObj);
    }
    public void HideMainStatTooltip() 
    {
        _statTooltip.GetComponent<PlayerMainStatTooltip>().CloseUI();
    }
}
