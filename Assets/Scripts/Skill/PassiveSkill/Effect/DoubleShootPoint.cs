using PassiveSkill;
using UnityEngine;

public class DoubleShootPoint : BaseEffect
{
    public DoubleShootPoint(StatType statType, int amount) : base(statType, amount)
    {
    }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.DoubleShootPoint;
    }

    public override void OnConditionEnable()
    {
        if (enable) return;
        enable = true;

        unit.doubleShootPointTrigger = true;
    }

    public override void OnConditionDisable()
    {
        unit.doubleShootPointTrigger = false;
    }

    #region IDISPLAYABLE_EFFECT

    public int GetIndex() => passive.index;
    public int GetStack() => GetAmount();
    public int GetDuration() => IDisplayableEffect.NONE;

    public bool CanDisplay()
    {
        if (passive is null) return false;
        if (passive.GetConditionType()[0] is ConditionType.Null) return false;
        
        return enable;
    }

    protected override void EffectSetup()
    { }
    #endregion
}
