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
    private int _curAmmo;
    public int hitRate;
    public int criticalChance;
    public int criticalDamage;
    public int script;

    public int currentAmmo
    {
        get => _curAmmo;
        set
        {
            int before = _curAmmo;
            _curAmmo = value;

            if (before == _curAmmo) return;
            if (unit is null) return;
                
            unit.onAmmoChanged.Invoke(before, _curAmmo);
        }
    }
    
    public UnitStat unitStat => unit.stat;
    public Unit unit;

    protected const float SHOTGUN_OVER_RANGE_PENALTY = 3f;
    protected const float REVOLVER_OVER_RANGE_PENALTY = 2f;
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
    Shotgun,
    Unarmed,
}
