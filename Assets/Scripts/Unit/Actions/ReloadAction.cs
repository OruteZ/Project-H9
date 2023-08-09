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

    public override int GetAmmoCost()
    {
        return 0;
    }

    public override bool IsSelectable()
    {
        return weapon.currentAmmo < weapon.maxAmmo;
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

        if (((_stateTimer -= Time.deltaTime) > 0)) return;
        
        switch (_curState)
        {
            case State.Reloading:
                unit.weapon.currentAmmo = unit.weapon.maxAmmo;
                _stateTimer = coolOffTime;
                _curState = State.CoolOff;
                break;
            case State.CoolOff:
                FinishAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
