using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Enemy : Unit
{
    [Header("Index")]
    public int dataIndex;
    private EnemyAI _ai;
    private static readonly int IDLE = Animator.StringToHash("Idle");

    protected override void Awake()
    {
        base.Awake();
        
        _ai = GetComponent<EnemyAI>();
    }

    public override void SetUp(string newName, UnitStat unitStat, Weapon weapon, GameObject unitModel)
    {
        base.SetUp(newName, unitStat, weapon, unitModel);
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
            Debug.LogError("AI가 실행할 수 없는 행동을 실행 중 : " + activeUnitAction.GetActionType());
            FieldSystem.turnSystem.EndTurn();
            return;
        }
    }

    public override void StartTurn()
    {
        #if UNITY_EDITOR
        Debug.Log(unitName + " Turn Started");

        //StartCoroutine(UITestEndTurn());

        #endif
        animator.SetTrigger(IDLE);
        currentActionPoint = stat.actionPoint;

        hasAttacked = false;
        activeUnitAction = GetAction<IdleAction>();
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
