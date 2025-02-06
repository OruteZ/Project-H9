using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Revolver : Weapon
{
    public Revolver(WeaponData data) : base(data)
    {
    }

    public override ItemType GetWeaponType() => ItemType.Revolver;
    public override float CalculateDistancePenalty(IDamageable target)
    {
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());
        int range = GetRange();

        return (100 - 
                distance * DISTANCE_PENALTY_SCALER * 
                (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1)
                );
    }
    public override int GetRange()
    {
        return weaponRange + magazine.GetNextBullet().data.range + UnitStat.revolverAdditionalRange;
    }

    public override float GetFinalCriticalRate()
    { 
        return criticalChance + magazine.GetNextBullet().data.criticalChance + UnitStat.criticalChance;
    }

    public override int GetFinalDamage()
    {
        return Mathf.RoundToInt(weaponDamage + magazine.GetNextBullet().data.damage + UnitStat.revolverAdditionalDamage);
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = weaponDamage + magazine.GetNextBullet().data.damage + UnitStat.revolverAdditionalDamage;
        dmg += dmg * ((criticalDamage + magazine.GetNextBullet().data.criticalDamage + UnitStat.revolverCriticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        float finalHitRate = (
            hitRate + 
            magazine.GetNextBullet().data.hitRate + 
            UnitStat.concentration * 
            CalculateDistancePenalty(target)
            ) * 0.01f;
        

        finalHitRate = Mathf.Round(10 * finalHitRate) * 0.1f;
        finalHitRate = Mathf.Clamp(finalHitRate, 0, 100);
        finalHitRate += target.GetHitRateModifier(unit);

        if (unit.GetSelectedAction() is FanningAction f)
        {
            finalHitRate += f.GetHitRateModifier();
        }

        return finalHitRate;
    }
}
