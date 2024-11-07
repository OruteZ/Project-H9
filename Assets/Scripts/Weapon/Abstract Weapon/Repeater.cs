using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : Weapon
{
    public Repeater(WeaponData data) : base(data)
    {
    }

    public override ItemType GetWeaponType() => ItemType.Repeater;
    public override float CalculateDistancePenalty(IDamageable target)
    {
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());
        int range = GetRange();

        return (100 - distance * (IsSweetSpot(distance) ? 0 : DISTANCE_PENALTY_SCALER) * (distance > range ? REPEATER_OVER_RANGE_PENALTY : 1));
    }
    public override int GetRange()
    {
        return weaponRange + magazine.GetNextBullet().data.range + UnitStat.repeaterAdditionalRange;
    }

    public override float GetFinalCriticalRate()
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        return UnitStat.criticalChance + criticalChance + magazine.GetNextBullet().data.criticalChance;
    }

    public override int GetFinalDamage()
    {
        return Mathf.RoundToInt(weaponDamage + magazine.GetNextBullet().data.damage + UnitStat.revolverAdditionalDamage);
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = weaponDamage + magazine.GetNextBullet().data.damage + UnitStat.revolverAdditionalDamage;
        dmg += dmg * ((UnitStat.revolverCriticalDamage + criticalDamage + magazine.GetNextBullet().data.criticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        int range = GetRange();
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());

        float finalHitRate = (
            hitRate + 
            magazine.GetNextBullet().data.hitRate + 
            UnitStat.concentration * CalculateDistancePenalty(target)
            ) * 0.01f;

        finalHitRate = Mathf.Round(10 * finalHitRate) * 0.1f;
        finalHitRate = Mathf.Clamp(finalHitRate, 0, 100);

        return finalHitRate;
    }
    
    public int GetSweetSpot()
    {
        //todo : get sweet spot from data 
        return GetRange() - 1;
    }
    
    private bool IsSweetSpot(int distance)
    {
        return distance == GetSweetSpot();
    }
}
