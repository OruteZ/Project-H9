using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneAction : BaseAction
{
    public override ActionType GetActionType()
    {
        return ActionType.None;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        Debug.LogError("None action doesn't have SetTarget func");
        throw new FieldAccessException();
    }

    public override int GetCost()
    {
        return 0;
    }

    public override bool CanExecute()
    {
        Debug.LogError("None action doesn't have CanExecute func");
        throw new FieldAccessException();
    }

    public override void Execute(Action onActionComplete)
    {
        Debug.LogError("None action doesn't have Execute func");
        throw new FieldAccessException();
    }
}
