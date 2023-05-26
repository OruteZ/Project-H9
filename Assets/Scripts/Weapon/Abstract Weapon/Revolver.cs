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
    
    public override void Attack(Unit target, out bool critical)
    {
        critical = Random.value < unit.criticalChance;
        
        Debug.Log("Weapon attack Call" + " : " + weaponName);

        int dmg = critical ? GetFinalCriticalDamage() : GetFinalDamage();
        
        target.OnHit(dmg);
        
        onSuccessAttack.Invoke(target, dmg);
        if (critical)
        {
            Debug.Log("Critical");
            onCriticalAttack.Invoke(target, dmg);
        }
    }

    public override int GetFinalDamage()
    {
        return Mathf.RoundToInt(baseDamage + unit.revolverAdditionalDamage);
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = baseDamage + unit.revolverAdditionalDamage;
        dmg += dmg * unit.revolverCriticalDamage;

        return Mathf.RoundToInt(dmg);
    }

    public override float GetHitRate(Unit target)
    {
        int range = baseRange + unit.revolverAdditionalRange;
        int distance = Hex.Distance(unit.position, target.position);

        float hitRate = unit.concentration * (100 - distance * GetDistancePenalty() *
            (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1)
            ) * 0.01f;

        hitRate = Mathf.Round(10 * hitRate) * 0.1f;
        hitRate = Mathf.Clamp(hitRate, 0, 100);
        
        #if UNITY_EDITOR
        Debug.Log("Hitrate = " + hitRate);
        #endif
        
        return hitRate * 0.01f;
    }
}
