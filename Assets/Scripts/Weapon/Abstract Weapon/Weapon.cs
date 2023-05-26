using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Weapon
{
    public static Weapon Clone(WeaponData data, Unit unit = null)
    {
        Weapon result = data.type switch
        {
            WeaponType.Revolver => new Revolver(),
            WeaponType.Repeater => new Repeater(),
            WeaponType.Shotgun => new Shotgun(),
            _ => throw new System.ArgumentOutOfRangeException()
        };
        
        
        result.weaponName = data.weaponName;
        result.weaponModel = data.weaponModel;
        result.baseDamage = data.baseDamage;
        result.baseRange = data.baseRange;
        result.unit = unit;

        result.onSuccessAttack = new UnityEvent<Unit, int>();
        result.onCriticalAttack = new UnityEvent<Unit, int>();

        result.gimmicks = new List<Gimmick>();
        foreach (var gimmickType in data.gimmicks)
        {
            result.gimmicks.Add(Gimmick.Clone(gimmickType));    
        }
        
        foreach (var gimmick in result.gimmicks)
        {
            gimmick.Setup(result);
        }

        return result;
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
    public abstract void Attack(Unit target, out bool critical);
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
