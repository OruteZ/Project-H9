using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    public Melee(WeaponData data) : base(data)
    {
    }

    public override ItemType GetWeaponType()
    {
        return ItemType.Character;
    }

    public override int GetFinalDamage()
    {
        throw new System.NotImplementedException();
    }

    public override int GetFinalCriticalDamage()
    {
        throw new System.NotImplementedException();
    }

    public override float GetFinalHitRate(IDamageable target)
    {
        throw new System.NotImplementedException();
    }

    public override float CalculateDistancePenalty(IDamageable target)
    {
        throw new System.NotImplementedException();
    }

    public override float GetFinalCriticalRate()
    {
        throw new System.NotImplementedException();
    }

    public override int GetRange()
    {
        throw new System.NotImplementedException();
    }
}
