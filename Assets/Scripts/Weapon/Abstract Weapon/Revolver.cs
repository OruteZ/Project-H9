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
    public override float GetDistancePenalty() => 5;
    public override int GetRange()
    {
        return weaponRange + magazine.GetNextBullet().data.range + unitStat.revolverAdditionalRange;
    }

    public override void Attack(IDamageable target, out bool isCritical)
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        isCritical = Random.value * 100 < criticalChance + magazine.GetNextBullet().data.criticalChance + unitStat.criticalChance;
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
        return Mathf.RoundToInt(weaponDamage + magazine.GetNextBullet().data.damage + unitStat.revolverAdditionalDamage);
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = weaponDamage + magazine.GetNextBullet().data.damage + unitStat.revolverAdditionalDamage;
        dmg += dmg * ((criticalDamage + magazine.GetNextBullet().data.criticalDamage + unitStat.revolverCriticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());
        int range = GetRange();

        float finalHitRate = (hitRate + +magazine.GetNextBullet().data.hitRate + unitStat.concentration * (100 - distance * GetDistancePenalty() *
            (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1)
            )) * 0.01f;

        finalHitRate = Mathf.Round(10 * finalHitRate) * 0.1f;
        finalHitRate = Mathf.Clamp(finalHitRate, 0, 100);
        finalHitRate += target.GetHitRateModifier();

#if UNITY_EDITOR
        UIManager.instance.debugUI.SetDebugUI
            (finalHitRate, unit, (Unit)target, distance, weaponRange,
                unitStat.revolverAdditionalRange,
                GetDistancePenalty() *
                (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1));
#endif

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
