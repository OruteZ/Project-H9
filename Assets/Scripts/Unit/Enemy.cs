using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using PassiveSkill;
using UnityEditor;
using UnityEngine;

public class Enemy : Unit
{
    [Header("Index")]
    public int dataIndex;
    [SerializeField] private BehaviourTree ai;
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int START_TURN = Animator.StringToHash("StartTurn");

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetUp(string newName, UnitStat unitStat, Weapon weapon, GameObject unitModel, List<Passive> passiveList)
    {
        base.SetUp(newName, unitStat, weapon, unitModel, passiveList);
    }

    public void SetupAI(BehaviourTree ai)
    {
        if (ai is null)
        {
            Debug.LogError("Ai is null");
            EditorApplication.isPaused = true;
        }

        this.ai = ai;
        this.ai.Setup();
    }
    
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (FieldSystem.unitSystem.IsCombatFinish(out var none)) return;

        ai.Operate(this);

        // activeUnitAction = _ai.resultAction;
        // Vector3Int target = _ai.resultPosition;
        //
        // Debug.Log("AI Selected Action = " + activeUnitAction.GetActionType());
        //
        // if (activeUnitAction is IdleAction)
        // {
        //     animator.Play(IDLE);
        //     FieldSystem.turnSystem.EndTurn();
        //     return;
        // }
        //
        // if (TryExecuteUnitAction(target, FinishAction))
        // {
        //     SetBusy();
        // }
        // else
        // {
        //     Debug.LogError("AI가 실행할 수 없는 행동을 실행 중 : " + activeUnitAction.GetActionType());
        //     animator.SetTrigger(IDLE);
        //     FieldSystem.turnSystem.EndTurn();
        // }
    }

    public override void StartTurn()
    {
        #if UNITY_EDITOR
        Debug.Log(unitName + " Turn Started");

        //StartCoroutine(UITestEndTurn());

        #endif
        animator.SetTrigger(IDLE);
        animator.SetTrigger(START_TURN);
        stat.Recover(StatType.CurActionPoint, stat.maxActionPoint);

        hasAttacked = false;
        activeUnitAction = GetAction<IdleAction>();
    }

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);
        UIManager.instance.onActionChanged.Invoke();
    }
    
    public new void FinishAction()
    {
        ClearBusy();
        ConsumeCost(activeUnitAction.GetCost());
        // if (currentActionPoint <= 0)
        // {
        //     FieldSystem.turnSystem.EndTurn();
        // }
    }

    public bool TrySelectAction(IUnitAction action)
    {
        if (action is null) return false;
        if (action.IsSelectable() is false) return false;
        if (action.GetCost() > currentActionPoint)
        {  
            Debug.Log("Cost is loss, Cost is " + action.GetCost());
            return false;
        }
#if UNITY_EDITOR
        Debug.Log("Select Action : " + action);
#endif

        activeUnitAction = action;
        onSelectedChanged.Invoke();
        return true;
    }

    public bool TryExecute(Vector3Int target)
    {
        if (TryExecuteUnitAction(target))
        {
            SetBusy();
            return true;
        }
        else
        {
            return false;
        }
    }

    public BehaviourTree GetAI() => ai;
}
