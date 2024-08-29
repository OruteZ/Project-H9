using PassiveSkill;
using UnityEngine;

public class FreeReload : BaseEffect
{
    public FreeReload(StatType statType, int amount) : base(statType, amount)
    {
    }

    protected override void EffectSetup()
    {
    }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.FreeReload;
    }

    public override void OnConditionEnable()
    {
        unit.tacticalReloadChance += GetAmount();
    }

    public override void OnConditionDisable()
    {
        unit.tacticalReloadChance -= GetAmount();
    }
}
