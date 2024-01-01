using PassiveSkill;

public class LightFootStep : BaseEffect
{
    public LightFootStep(StatType statType, int amount) : base(statType, amount)
    {
    }

    protected override void EffectSetup()
    {
    }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.LightFootStep;
    }

    public override void OnConditionEnable()
    {
        unit.lightFootTrigger = true;
    }

    public override void OnConditionDisable()
    {
        unit.lightFootTrigger = false;
    }
}