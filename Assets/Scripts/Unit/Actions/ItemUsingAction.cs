using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUsingAction : BaseAction
{
    private Vector3Int _targetPos;
    private IItem _item;
    private bool _itemUsedTrigger;

    public override ActionType GetActionType()
    {
        return ActionType.ItemUsing;
    }

    public bool GetItemUsedTrigger()
    {
        return _itemUsedTrigger;
    }

    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        _itemUsedTrigger = false;
        unit.onFinishAction.AddListener((none) => ResetUseTrigger());
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _targetPos = targetPos;
    }

    public void SetItem(IItem item)
    {
        _item = item;
    }

    public override bool CanExecute()
    {
        if (_itemUsedTrigger) return false;
        return _item is not null;
    }

    public override bool IsSelectable()
    {
        if (_itemUsedTrigger) return false;

        return _item is not null;
    }

    public override bool CanExecuteImmediately()
    {
        return _item.IsImmediate();
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        _item.Use(unit, _targetPos);
        yield return null;
    }

    private void ResetUseTrigger()
    {
        _itemUsedTrigger = false;
    }
}