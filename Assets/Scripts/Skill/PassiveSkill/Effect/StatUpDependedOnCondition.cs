
using System;
using PassiveSkill;

public class StatUpDependedOnCondition : BaseEffect, IDisplayableEffect
{
    public StatUpDependedOnCondition(StatType statType, int amount) : base(statType, amount)
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
        
        unit.stat.Subtract(GetStatType(), GetAmount());
    }

    public override void OnConditionDisable()
    {
        if (enable is false) return;
        enable = false;
        
        unit.stat.Add(GetStatType(), GetAmount());
    }
    
    #region IDISPLAYABLE_EFFECT
    public string GetEffectName()
    {
        return GetEffectType().ToString();
    }

    public int GetStack()
    {
        return GetAmount();
    }

    public int GetDuration()
    {
        return IDisplayableEffect.NONE;
    }

    public bool CanDisplay()
    {
        return enable;
    }
    #endregion
}