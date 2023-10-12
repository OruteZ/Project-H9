using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NSubstitute.Exceptions;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing.Extension;

public interface IUnit
{
    /// <summary>
    /// 해당 유닛이 턴을 시작합니다.
    /// </summary>
    void StartTurn();
    
    /// <summary>
    /// damage 만큼 피해를 입습니다.
    /// </summary>
    /// <param name="damage">피해량</param>
    void GetDamage(int damage);

    /// <summary>
    /// Unit의 상태를 설정합니다.
    /// </summary>
    /// <param name="newName">유닛의 이름</param>
    /// <param name="unitStat">유닛의 스탯</param>
    /// <param name="newWeapon">무기</param>
    /// <param name="unitModel"></param>
    /// <param name="passiveList">패시브 스킬의 List</param>
    void SetUp(string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel, List<Passive> passiveList);
    
    /// <summary>
    /// 특정 타입에 해당하는 액션을 반환합니다. 가지고 있지 않을경우 NoneAction을 반환합니다.
    /// </summary>
    /// <typeparam name="T">Action의 타입입니다.</typeparam>
    /// <returns>유닛이 가진 해당 Action을 반환합니다.</returns>
    T GetAction<T>();
    
    /// <summary>
    /// 해당 Unit이 가진 모든 Action을 반환합니다.
    /// </summary>
    /// <returns>UnitAction의 배열</returns>
    IUnitAction[] GetUnitActionArray();
}

[Serializable]
public class UnitStat : ICloneable
{
    public UnitStat()
    {
        ResetModifier();
    }
    
    public int maxHp => GetStat(StatType.MaxHp);
    public int curHp => GetStat(StatType.CurHp);

    public int concentration => GetStat(StatType.Concentration); 
    public int sightRange => GetStat(StatType.SightRange); 
    public int speed => GetStat(StatType.Speed);
    public int maxActionPoint => GetStat(StatType.MaxActionPoint);
    public float additionalHitRate => GetStat(StatType.CurActionPoint);
    public float criticalChance => GetStat(StatType.CriticalChance);
    public int revolverAdditionalDamage => GetStat(StatType.RevolverAdditionalDamage);
    public int repeaterAdditionalDamage => GetStat(StatType.RepeaterAdditionalDamage);
    public int shotgunAdditionalDamage => GetStat(StatType.ShotgunAdditionalDamage);
    public int revolverAdditionalRange => GetStat(StatType.RevolverAdditionalRange);
    public int repeaterAdditionalRange => GetStat(StatType.RepeaterAdditionalRange);
    public int shotgunAdditionalRange => GetStat(StatType.ShotgunAdditionalRange);
    public float revolverCriticalDamage => GetStat(StatType.RevolverCriticalDamage);
    public float shotgunCriticalDamage => GetStat(StatType.ShotgunCriticalDamage);
    public float repeaterCriticalDamage => GetStat(StatType.RepeaterCriticalDamage);

    public int[] original = new int[(int)StatType.Length];
    private int[] _additional = new int[(int)StatType.Length];
    private float[] _multiplier = new float[(int)StatType.Length];

    public object Clone()
    {
        return MemberwiseClone();
    }

    public int GetStat(StatType type)
    {
        int idx = (int)type;
        
        //todo : 계산 최종 값 int로 할지 반올림으로 할지 결정 후 변경
        return (int)((original[idx] + _additional[idx]) * _multiplier[idx]);
    }

    public int GetOriginalStat(StatType type)
    {
        return original[(int)type];
    }

    public void SetOriginalStat(StatType type, int value)
    {
        original[(int)type] = value;
    }

    public void AppendAdditional(StatType type, int value)
    {
        _additional[(int)type] += value;
    }

    public void AppendMultiplier(StatType type, float value)
    {
        _multiplier[(int)type] += value;
    }

    public void RemoveAdditional(StatType type, int value)
    {
        _additional[(int)type] -= value;
    }

    public void RemoveMultiplier(StatType type, float value)
    {
        _multiplier[(int)type] -= value;
    }

    public void ResetModifier()
    {
        for (int i = 0; i < 18; i++)
        {
            _additional[i] = 0;
            _multiplier[i] = 1;
        }
    }

    public void Recover(StatType stat, int value)
    {
        if (stat is not StatType.CurHp and not StatType.CurActionPoint)
        {
            Debug.LogError("Can't Recover Stat that is not curHp or curAp");
            return;
        }

        int maxValue = GetStat((StatType)((int)stat - 1));

        int idx = (int)stat;
        original[idx] = Mathf.Clamp(original[idx] + value, 0, maxValue);
    }

    public void Consume(StatType stat, int value)
    {
        if (stat is not StatType.CurHp and not StatType.CurActionPoint)
        {
            Debug.LogError("Can't Recover Stat that is not curHp or curAp");
            return;
        }
        int maxValue = GetStat((StatType)((int)stat - 1));
        
        int idx = (int)stat;
        original[idx] = Mathf.Clamp(original[idx] - value, 0, maxValue);
    }

    public bool TryConsume(StatType stat, int value)
    {
        if (stat is not StatType.CurHp and not StatType.CurActionPoint)
        {
            Debug.LogError("Can't Recover Stat that is not curHp or curAp");
            return false;
        }

        if (value > GetStat(stat)) 
            return false;
        
        original[(int)stat] -= value; 
        return true;
    }
}


public enum StatType
{
    MaxHp, //1
    CurHp, // 2
    Concentration, // 3
    SightRange, // 4
    Speed, // 5
    MaxActionPoint,// 6
    CurActionPoint,
    AdditionalHitRate,// 7
    CriticalChance,// 8
    RevolverAdditionalDamage,// 9
    RepeaterAdditionalDamage,// 10
    ShotgunAdditionalDamage,// 11
    RevolverAdditionalRange,// 12
    RepeaterAdditionalRange,// 13
    ShotgunAdditionalRange,// 14
    RevolverCriticalDamage,// 15
    RepeaterCriticalDamage,// 16
    ShotgunCriticalDamage,// 18
    Length,
}