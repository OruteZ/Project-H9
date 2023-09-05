using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : BaseAction
{
    public override ActionType GetActionType()
    {
        return ActionType.Idle;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        return;
    }

    public override int GetCost()
    {
        return 0;
    }

    public override int GetAmmoCost()
    {
        return 0;   
    }

    public override bool IsSelectable()
    {
        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    public override bool CanExecute()
    {
        return false;
    }

    public override void Execute(Action onActionComplete)
    {
        unit.animator.SetTrigger(IDLE);
    }
}
