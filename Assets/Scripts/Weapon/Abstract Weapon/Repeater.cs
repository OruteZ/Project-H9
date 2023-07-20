using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : Weapon
{
    public override void Attack(Unit target, out bool isCritical)
    {
        throw new System.NotImplementedException();
    }

    public override WeaponType GetWeaponType() => WeaponType.Repeater;
    public override int GetFinalDamage()
    {
        throw new System.NotImplementedException();
    }

    public override int GetFinalCriticalDamage()
    {
        throw new System.NotImplementedException();
    }

    public override float GetFinalHitRate(Unit target)
    {
        throw new System.NotImplementedException();
    }

    public override float GetDistancePenalty()
    {
        throw new System.NotImplementedException();
    }
}
