using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    public override void Attack(IDamageable target, out bool isCritical)
    {
        throw new System.NotImplementedException();
    }

    public override ItemType GetWeaponType()
    {
        throw new System.NotImplementedException();
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

    public override float GetDistancePenalty()
    {
        throw new System.NotImplementedException();
    }

    public override int GetRange()
    {
        throw new System.NotImplementedException();
    }
}
