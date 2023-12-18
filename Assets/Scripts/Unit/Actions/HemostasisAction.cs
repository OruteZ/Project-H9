﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HemostasisAction : BaseAction
{
    public override ActionType GetActionType()
    {
        return ActionType.Hemostasis;
    }

    public override void SetTarget(Vector3Int targetPos)
    { }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    public override bool IsSelectable()
    {
        if (unit.HasStatusEffect(StatusEffectType.Bleeding)) return true;
        
        return false;
    }

    public override bool CanExecute()
    {
        return IsSelectable();
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.SetTrigger(DYNAMITE);
        
        yield return new WaitForSeconds(0);

        unit.TryRemoveStatus(StatusEffectType.Bleeding);
    }
}