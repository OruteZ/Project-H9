using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : Weapon
{
     public override ItemType GetWeaponType() => ItemType.Repeater;
    public override float GetDistancePenalty() => 5;
    public override int GetRange()
    {
        return weaponRange + unitStat.revolverAdditionalRange;
    }

    public override void Attack(Unit target, out bool isCritical)
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        isCritical = Random.value * 100 < unitStat.criticalChance + criticalChance;
        if (isCritical)
        {
            CriticalAttack(target);
        }
        else
        {
            NonCriticalAttack(target);
        }
        //UIManager.instance.combatUI.enemyHpUI.SetEnemyHpBars(); //test
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

    public override float GetFinalHitRate(Unit target)
    {
        int range = weaponRange + unitStat.revolverAdditionalRange;
        int distance = Hex.Distance(unit.hexPosition, target.hexPosition);

        float finalHitRate = (hitRate + unitStat.concentration * 
            (100 - distance * (IsSweetSpot(distance) ? 0 : GetDistancePenalty()) *
            (distance > range ? REPEATER_OVER_RANGE_PENALTY : 1)
            )) * 0.01f;

        finalHitRate = Mathf.Round(10 * finalHitRate) * 0.1f;
        finalHitRate = Mathf.Clamp(finalHitRate, 0, 100);

        UIManager.instance.debugUI.SetDebugUI
            (finalHitRate, unit, target, distance, weaponRange,
                unitStat.revolverAdditionalRange,
                (IsSweetSpot(distance) ? 0 : GetDistancePenalty()) *
                (distance > range ? REPEATER_OVER_RANGE_PENALTY : 1));
        
        return finalHitRate;
    }

    private void NonCriticalAttack(Unit target)
    {
        int damage = GetFinalDamage();
        target.TakeDamage(damage, unit);
    }

    private void CriticalAttack(Unit target)
    {
        int damage = GetFinalCriticalDamage();
        target.TakeDamage(damage, unit, Damage.Type.Critical);
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
