using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Repeater : Weapon
{
    public override WeaponType GetWeaponType() => WeaponType.Repeater;
}
