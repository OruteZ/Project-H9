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
    public int hitRate;
    public int criticalChance;
    public int criticalDamage;
    public int script;

    public Magazine magazine;

    public int currentAmmo
    {
        get => magazine.bullets.Count;
        set
        {
            int before = magazine.bullets.Count;
            int after = value;

            if (before == after) return;
            if (unit is null) return;

            if (Mathf.Abs(before - after) > 1) 
            {
                Debug.LogError("??");
            }
            if (before > after) magazine.UseBullet();
            else magazine.LoadBullet();


            unit.onAmmoChanged.Invoke(before, after);
        }
    }
    
    public UnitStat unitStat => unit.stat;
    public Unit unit;

    protected const float SHOTGUN_OVER_RANGE_PENALTY = 3f;
    protected const float REVOLVER_OVER_RANGE_PENALTY = 2f;
    protected const float REPEATER_OVER_RANGE_PENALTY = 1.25f;

    public Weapon(WeaponData data) 
    {
        nameIndex = data.weaponNameIndex;
        model = data.weaponModel;
        if (data.weaponModel == null)
        {
            Debug.LogError("Weapon Model Is NULL");
        }
        if (model == null)
        {
            Debug.LogError("Weapon Model Is NULL");
        }
        weaponDamage = data.weaponDamage;
        weaponRange = data.weaponRange;
        maxAmmo = data.weaponAmmo;
        //currentAmmo = data.weaponAmmo;
        hitRate = data.weaponHitRate;
        criticalChance = data.weaponCriticalChance;
        criticalDamage = data.weaponCriticalDamage;
        script = data.weaponScript;

        magazine = new Magazine(maxAmmo);
    }
   
    /// <summary>
    /// 적을 향해 사격합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="isCritical"></param>
    public abstract void Attack(Unit target, out bool isCritical);
    public abstract ItemType GetWeaponType();
    public abstract int GetFinalDamage();
    public abstract int GetFinalCriticalDamage();
    public abstract float GetFinalHitRate(Unit target);
    public abstract float GetDistancePenalty();

    public abstract int GetRange();
}
