using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : Weapon
{
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

    private void NonCriticalAttack(Unit target)
    {
        int damage = GetFinalDamage();
        target.TakeDamage(damage, unit);
        Service.SetText(index:0, damage.ToString(), target.transform.position);
    }

    private void CriticalAttack(Unit target)
    {
        int damage = GetFinalCriticalDamage();
        target.TakeDamage(damage, unit);
        Service.SetText(index:1, damage.ToString(), target.transform.position);
    }

    public override WeaponType GetWeaponType() => WeaponType.Repeater;
    public override int GetFinalDamage()
    {
        return Mathf.RoundToInt(weaponDamage + unitStat.repeaterAdditionalDamage);
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = weaponDamage + unitStat.repeaterAdditionalDamage;
        dmg += dmg * (unitStat.repeaterCriticalDamage + criticalDamage);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(Unit target)
    {
        int range = weaponRange + unitStat.repeaterAdditionalRange;
        int distance = Hex.Distance(unit.hexPosition, target.hexPosition);

        float hitRate = this.hitRate + unitStat.concentration * (100 - distance * GetDistancePenalty() *
                (distance > range ? REPEATER_OVER_RANGE_PENALTY : 1)
            ) * 0.01f;

        hitRate = Mathf.Round(10 * hitRate) * 0.1f;
        hitRate = Mathf.Clamp(hitRate, 0, 100);

        // #if UNITY_EDITOR
        // Debug.Log("Hit rate = " + hitRate);
        // #endif

        UIManager.instance.debugUI.SetDebugUI(
            hitRate, unit, target, distance, weaponRange, unitStat.repeaterAdditionalRange, 
            GetDistancePenalty() * (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1));
        
        return hitRate * 0.01f;
    }

    public override float GetDistancePenalty()
    {
        return 4;
    }

    public override int GetRange()
    {
        return weaponRange + unitStat.repeaterAdditionalRange;
    }
}
