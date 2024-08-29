using PassiveSkill;

public class IncreaseStatForOneTurn : BaseEffect, IDisplayableEffect
{
    protected int _duration;
    public IncreaseStatForOneTurn(StatType statType, int amount) : base(statType, amount) { }
    public override PassiveEffectType GetEffectType() => PassiveEffectType.IncreaseStatForOneTurn;
    protected override void EffectSetup()
    {
        unit.onTurnEnd.AddListener(OnTurnFinished);
    }

    public override void OnConditionEnable()
    {
        if (!enable) unit.stat.Add(GetStatType(), GetAmount());
        enable = true;
        _duration = 1;
    }
    public override void OnConditionDisable() { }

    public void OnTurnFinished(Unit u)
    {
        if (!enable || u != unit) return;
        //calcuate duration
        _duration -= 1;
        if (_duration <= 0)
        {
            enable = false;
            unit.stat.Subtract(GetStatType(), GetAmount());
            unit.RemoveDisplayableEffect(this);
        }
    }
    #region IDISPLAYABLE_EFFECT
    public int GetIndex() => passive.index;
    public int GetStack() => GetAmount();
    public int GetDuration() => _duration;

    public bool CanDisplay()
    {
        if (passive is null) return false;
        if (passive.GetConditionType()[0] is ConditionType.Null) return false;
        return enable;
    }
    #endregion
}
public class IncreaseStatForTwoTurns : IncreaseStatForOneTurn
{
    public IncreaseStatForTwoTurns(StatType statType, int amount) : base(statType, amount) { }
    public override PassiveEffectType GetEffectType() => PassiveEffectType.IncreaseStatForTwoTurns;

    public override void OnConditionEnable()
    {
        if (!enable) unit.stat.Add(GetStatType(), GetAmount());
        enable = true;
        _duration = 2;
    }
}
public class IncreaseStatForThreeTurns : IncreaseStatForOneTurn
{
    public IncreaseStatForThreeTurns(StatType statType, int amount) : base(statType, amount) { }
    public override PassiveEffectType GetEffectType() => PassiveEffectType.IncreaseStatForThreeTurns;

    public override void OnConditionEnable()
    {
        if (!enable) unit.stat.Add(GetStatType(), GetAmount());
        enable = true;
        _duration = 3;
    }
}
public class IncreaseStatForFourTurns : IncreaseStatForOneTurn
{
    public IncreaseStatForFourTurns(StatType statType, int amount) : base(statType, amount) { }
    public override PassiveEffectType GetEffectType() => PassiveEffectType.IncreaseStatForFourTurns;

    public override void OnConditionEnable()
    {
        if (!enable) unit.stat.Add(GetStatType(), GetAmount());
        enable = true;
        _duration = 4;
    }
}
public class IncreaseStatForFiveTurns : IncreaseStatForOneTurn
{
    public IncreaseStatForFiveTurns(StatType statType, int amount) : base(statType, amount) { }
    public override PassiveEffectType GetEffectType() => PassiveEffectType.IncreaseStatForFiveTurns;

    public override void OnConditionEnable()
    {
        if (!enable) unit.stat.Add(GetStatType(), GetAmount());
        enable = true;
        _duration = 5;
    }
}
