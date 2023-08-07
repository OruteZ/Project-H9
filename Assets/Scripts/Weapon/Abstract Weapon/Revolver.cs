using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Revolver : Weapon
{
    public override WeaponType GetWeaponType() => WeaponType.Revolver;
    public override float GetDistancePenalty() => 6;
    public override int GetRange()
    {
        return weaponRange + unitStat.revolverAdditionalRange;
    }

    public override void Attack(Unit target, out bool isCritical)
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        isCritical = Random.value < unitStat.criticalChance + criticalChance;
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
        dmg += dmg * (unitStat.revolverCriticalDamage + criticalDamage);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(Unit target)
    {
        int range = weaponRange + unitStat.revolverAdditionalRange;
        int distance = Hex.Distance(unit.hexPosition, target.hexPosition);

        float hitRate = this.hitRate + unitStat.concentration * (100 - distance * GetDistancePenalty() *
            (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1)
            ) * 0.01f;

        hitRate = Mathf.Round(10 * hitRate) * 0.1f;
        hitRate = Mathf.Clamp(hitRate, 0, 100);

        // #if UNITY_EDITOR
        // Debug.Log("Hit rate = " + hitRate);
        // #endif

        UIManager.instance.debugUI.SetDebugUI(hitRate, unit, target, distance, weaponRange, unitStat.revolverAdditionalRange, REVOLVER_OVER_RANGE_PENALTY);
        return hitRate * 0.01f;
    }

    private void NonCriticalAttack(Unit target)
    {
        int damage = GetFinalDamage();
        target.GetDamage(damage);

        unit.onSuccessAttack.Invoke(target, damage);
    }

    private void CriticalAttack(Unit target)
    {
        int damage = GetFinalCriticalDamage();
        target.GetDamage(damage);
        
        unit.onSuccessAttack.Invoke(target, damage);
        unit.onCriticalAttack.Invoke(target, damage);
    }
}
