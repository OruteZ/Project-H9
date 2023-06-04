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
    
    public override void Attack(Unit target, out bool isCritical)
    {
        Debug.Log("Weapon attack Call" + " : " + weaponName);
        
        isCritical = Random.value < unitStat.criticalChance;
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
        return Mathf.RoundToInt(baseDamage + unitStat.revolverAdditionalDamage);
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = baseDamage + unitStat.revolverAdditionalDamage;
        dmg += dmg * unitStat.revolverCriticalDamage;

        return Mathf.RoundToInt(dmg);
    }

    public override float GetHitRate(Unit target)
    {
        int range = baseRange + unitStat.revolverAdditionalRange;
        int distance = Hex.Distance(unit.position, target.position);

        float hitRate = unitStat.concentration * (100 - distance * GetDistancePenalty() *
            (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1)
            ) * 0.01f;

        hitRate = Mathf.Round(10 * hitRate) * 0.1f;
        hitRate = Mathf.Clamp(hitRate, 0, 100);
        
        #if UNITY_EDITOR
        Debug.Log("Hit rate = " + hitRate);
        #endif
        
        return hitRate * 0.01f;
    }

    private void NonCriticalAttack(Unit target)
    {
        int damage = GetFinalDamage();
        target.GetDamage(damage);

        onSuccessAttack.Invoke(target, damage);
    }

    private void CriticalAttack(Unit target)
    {
        int damage = GetFinalCriticalDamage();
        target.GetDamage(damage);
        
        onSuccessAttack.Invoke(target, damage);
        onCriticalAttack.Invoke(target, damage);
    }
}
