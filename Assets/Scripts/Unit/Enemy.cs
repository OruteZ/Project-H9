using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using PassiveSkill;
using UnityEngine;

public class Enemy : Unit
{
    [Header("Index")]
    public int dataIndex;
    private EnemyAI _ai;
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int START_TURN = Animator.StringToHash("StartTurn");

    protected override void Awake()
    {
        base.Awake();
        
        _ai = GetComponent<EnemyAI>();
    }

    public override void SetUp(string newName, UnitStat unitStat, Weapon weapon, GameObject unitModel, List<Passive> passiveList)
    {
        base.SetUp(newName, unitStat, weapon, unitModel, passiveList);
    }
    
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (FieldSystem.unitSystem.IsCombatFinish(out var none)) return;

        _ai.SelectAction();

        activeUnitAction = _ai.resultAction;
        Vector3Int target = _ai.resultPosition;
        
        Debug.Log("AI Selected Action = " + activeUnitAction.GetActionType());

        if (activeUnitAction is IdleAction)
        {
            animator.Play(IDLE);
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
            animator.SetTrigger(IDLE);
            FieldSystem.turnSystem.EndTurn();
        }
    }

    public override void StartTurn()
    {
        #if UNITY_EDITOR
        Debug.Log(unitName + " Turn Started");

        //StartCoroutine(UITestEndTurn());

        #endif
        animator.SetTrigger(IDLE);
        animator.SetTrigger(START_TURN);
        currentActionPoint = originalstat.actionPoint;

        hasAttacked = false;
        activeUnitAction = GetAction<IdleAction>();
    }

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);
        UIManager.instance.onActionChanged.Invoke();
    }
    
    private void FinishAction()
    {
        int beforeCost = currentActionPoint;
        
        ClearBusy();
        currentActionPoint -= activeUnitAction.GetCost();
        onCostChanged.Invoke(beforeCost, currentActionPoint);
        
        // if (currentActionPoint <= 0)
        // {
        //     FieldSystem.turnSystem.EndTurn();
        // }
    }
}
