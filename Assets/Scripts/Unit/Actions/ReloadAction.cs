using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAction : BaseAction
{
    private Weapon weapon => unit.weapon;
    
    public override ActionType GetActionType()
    {
        return ActionType.Reload;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        return;
    }

    public override int GetCost()
    {
        return 3;
    }

    public override bool IsSelectable()
    {
        return weapon.currentEmmo < weapon.maxEmmo;
    }

    public override bool ExecuteImmediately()
    {
        return true;
    }

    public override bool CanExecute()
    {
        return IsSelectable();
    }

    public override void Execute(Action onActionComplete)
    {
        Debug.Log("Reload weapon");

        StartAction(onActionComplete);
        weapon.currentEmmo = weapon.maxEmmo;
        FinishAction();
    }
}
