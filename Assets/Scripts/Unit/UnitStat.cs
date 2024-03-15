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
    private int[] original = new int[(int)StatType.Length - 3];
    [SerializeField]
    private int[] _additional = new int[(int)StatType.Length - 3];
    [SerializeField]
    private int[] _multiplier = new int[(int)StatType.Length - 3];

    public object Clone()
    {
        UnitStat newStat = new UnitStat();
        newStat.DeepCopy(original, _additional, _multiplier);

        return newStat;
    }

    public int GetStat(StatType type)
    {
        if (IsAllStat(type))
        {
            Debug.LogError("Trying to access AllStat, but it's not allowed");
            return -999999;
        }
        
        int idx = (int)type;
        
        return Mathf.RoundToInt((original[idx] + _additional[idx]) * (_multiplier[idx] / 100f));
    }

    public int GetOriginalStat(StatType type)
    {
        if (IsAllStat(type))
        {
            Debug.LogError("Trying to access AllStat, but it's not allowed");
            return -999999;
        }
        
        return original[(int)type];
    }

    public void SetOriginalStat(StatType type, int value)
    {
        //is all stat type, convert to array and set value
        if (IsAllStat(type))
        {
            StatType[] eachType = ConvertAllTypeToEachArray(type);
            foreach (var statType in eachType)
            {
                original[(int)statType] = value;
            }
            return;
        }
        
        original[(int)type] = value;
    }

    public void Add(StatType type, int value)
    {
        //is all stat type, convert to array and set value
        if (IsAllStat(type))
        {
            StatType[] eachType = ConvertAllTypeToEachArray(type);
            foreach (var statType in eachType)
            {
                _additional[(int)statType] += value;
            }
            return;
        }
        
        _additional[(int)type] += value;
    }

    public void AddMultiplier(StatType type, int value)
    {
        //is all stat type, convert to array and set value
        if (IsAllStat(type))
        {
            StatType[] eachType = ConvertAllTypeToEachArray(type);
            foreach (var statType in eachType)
            {
                _multiplier[(int)statType] += value;
            }
            return;
        }
        
        _multiplier[(int)type] += value;
    }

    public void Subtract(StatType type, int value)
    {
        //is all stat type, convert to array and set value
        if (IsAllStat(type))
        {
            StatType[] eachType = ConvertAllTypeToEachArray(type);
            foreach (var statType in eachType)
            {
                _additional[(int)statType] -= value;
            }

            return;
        }

        _additional[(int)type] -= value;
    }

    public void SubtractMultiplier(StatType type, int value)
    {
        //is all stat type, convert to array and set value
        if (IsAllStat(type))
        {
            StatType[] eachType = ConvertAllTypeToEachArray(type);
            foreach (var statType in eachType)
            {
                _multiplier[(int)statType] -= value;
            }

            return;
        }
        
        _multiplier[(int)type] -= value;
    }

    public void ResetModifier()
    {
        _additional = new int[(int)StatType.Length - 3];
        _multiplier = new int[(int)StatType.Length - 3];
        
        for (int i = 0; i < (int)StatType.Length - 3; i++)
        {
            _additional[i] = 0;
            _multiplier[i] = 100;
        }
    }

    public void Recover(StatType stat, int value, out int appliedValue)
    {
        if (stat is not StatType.CurHp and not StatType.CurActionPoint)
        {
            appliedValue = 0;
            Debug.LogError("Can't Recover Stat that is not curHp or curAp");
            return;
        }
        
        int maxValue = GetStat((StatType)((int)stat - 1));

        int idx = (int)stat;

        original[idx] = Mathf.Clamp(original[idx] + value, 0, maxValue);
        appliedValue = original[idx] - value;
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
        for (int i = 0; i < (int)StatType.Length - 3; i++)
        {
            this.original[i] = original[i];
            _additional[i] = additional[i];
            _multiplier[i] = multiplier[i];
        }
    }
    
    public static bool IsAllStat(StatType type)
    {
        return type is StatType.AllAdditionalDamage or StatType.AllAdditionalRange or StatType.AllCriticalDamage;
    }

    public static StatType[] ConvertAllTypeToEachArray(StatType allType)
    {
        //if it is not all stat type, return null
        if (IsAllStat(allType) is false) return null;
        
        StatType[] eachType = new StatType[3];
        switch (allType)
        {
            case StatType.AllAdditionalDamage:
                eachType[0] = StatType.RevolverAdditionalDamage;
                eachType[1] = StatType.RepeaterAdditionalDamage;
                eachType[2] = StatType.ShotgunAdditionalDamage;
                break;
            case StatType.AllAdditionalRange:
                eachType[0] = StatType.RevolverAdditionalRange;
                eachType[1] = StatType.RepeaterAdditionalRange;
                eachType[2] = StatType.ShotgunAdditionalRange;
                break;
            case StatType.AllCriticalDamage:
                eachType[0] = StatType.RevolverCriticalDamage;
                eachType[1] = StatType.RepeaterCriticalDamage;
                eachType[2] = StatType.ShotgunCriticalDamage;
                break;
        }
        
        return eachType;
    }
}


public enum StatType
{
    Null = 0,
    MaxHp = 1,
    CurHp = 2,
    Concentration = 3,
    SightRange = 4,
    Speed = 5,
    
    MaxActionPoint = 6,
    CurActionPoint = 7,
    
    AdditionalHitRate = 8,
    CriticalChance = 9,
    
    RevolverAdditionalDamage = 10,
    RevolverAdditionalRange = 11,
    RevolverCriticalDamage = 12,
    
    RepeaterAdditionalDamage = 13,
    RepeaterAdditionalRange = 14,
    RepeaterCriticalDamage = 15,
    
    ShotgunAdditionalDamage = 16,
    ShotgunAdditionalRange = 17,
    ShotgunCriticalDamage = 18,
    
    AllAdditionalDamage = 19,
    AllAdditionalRange = 20,
    AllCriticalDamage = 21,
    Length,
}