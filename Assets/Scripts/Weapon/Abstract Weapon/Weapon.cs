using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Weapon
{
    protected UnitStat unitStat;
    public static Weapon Clone(WeaponData data, Unit owner = null)
    {
        Weapon weapon = data.type switch
        {
            WeaponType.Revolver => new Revolver(),
            WeaponType.Repeater => new Repeater(),
            WeaponType.Shotgun => new Shotgun(),
            _ => throw new System.ArgumentOutOfRangeException()
        };

        weapon.unit = owner;
        weapon.unitStat = weapon.unit is null ? new UnitStat() : weapon.unit.GetStat();
        
        weapon.SetUpData(data);
        weapon.SetUpGimmicks();
        
        return weapon;
    }
    
    protected const float SHOTGUN_OVER_RANGE_PENALTY = 3f;
    protected const float REVOLVER_OVER_RANGE_PENALTY = 2.5f;
    protected const float REPEATER_OVER_RANGE_PENALTY = 2f;

    public Unit unit;
    
    public string weaponName;
    public GameObject weaponModel;
    public int baseDamage;
    public int baseRange;
    public List<Gimmick> gimmicks;

    [HideInInspector] public UnityEvent<Unit, int> onSuccessAttack;
    [HideInInspector] public UnityEvent<Unit, int> onCriticalAttack;

    private void SetUpData(WeaponData data)
    {
        weaponName = data.weaponName;
        weaponModel = data.weaponModel;
        baseDamage = data.baseDamage;
        baseRange = data.baseRange;

        onSuccessAttack = new UnityEvent<Unit, int>();
        onCriticalAttack = new UnityEvent<Unit, int>();
        
        gimmicks = new List<Gimmick>();
        foreach (var gimmickType in data.gimmicks)
        {
            gimmicks.Add(Gimmick.Clone(gimmickType));    
        }
    }
    private void SetUpGimmicks()
    {
        foreach (var gimmick in gimmicks)
        {
            gimmick.Setup(this);
        }
    }
    public abstract void Attack(Unit target, out bool isCritical);
    public abstract WeaponType GetWeaponType();
    public abstract int GetFinalDamage();
    public abstract int GetFinalCriticalDamage();
    public abstract float GetHitRate(Unit target);
    public abstract float GetDistancePenalty();
}

public enum WeaponType
{
    Revolver,
    Repeater,
    Shotgun
}
