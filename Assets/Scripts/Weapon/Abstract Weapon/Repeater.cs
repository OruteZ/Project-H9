using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : Weapon
{
    public Repeater(WeaponData data) : base(data)
    {
    }

    public override ItemType GetWeaponType() => ItemType.Repeater;
    public override float GetDistancePenalty() => 5;
    public override int GetRange()
    {
        return weaponRange + magazine.GetNextBullet().data.range + unitStat.repeaterAdditionalRange;
    }

    public override void Attack(IDamageable target, out bool isCritical)
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        isCritical = Random.value * 100 < unitStat.criticalChance + criticalChance + magazine.GetNextBullet().data.criticalChance;
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
        dmg += dmg * ((unitStat.revolverCriticalDamage + criticalDamage + magazine.GetNextBullet().data.criticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        int range = GetRange();
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());

        float finalHitRate = (hitRate + magazine.GetNextBullet().data.hitRate + unitStat.concentration * 
            (100 - distance * (IsSweetSpot(distance) ? 0 : GetDistancePenalty()) *
            (distance > range ? REPEATER_OVER_RANGE_PENALTY : 1)
            )) * 0.01f;

        finalHitRate = Mathf.Round(10 * finalHitRate) * 0.1f;
        finalHitRate = Mathf.Clamp(finalHitRate, 0, 100);


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
