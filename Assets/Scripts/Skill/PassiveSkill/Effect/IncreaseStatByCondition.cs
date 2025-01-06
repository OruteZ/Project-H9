using PassiveSkill;

public class IncreaseStatByCondition : BaseEffect, IDisplayableEffect
{
    public IncreaseStatByCondition(StatType statType, int amount) : base(statType, amount) { }
    public override PassiveEffectType GetEffectType() => PassiveEffectType.IncreaseStatByCondition;
    protected override void EffectSetup() { }

    public override void OnConditionEnable()
    {
        if (enable) return;
        enable = true;
        unit.stat.Add(GetStatType(), GetAmount());
    }
    public override void OnConditionDisable()
    {
        if (enable is false) return;
        enable = false;

        unit.stat.Subtract(GetStatType(), GetAmount());
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

    #endregion
}