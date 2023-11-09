using System;
using Unity.VisualScripting;
using UnityEngine;

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

    [SerializeField]
    private int[] original = new int[(int)StatType.Length];
    private int[] _additional = new int[(int)StatType.Length];
    private int[] _multiplier = new int[(int)StatType.Length];

    public object Clone()
    {
        UnitStat newStat = new UnitStat();
        newStat.DeepCopy(original, _additional, _multiplier);

        return newStat;
    }

    public int GetStat(StatType type)
    {
        int idx = (int)type;
        
        return Mathf.RoundToInt((original[idx] + _additional[idx]) * (_multiplier[idx] / 100f));
    }

    public int GetOriginalStat(StatType type)
    {
        return original[(int)type];
    }

    public void SetOriginalStat(StatType type, int value)
    {
        original[(int)type] = value;
    }

    public void Subtract(StatType type, int value)
    {
        _additional[(int)type] += value;
    }

    public void AddMultiplier(StatType type, int value)
    {
        _multiplier[(int)type] += value;
    }

    public void Add(StatType type, int value)
    {
        _additional[(int)type] -= value;
    }

    public void SubtractMultiplier(StatType type, int value)
    {
        _multiplier[(int)type] -= value;
    }

    public void ResetModifier()
    {
        for (int i = 0; i < (int)StatType.Length; i++)
        {
            _additional[i] = 0;
            _multiplier[i] = 100;
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

    public void DeepCopy(int[] original, int[] additional, int[] multiplier)
    {
        for (int i = 0; i < (int)StatType.Length; i++)
        {
            this.original[i] = original[i];
            _additional[i] = additional[i];
            _multiplier[i] = multiplier[i];
        }
    }
}


public enum StatType
{
    MaxHp, // 0
    CurHp, // 1
    Concentration, // 2
    SightRange, // 3
    Speed, // 4
    MaxActionPoint,// 5
    CurActionPoint, //6
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