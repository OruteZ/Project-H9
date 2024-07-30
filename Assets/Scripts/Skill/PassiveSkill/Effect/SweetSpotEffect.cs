using PassiveSkill;
using UnityEngine;

public class SweetSpotEffect : BaseEffect, IDisplayableEffect
{
    public SweetSpotEffect(StatType statType, int amount) : base(statType, amount)
    {
    }

    public override PassiveEffectType GetEffectType()
    {
        return PassiveEffectType.SweetSpot;
    }
    private void CheckIfEnemyOnSweetSpot(IDamageable target) 
    {
        if (unit.weapon is not Repeater repeater) return;
        var dist = Hex.Distance(unit.hexPosition, target.GetHex());
        if (dist == repeater.GetSweetSpot())
        {
            unit.stat.Add(StatType.CriticalChance, GetAmount());
        }
    }
    private void ClearEffect(Damage context)
    {
        unit.stat.Subtract(StatType.CriticalChance, GetAmount());
    }

    public override void OnConditionEnable()
    {
        if (enable) return;
        enable = true;
    }

    public override void OnConditionDisable()
    {
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
    {
        unit.onStartShoot.AddListener(CheckIfEnemyOnSweetSpot);
        unit.onFinishShoot.AddListener(ClearEffect);
    }
    #endregion
}
