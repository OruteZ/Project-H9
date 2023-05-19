using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    protected Unit unit;

    public int damage;
    public int range;
    public abstract void Attack(Unit target);
    public abstract WeaponType GetWeaponType();
}

public enum WeaponType
{
    Revolver,
    Repeater,
    Shotgun
}
