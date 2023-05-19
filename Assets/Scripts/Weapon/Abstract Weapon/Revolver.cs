using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Revolver : Weapon
{
    public override WeaponType GetWeaponType() => WeaponType.Revolver;
    public override void Attack(Unit target)
    {
        Debug.Log("Weapon attack Call : " + name);
        target.OnHit(damage);
    }
}
