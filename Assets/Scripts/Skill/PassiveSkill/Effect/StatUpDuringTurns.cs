using PassiveSkill;
using UnityEngine;

public class StatUpDuringATurn : BaseEffect, IDisplayableEffect
{
    int _duration;
    public StatUpDuringATurn(StatType statType, int amount) : base(statType, amount)
    {
    }

    public override PassiveEffectType GetEffectType() => PassiveEffectType.StatUpDuringThreeTurns;

    public override void OnConditionEnable()
    {
        if (!enable) unit.stat.Add(GetStatType(), GetAmount());
        enable = true;
        _duration = 1;
    }

    public override void OnConditionDisable()
    {
    }
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
    protected override void EffectSetup()
    {
        unit.onTurnEnd.AddListener(OnTurnFinished);
    }
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
public class StatUpDuringThreeTurns : BaseEffect, IDisplayableEffect
{
    int _duration;
    public StatUpDuringThreeTurns(StatType statType, int amount) : base(statType, amount)
    {
    }

    public override PassiveEffectType GetEffectType() => PassiveEffectType.StatUpDuringThreeTurns;

    public override void OnConditionEnable()
    {
        if (!enable) unit.stat.Add(GetStatType(), GetAmount());
        enable = true;
        _duration = 3;
    }

    public override void OnConditionDisable()
    {
    }
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
    protected override void EffectSetup()
    {
        unit.onTurnEnd.AddListener(OnTurnFinished);
    }
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
