using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Enemy : Unit
{
    [Header("Index")]
    public int dataIndex;
    private EnemyAI _ai;

    protected override void Awake()
    {
        base.Awake();
        
        _ai = GetComponent<EnemyAI>();
    }

    public override void SetUp(string newName, UnitStat unitStat, Weapon weapon)
    {
        base.SetUp(newName, unitStat, weapon);
        
    }
    
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;

        _ai.SelectAction();

        activeUnitAction = _ai.resultAction;
        Vector3Int target = _ai.resultPosition;
        
        Debug.Log("Enemy Selected Action = " + activeUnitAction.GetActionType());

        if (activeUnitAction is IdleAction)
        {
            FieldSystem.turnSystem.EndTurn();
            return;
        }
        
        if (TryExecuteUnitAction(target, FinishAction))
        {
            SetBusy();
        }
        else
        {
            Debug.Log("AI가 실행할 수 없는 행동을 실행 중 : " + activeUnitAction.GetActionType());
        }
    }

    public override void StartTurn()
    {
        #if UNITY_EDITOR
        Debug.Log(unitName + " Turn Started");

        //StartCoroutine(UITestEndTurn());

        #endif
        currentActionPoint = stat.actionPoint;

        hasAttacked = false;
        activeUnitAction = null;
    }

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);
    }
    
    private void FinishAction()
    {
        ClearBusy();
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(currentActionPoint);
        
        if (currentActionPoint <= 0)
        {
            FieldSystem.turnSystem.EndTurn();
        }
    }
}
