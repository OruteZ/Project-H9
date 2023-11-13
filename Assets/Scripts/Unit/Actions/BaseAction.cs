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
    protected static readonly int DYNAMITE = Animator.StringToHash("Dynamite");
    
    public abstract ActionType GetActionType();
    public abstract bool CanExecute();

    public void Execute()
    {
        IEnumerator Coroutine()
        {
            yield return ExecuteCoroutine();
            FinishAction();
        }
        
        StartAction();
        StartCoroutine(Coroutine());
    }
    public abstract void SetTarget(Vector3Int targetPos);
    public abstract int GetAmmoCost();

    public abstract int GetCost();

    public abstract bool IsSelectable();

    public abstract bool CanExecuteImmediately();

    protected Unit unit;
    protected bool isActive;
    public void SetUp(Unit unit)
    {
        this.unit = unit;
    }
    public Unit GetUnit() => unit;
    public bool IsActive() => isActive;

    private void StartAction()
    {
        unit.animator.ResetTrigger(IDLE);
        isActive = true;
        UIManager.instance.onActionChanged.Invoke();
    }

    private void FinishAction()
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
                unit.animator.ResetTrigger(DYNAMITE);
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
        UIManager.instance.onActionChanged.Invoke();
        unit.FinishAction();
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

    public virtual void SetAmount(float[] amounts)
    { }
}