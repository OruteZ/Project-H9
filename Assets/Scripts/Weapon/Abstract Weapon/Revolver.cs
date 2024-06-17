using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Revolver : Weapon
{
    public override ItemType GetWeaponType() => ItemType.Revolver;
    public override float GetDistancePenalty() => 5;
    public override int GetRange()
    {
        return weaponRange + unitStat.revolverAdditionalRange;
    }

    public override void Attack(IDamageable target, out bool isCritical)
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        isCritical = Random.value * 100 < unitStat.criticalChance + criticalChance 
                     || GetCurrentBullet() == AmmoType.GoldBullet;

        if (isCritical)
        {
            CriticalAttack(target);
        }
        else
        {
            NonCriticalAttack(target);
        }
    }

    public override int GetFinalDamage()
    {
        return Mathf.RoundToInt(weaponDamage + unitStat.revolverAdditionalDamage);
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = weaponDamage + unitStat.revolverAdditionalDamage;
        dmg += dmg * ((unitStat.revolverCriticalDamage + criticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        int range = weaponRange + unitStat.revolverAdditionalRange;
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());

        var finalHitRate = (hitRate + unitStat.concentration * (100 - distance * GetDistancePenalty() *
            (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1)
            )) * 0.01f;

        finalHitRate = Mathf.Round(10 * finalHitRate) * 0.1f;
        finalHitRate = Mathf.Clamp(finalHitRate, 0, 100);
        finalHitRate += target.GetHitRateModifier();

        #if UNITY_EDITOR
        //Debug.Log("Hit rate = " + finalHitRate);
        #endif

        UIManager.instance.debugUI.SetDebugUI
            (finalHitRate, unit, (Unit)target, distance, weaponRange,
                unitStat.revolverAdditionalRange,
                GetDistancePenalty() *
                (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1));
        
        return finalHitRate;
    }

    private void NonCriticalAttack(IDamageable target)
    {
        int damage = GetFinalDamage();
        target.TakeDamage(damage, unit);
    }

    private void CriticalAttack(IDamageable target)
    {
        int damage = GetFinalCriticalDamage();
        target.TakeDamage(damage, unit, Damage.Type.Critical);
    }
}
