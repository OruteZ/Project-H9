using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseAction : MonoBehaviour, IUnitAction
{
    public abstract ActionType GetActionType();
    public abstract bool CanExecute();
    public abstract void Execute(Action onActionComplete);
    public abstract void SetTarget(Vector3Int targetPos);
    public abstract int GetAmmoCost();

    public abstract int GetCost();

    public abstract bool IsSelectable();

    public abstract bool CanExecuteImmediately();

    public UnityEvent onActionStarted;
    public UnityEvent onActionFinished;

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;
    public void SetUp(Unit unit)
    {
        this.unit = unit;
        onActionStarted.AddListener(() => UIManager.instance.combatUI.SetCombatUI());
        onActionFinished.AddListener(() => UIManager.instance.combatUI.SetCombatUI());
    }
    public Unit GetUnit() => unit;
    public bool IsActive() => isActive;

    protected void StartAction(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        isActive = true;
        
        onActionStarted.Invoke();
    }

    protected void FinishAction()
    {
        isActive = false;
        onActionComplete();

        onActionFinished.Invoke();
    }
}