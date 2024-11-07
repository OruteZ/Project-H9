using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    private Vector3Int _targetHex;

    public Shotgun(WeaponData data) : base(data)
    {
    }

    public override ItemType GetWeaponType() => ItemType.Shotgun;
    public override float CalculateDistancePenalty(IDamageable target) { return 0; }
    public override int GetRange()
    {
        return weaponRange + magazine.GetNextBullet().data.range + UnitStat.shotgunAdditionalDamage;
    }

    public override float GetFinalCriticalRate()
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        return  UnitStat.criticalChance + criticalChance + magazine.GetNextBullet().data.criticalChance;
    }

    public override int GetFinalDamage()
    {
        float baseDamage = (weaponDamage + magazine.GetNextBullet().data.damage + UnitStat.shotgunAdditionalDamage);

        int range = GetRange();
        int distance = Hex.Distance(unit.hexPosition, _targetHex);

        int value = range - distance;
        int damage = Mathf.RoundToInt(baseDamage * Mathf.Pow(2, value));
        
        return damage;
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = GetFinalDamage();
        dmg += dmg * ((UnitStat.shotgunCriticalDamage + criticalDamage + +magazine.GetNextBullet().data.criticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        int range = GetRange();
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());
        
        _targetHex = target.GetHex();

        float finalHitRate = distance <= range ? 100 : 0;

        return finalHitRate;
    }
}
