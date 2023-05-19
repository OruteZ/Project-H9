using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shotgun : Weapon
{
    public override WeaponType GetWeaponType() => WeaponType.Shotgun;
}
