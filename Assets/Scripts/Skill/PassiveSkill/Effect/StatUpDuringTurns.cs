using PassiveSkill;
using UnityEngine;

public class StatUpDuringThreeTurn : BaseEffect, IDisplayableEffect
{
    int _duration;
    public StatUpDuringThreeTurn(StatType statType, int amount) : base(statType, amount)
    {
        unit.onTurnEnd.AddListener(OnTurnFinished);
    }

    public override PassiveEffectType GetEffectType() => PassiveEffectType.StatUpDuringThreeTurn;

    public override void OnConditionEnable()
    {
        if(!enable) unit.stat.Add(GetStatType(), GetAmount());
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
