using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Enemy : Unit
{
    private EnemyAI _ai;

    protected override void Awake()
    {
        base.Awake();
        
        _ai = GetComponent<EnemyAI>();
    }

    public override void SetUp(string newName, UnitStat unitStat, int weaponIndex)
    {
        base.SetUp(newName, unitStat, weaponIndex);
    }
    
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;

        activeUnitAction = _ai.SelectAction(out var target);
        if (activeUnitAction is NoneAction) FinishAction();
        
        if (TryExecuteUnitAction(target, FinishAction))
        {
            SetBusy();
        }

        // activeUnitAction.SetTarget(target.position);
        // if (activeUnitAction.CanExecute() && currentActionPoint >= activeUnitAction.GetCost())
        // {
        //     #if UNITY_EDITOR
        //     Debug.Log("Active Action of " + gameObject + " = " + activeUnitAction.GetActionType());
        //     #endif 
        //     isBusy = true;
        //     activeUnitAction.Execute(FinishAction);
        // }
    }

    public override void StartTurn()
    {
        #if UNITY_EDITOR
        Debug.Log(unitName + " Turn Started");
        #endif
        currentActionPoint = stat.actionPoint;

        activeUnitAction = null;
    }

    public override void GetDamage(int damage)
    {
        Debug.Log(damage + " 데미지 받음");
    }
    
    private void FinishAction()
    {
        ClearBusy();
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(currentActionPoint);
        
        if (currentActionPoint == 0)
        {
            FieldSystem.turnSystem.EndTurn();
        }
    }
}
