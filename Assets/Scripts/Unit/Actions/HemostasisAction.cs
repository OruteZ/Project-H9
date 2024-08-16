using System.Collections;
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

    public override bool CanExecute(Vector3Int targetPos)
    {
        return IsSelectable();
    }

    public override bool IsSelectable()
    {
        if (unit.GetAction<ItemUsingAction>().GetItemUsedTrigger()) return false;

        return unit.HasStatusEffect(StatusEffectType.Bleeding);
    }

    public override bool CanExecute()
    {
        return IsSelectable();
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.SetTrigger(IDLE);
        
        yield return new WaitForSeconds(0);

        if (unit.TryGetStatusEffect(StatusEffectType.Bleeding, out var bleeding))
        {
            bleeding.damage--;
        }
    }

    public override int GetSkillIndex()
    {
        Debug.LogError("unexpected access");
        return 0;
    }
}