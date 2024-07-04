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
    protected static readonly int FANNING_FIRE = Animator.StringToHash("FanningFire");
    protected static readonly int FANNING_FINISH = Animator.StringToHash("FanningFinish");
    protected static readonly int RELOAD = Animator.StringToHash("Reload");
    protected static readonly int DYNAMITE = Animator.StringToHash("Dynamite");

    [SerializeField] protected int cost;
    [SerializeField] protected int ammoCost;
    [SerializeField] protected int range;
    [SerializeField] protected int radius;
    [SerializeField] protected int damage;
    
    public void SetData(ActiveInfo info)
    {
        cost = info.cost;
        ammoCost = info.ammoCost;
        range = info.range;
        radius = info.radius;
        damage = info.damage;
        
        SetAmount(info.amounts);
    }

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
    public virtual int GetAmmoCost() => ammoCost;

    public virtual int GetCost() => cost;

    public abstract bool IsSelectable();

    public abstract bool CanExecuteImmediately();

    protected Unit unit;
    protected bool isActive;
    public virtual void SetUp(Unit unit)
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
            case ActionType.None:
                break;
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
            case ActionType.Hemostasis:
                break;
            case ActionType.ItemUsing:
                break;
            case ActionType.Cover:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        unit.animator.SetTrigger(IDLE);
        
        isActive = false;
        unit.FinishAction();
        UIManager.instance.onActionChanged.Invoke();
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

    protected virtual void SetAmount(float[] amounts)
    { }
    
    public virtual void TossAnimationEvent(string eventString)
    { }
}