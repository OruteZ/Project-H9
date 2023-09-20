using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Weapon
{
    //Status
    public int nameIndex;
    public GameObject model;
    public int weaponDamage;
    public int weaponRange;
    public int maxAmmo;
    public int currentAmmo;
    public float hitRate;
    public float criticalChance;
    public float criticalDamage;
    public int script;
    
    public UnitStat unitStat => unit.GetStat();
    public Unit unit;
    public WeaponModel weaponModel;

    protected const float SHOTGUN_OVER_RANGE_PENALTY = 3f;
    protected const float REVOLVER_OVER_RANGE_PENALTY = 2.5f;
    protected const float REPEATER_OVER_RANGE_PENALTY = 2f;
    
   
    /// <summary>
    /// 적을 향해 사격합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="isCritical"></param>
    public abstract void Attack(Unit target, out bool isCritical);
    public abstract WeaponType GetWeaponType();
    public abstract int GetFinalDamage();
    public abstract int GetFinalCriticalDamage();
    public abstract float GetFinalHitRate(Unit target);
    public abstract float GetDistancePenalty();

    public abstract int GetRange();
}

public enum WeaponType
{
    Null,
    Character,
    Revolver,
    Repeater,
    Shotgun
}
