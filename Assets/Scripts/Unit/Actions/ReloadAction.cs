using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAction : BaseAction
{
    private Weapon weapon => unit.weapon;

    private State _curState;
    public float animTime;
    public float coolOffTime;

    private enum State
    {
        Reloading,
        CoolOff
    }
    
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

    public override bool CanExecuteImmediately()
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
        _stateTimer = animTime;
        _curState = State.Reloading;
    }

    private float _stateTimer;
    private void Update()
    {
        if (isActive is false) return;

        if (!((_stateTimer -= Time.deltaTime) < 0)) return;
        
        switch (_curState)
        {
            case State.Reloading:
                unit.weapon.currentEmmo = unit.weapon.maxEmmo;
                break;
            case State.CoolOff:
                FinishAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
