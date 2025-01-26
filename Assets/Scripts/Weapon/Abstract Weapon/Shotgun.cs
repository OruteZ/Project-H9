using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    private Vector3Int _targetHex;

    public Shotgun(WeaponData data) : base(data)
    {
    }

    public override ItemType GetWeaponType() => ItemType.Shotgun;

    public override float CalculateDistancePenalty(IDamageable target)
    {
        return 10;
    }
    
    public override int GetRange()
    {
        return weaponRange + magazine.GetNextBullet().data.range + UnitStat.shotgunAdditionalDamage;
    }

    public override float GetFinalCriticalRate()
    {
        Debug.Log("Weapon attack Call" + " : " + nameIndex);

        return  UnitStat.criticalChance + criticalChance + magazine.GetNextBullet().data.criticalChance;
    }

    public override int GetFinalDamage()
    {
        float baseDamage = (weaponDamage + magazine.GetNextBullet().data.damage + UnitStat.shotgunAdditionalDamage);
        
        // shotgun Requires target's reference.
        // if there is not target ( ex. attack to empty hex ) return 0
        IUnitAction action = unit.GetSelectedAction();
        if (action is not AttackAction attackAction)
        {
            Debug.LogError(
                "Action is not AttackAction. " +
                "shotgun requires only AttackAction" +
                "Current action is " + action.GetActionType()
            );
            
            return 0;
        }
        
        IDamageable target = attackAction.GetTarget();
        int finalDamage = 0;
        
        for (int i = 0; i < baseDamage; i++)
        {
            if (Random.Range(0, 100) < GetEachHitRate(target))
            {
                finalDamage++;
            }
        }

        return finalDamage;
    }

    public override int GetFinalCriticalDamage()
    {
        float dmg = GetFinalDamage();
        dmg += dmg * ((UnitStat.shotgunCriticalDamage + criticalDamage + +magazine.GetNextBullet().data.criticalDamage) * 0.01f);

        return Mathf.RoundToInt(dmg);
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        int range = GetRange();
        int distance = Hex.Distance(unit.hexPosition, target.GetHex());
        
        _targetHex = target.GetHex();

        float finalHitRate = distance <= range ? 100 : 0;

        return finalHitRate;
    }
    
    private float GetEachHitRate(IDamageable target)
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

        return finalHitRate;
    }
}
