using PassiveSkill;

public class InfinityShootPoint : BaseEffect
{
    public InfinityShootPoint(StatType statType, int amount) : base(statType, amount)
    {
    }

    protected override void EffectSetup()
    {
    }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.InfinityShootPoint;
    }

    public override void OnConditionEnable()
    {
        unit.freeReloadTrigger = true;
    }

    public override void OnConditionDisable()
    {
        unit.freeReloadTrigger = false;
    }
}