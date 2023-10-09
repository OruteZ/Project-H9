using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseAction : MonoBehaviour, IUnitAction
{
    protected static readonly int MOVE = Animator.StringToHash("Move");
    protected static readonly int SHOOT = Animator.StringToHash("Shoot");
    protected static readonly int IDLE = Animator.StringToHash("Idle");
    protected static readonly int FANNING = Animator.StringToHash("Fanning");
    protected static readonly int RELOAD = Animator.StringToHash("Reload");
    
    public abstract ActionType GetActionType();
    public abstract bool CanExecute();
    public abstract void Execute(Action onActionComplete);
    public abstract void SetTarget(Vector3Int targetPos);
    public abstract int GetAmmoCost();

    public abstract int GetCost();

    public abstract bool IsSelectable();

    public abstract bool CanExecuteImmediately();

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;
    public void SetUp(Unit unit)
    {
        this.unit = unit;
    }
    public Unit GetUnit() => unit;
    public bool IsActive() => isActive;

    protected void StartAction(Action onActionComplete)
    {
        unit.animator.ResetTrigger(IDLE);
        this.onActionComplete = onActionComplete;
        isActive = true;
        StartCoroutine(ExecuteCoroutine());
        UIManager.instance.onActionChanged.Invoke();
    }

    protected void FinishAction()
    {
        switch (GetActionType())
        {
            case ActionType.Move:
                unit.animator.ResetTrigger(MOVE);
                break;
            case ActionType.Spin:
                // unit.animator.ResetTrigger(MOVE);
                break;
            case ActionType.Attack:
                unit.animator.ResetTrigger(SHOOT);
                break;
            case ActionType.Dynamite:
                break;
            case ActionType.Idle:
                break;
            case ActionType.Reload:
                unit.animator.ResetTrigger(RELOAD);
                break;
            case ActionType.Fanning:
                unit.animator.ResetTrigger(FANNING);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        unit.animator.SetTrigger(IDLE);
        
        isActive = false;
        onActionComplete();
        UIManager.instance.onActionChanged.Invoke();

        unit.onFinishAction.Invoke(this);
    }

    protected virtual IEnumerator ExecuteCoroutine()
    {
        yield return null; 
    }

    public virtual void ForceFinish()
    {
        StopCoroutine(ExecuteCoroutine());
        FinishAction();
    }
}