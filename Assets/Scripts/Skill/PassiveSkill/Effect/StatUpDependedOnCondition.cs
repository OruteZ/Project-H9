
using System;
using PassiveSkill;

public class StatUpDependedOnCondition : BaseEffect
{
    public StatUpDependedOnCondition(StatType statType, float amount) : base(statType, amount)
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
        
        unit.stat.Subtract(GetStatType(), (int)GetAmount());
    }

    public override void OnConditionDisable()
    {
        if (enable is false) return;
        enable = false;
        
        unit.stat.Add(GetStatType(), (int)GetAmount());
    }
}