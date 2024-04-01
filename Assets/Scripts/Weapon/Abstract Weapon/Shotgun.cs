using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    private Vector3Int _targetHex;
    public override ItemType GetWeaponType() => ItemType.Shotgun;
    public override float GetDistancePenalty() => 2;
    public override int GetRange()
    {
        return weaponRange + unitStat.shotgunAdditionalDamage;
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
    }

    public override int GetFinalDamage()
    {
        float baseDamage = (weaponDamage + unitStat.shotgunAdditionalDamage);

        int range = GetRange();
        int distance = Hex.Distance(unit.hexPosition, _targetHex);

        int value = range - distance;
        int damage = Mathf.RoundToInt(baseDamage * Mathf.Pow(2, value));
        
        return damage;
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = GetFinalDamage();
        dmg += dmg * ((unitStat.shotgunCriticalDamage + criticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(Unit target)
    {
        int range = GetRange();
        int distance = Hex.Distance(unit.hexPosition, target.hexPosition);
        
        _targetHex = target.hexPosition;

        float finalHitRate = distance <= range ? 100 : 0;

        #if UNITY_EDITOR
        //Debug.Log("Hit rate = " + finalHitRate);
        #endif

        UIManager.instance.debugUI.SetDebugUI
            (finalHitRate, unit, target, distance, weaponRange,
                unitStat.revolverAdditionalRange,
                GetDistancePenalty() *
                (distance > range ? REVOLVER_OVER_RANGE_PENALTY : 1));
        
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
        target.TakeDamage(damage, unit, eDamageType.Type.Critical);
    }
}
