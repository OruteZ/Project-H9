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

    public int currentAmmo
    {
        get => bulletQueue.Count;
        set
        {
            int before = currentAmmo;
            
            while(bulletQueue.Count < value)
            {
                bulletQueue.Enqueue(AmmoType.Bullet);
            }
            while(bulletQueue.Count > value)
            {
                bulletQueue.Dequeue();
            }
            

            if (before == currentAmmo) return;
            if (unit is null) return;
                
            unit.onAmmoChanged.Invoke(before, currentAmmo);
        }
    }

    public Queue<AmmoType> bulletQueue = new (); 
    
    public UnitStat unitStat => unit.stat;
    public Unit unit;

    protected const float SHOTGUN_OVER_RANGE_PENALTY = 3f;
    protected const float REVOLVER_OVER_RANGE_PENALTY = 2f;
    protected const float REPEATER_OVER_RANGE_PENALTY = 1.25f;
    
   
    /// <summary>
    /// 적을 향해 사격합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="isCritical"></param>
    public abstract void Attack(IDamageable target, out bool isCritical);
    public abstract ItemType GetWeaponType();
    public abstract int GetFinalDamage();
    public abstract int GetFinalCriticalDamage();
    public abstract float GetFinalHitRate(IDamageable target);
    public abstract float GetDistancePenalty();

    public abstract int GetRange();
    public AmmoType GetCurrentBullet()
    {
        return bulletQueue.Count == 0 ? AmmoType.None : bulletQueue.Peek();
    }
}

public enum AmmoType
{
    None,
    Bullet,
    GoldBullet,
}
