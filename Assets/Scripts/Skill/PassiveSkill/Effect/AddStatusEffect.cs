using PassiveSkill;
public abstract class IncreaseStatusEffectChance : BaseEffect, IDisplayableEffect
{
    protected bool isEffectOn = false;
    protected StatusEffectType statusEffectType = StatusEffectType.None;
    public IncreaseStatusEffectChance(StatType statType, int amount) : base(statType, amount) { }
    protected override void EffectSetup() { }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.Null;
    }

    public override void OnConditionEnable()
    {
        if (!isEffectOn)
        {
            isEffectOn = true;
            unit.statusEffectChances[statusEffectType] += GetAmount();
        }
    }
    public override void OnConditionDisable()
    {
        if (isEffectOn)
        {
            isEffectOn = false;
            unit.statusEffectChances[statusEffectType] -= GetAmount();
        }
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

public class IncreaseBleedingEffectChance : IncreaseStatusEffectChance
{
    public IncreaseBleedingEffectChance(StatType statType, int amount) : base(statType, amount) 
    {
        statusEffectType = StatusEffectType.Bleeding;
    }
    public override PassiveEffectType GetEffectType() => PassiveEffectType.Bleed;
}

/*
    None = 0,
    Burning = 10,
    Bleeding = 11,
    Stun = 12,
    UnArmed = 13,
    Taunted = 14,
    Concussion = 15,
    Fracture = 16,
    Blind = 17,
    Recoil = 18,
    Rooted = 19
 */
