using PassiveSkill;

public class DemolitionDynamite : BaseEffect
{
    public DemolitionDynamite(StatType statType, int amount) : base(statType, amount)
    {
    }

    protected override void EffectSetup()
    {
    }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.DemolitionDynamite;
    }

    public override void OnConditionEnable()
    {
        unit.coverObjDmgMultiplier = GetAmount() / 100.0f;
    }

    public override void OnConditionDisable()
    {
        unit.coverObjDmgMultiplier = 1;
    }
}
