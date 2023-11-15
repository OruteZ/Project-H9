using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBleedingAction : BaseAction
{
    public override ActionType GetActionType()
    {
        return ActionType.StopBleeding;
    }

    public override void SetTarget(Vector3Int targetPos)
    { }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    public override int GetAmmoCost()
    {
        return 0;
    }

    public override int GetCost()
    {
        return 1;
    }

    public override bool IsSelectable()
    {
        if (unit.HasStatusEffect(StatusEffectType.Bleeding)) return true;
        else return false;
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