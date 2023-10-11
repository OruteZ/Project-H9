
using System;
using PassiveSkill;

public class StatUpDependedOnCondition : BaseEffect
{
    public StatUpDependedOnCondition(UnitStatType statType, float amount) : base(statType, amount)
    { }

    protected override void EffectSetup()
    { }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.StatUpDependedOnCondition;
    }

    public override void OnConditionEnable()
    {
        if (enable) return;
        enable = true;
        
        var stat = unit.GetStat();

        switch (GetStatType())
        {
            case UnitStatType.MaxHp:
                stat.maxHp += (int)GetAmount();
                break;
            case UnitStatType.CurHp:
                stat.curHp += (int)GetAmount();
                break;
            case UnitStatType.Concentration:
                stat.concentration += (int)GetAmount();
                break;
            case UnitStatType.SightRange:
                stat.sightRange += (int)GetAmount();
                break;
            case UnitStatType.Speed:
                stat.speed += (int)GetAmount();
                break;
            case UnitStatType.ActionPoint:
                unit.currentActionPoint += (int)GetAmount();
                break;
            case UnitStatType.AdditionalHitRate:
                break;
            case UnitStatType.CriticalChance:
                break;
            case UnitStatType.RevolverAdditionalDamage:
                break;
            case UnitStatType.RepeaterAdditionalDamage:
                break;
            case UnitStatType.ShotgunAdditionalDamage:
                break;
            case UnitStatType.RevolverAdditionalRange:
                break;
            case UnitStatType.RepeaterAdditionalRange:
                break;
            case UnitStatType.ShotgunAdditionalRange:
                break;
            case UnitStatType.RevolverCriticalDamage:
                break;
            case UnitStatType.RepeaterCriticalDamage:
                break;
            case UnitStatType.ShotgunCriticalDamage:
                break;
            case UnitStatType.AllAdditionalDamage:
                stat.revolverAdditionalDamage += (int)GetAmount();
                stat.repeaterAdditionalDamage += (int)GetAmount();
                stat.shotgunAdditionalDamage += (int)GetAmount();
                break;
            case UnitStatType.AllAdditionalRange:
                break;
            case UnitStatType.AllCriticalDamage:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void OnConditionDisable()
    {
        if (enable is false) return;
        enable = false;
        
        var stat = unit.GetStat();

        switch (GetStatType())
        {
            case UnitStatType.MaxHp:
                stat.maxHp -= (int)GetAmount();
                break;
            case UnitStatType.CurHp:
                stat.curHp -= (int)GetAmount();
                break;
            case UnitStatType.Concentration:
                stat.concentration -= (int)GetAmount();
                break;
            case UnitStatType.SightRange:
                stat.sightRange -= (int)GetAmount();
                break;
            case UnitStatType.Speed:
                stat.speed -= (int)GetAmount();
                break;
            case UnitStatType.ActionPoint:
                break;
            case UnitStatType.AdditionalHitRate:
                break;
            case UnitStatType.CriticalChance:
                break;
            case UnitStatType.RevolverAdditionalDamage:
                break;
            case UnitStatType.RepeaterAdditionalDamage:
                break;
            case UnitStatType.ShotgunAdditionalDamage:
                break;
            case UnitStatType.RevolverAdditionalRange:
                break;
            case UnitStatType.RepeaterAdditionalRange:
                break;
            case UnitStatType.ShotgunAdditionalRange:
                break;
            case UnitStatType.RevolverCriticalDamage:
                break;
            case UnitStatType.RepeaterCriticalDamage:
                break;
            case UnitStatType.ShotgunCriticalDamage:
                break;
            case UnitStatType.AllAdditionalDamage:
                stat.revolverAdditionalDamage -= (int)GetAmount();
                stat.repeaterAdditionalDamage -= (int)GetAmount();
                stat.shotgunAdditionalDamage -= (int)GetAmount();
                break;
            case UnitStatType.AllAdditionalRange:
                break;
            case UnitStatType.AllCriticalDamage:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}