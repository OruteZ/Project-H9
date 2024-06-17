using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PassiveSkill;

public class KillCondition : BaseCondition
{
    public KillCondition(float amt) : base(amt)
    { }

    public override ConditionType GetConditionType() => ConditionType.KillEnemy;

    protected override void ConditionSetup()
    {
        unit.onKill.AddListener(OnKill);
    }

    protected void OnKill(Unit target)
    {
        passive.FullfillCondition(this);
    }
}

public class KillOnSweetSpotCondition : BaseCondition
{
    public KillOnSweetSpotCondition(float amt) : base(amt)
    { }

    public override ConditionType GetConditionType() => ConditionType.KillEnemyOnSweetSpot;

    IDamageable _killTarget;
    protected override void ConditionSetup()
    {
        _killTarget = null;
        unit.onStartShoot.AddListener(SetTarget);
        unit.onKill.AddListener(OnKill);
    }

    private void SetTarget(IDamageable target)
    {
        var dist = Hex.Distance(unit.hexPosition, target.GetHex());
        if (unit.weapon is not Repeater repeater) return;

        if (dist == repeater.GetSweetSpot()) 
        {
            _killTarget = target;
        }
        else
        {
            _killTarget = null;
        }
    }
    protected void OnKill(Unit target)
    {
        if (target == (Unit)_killTarget)
        {
            passive.FullfillCondition(this);
        }
        else
        {
            passive.NotFullfillCondition(this);
        }
    }
}