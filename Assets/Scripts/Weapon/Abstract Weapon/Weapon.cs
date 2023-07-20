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
    public int maxEmmo;
    public int currentEmmo;
    public float hitRate;
    public float criticalChance;
    public float criticalDamage;
    public int script;
    
    public UnitStat unitStat;
    public Unit unit;

    protected const float SHOTGUN_OVER_RANGE_PENALTY = 3f;
    protected const float REVOLVER_OVER_RANGE_PENALTY = 2.5f;
    protected const float REPEATER_OVER_RANGE_PENALTY = 2f;
    
    // private void SetUpGimmicks()
    // {
    //     foreach (var gimmick in gimmicks)
    //     {
    //         gimmick.Setup(this);
    //     }
    // }
    public abstract void Attack(Unit target, out bool isCritical);
    public abstract WeaponType GetWeaponType();
    public abstract int GetFinalDamage();
    public abstract int GetFinalCriticalDamage();
    public abstract float GetFinalHitRate(Unit target);
    public abstract float GetDistancePenalty();

    public void Reload()
    {
        currentEmmo = maxEmmo;
    }
}

public enum WeaponType
{
    Null,
    Character,
    Revolver,
    Repeater,
    Shotgun
}
