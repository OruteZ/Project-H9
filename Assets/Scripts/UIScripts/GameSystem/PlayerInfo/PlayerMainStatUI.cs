using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainStatUI : UIElement
{
    [SerializeField] private GameObject AtkText;
    [SerializeField] private GameObject SightText;
    [SerializeField] private GameObject SpeedText;
    [SerializeField] private GameObject AddHRText;
    [SerializeField] private GameObject CritChanceText;
    [SerializeField] private GameObject CritDmgText;
    
    public void SetMainStatUI() 
    {
        UnitStat stat = GameManager.instance.playerStat;
        int dmg = 1;
        float criDmg = 100;
        WeaponType wT = FieldSystem.unitSystem.GetPlayer().weapon.GetWeaponType();
        if (wT == WeaponType.Revolver) 
        {
            dmg += stat.revolverAdditionalDamage;
            criDmg += stat.revolverCriticalDamage;
        }
        else if (wT == WeaponType.Repeater)
        {
            dmg += stat.repeaterAdditionalDamage;
            criDmg += stat.repeaterCriticalDamage;
        }
        else if (wT == WeaponType.Shotgun)
        {
            dmg += stat.shotgunAdditionalDamage;
            criDmg += stat.shotgunCriticalDamage;
        }
        AtkText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Damage", dmg.ToString());   //여기 정확히 뭘 기재해야 하나? 
        SightText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Sight Range", stat.sightRange.ToString());
        SpeedText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Speed", stat.speed.ToString());
        AddHRText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Additional Hit Rate", stat.additionalHitRate.ToString() + "%");
        CritChanceText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Critical Rate", stat.criticalChance.ToString() + "%");
        CritDmgText.GetComponent<PlayerMainStatElement>().SetPlayerMainStatUI("Critical Damage", (criDmg / 100.0f).ToString() + "x");
    }
}
