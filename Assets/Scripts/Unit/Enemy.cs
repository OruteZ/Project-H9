using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using KieranCoppins.DecisionTrees;
using PassiveSkill;
using UnityEditor;
using UnityEngine;

public class Enemy : Unit
{
    [Header("Index")]
    public int dataIndex;
    
    [SerializeField] private EnemyAI ai;
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int START_TURN = Animator.StringToHash("StartTurn");

    protected override void Awake()
    {
        base.Awake();

        if (ai == null && TryGetComponent(out ai) is false)
        {
            ai = gameObject.AddComponent<EnemyAI>();
        }
    }

    public override void SetUp(int index, string newName, UnitStat unitStat, Weapon weapon, GameObject unitModel, List<Passive> passiveList)
    {
        base.SetUp(index, newName, unitStat, weapon, unitModel, passiveList);
    }

    public void SetupAI(DecisionTree tree)
    {
        // null check : AI
        if (ai == null)
        {
            Debug.LogError("Enemy Ai Component is null");
            
            #if UNITY_EDITOR
            EditorApplication.isPaused = true;
            #else
            throw new System.Exception("Ai is null");
            #endif
        }

        // null check : Decision Tree
        if (tree == null)
        {
            Debug.LogError("Decision Tree is null");
            
            #if UNITY_EDITOR
            EditorApplication.isPaused = true;
            #else
            throw new System.Exception("Decision Tree is null");
            #endif
        }

        if (ai != null) ai.Setup(this, tree);
    }
    
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (FieldSystem.IsCombatFinish(out bool none)) return;
        
        var result = ai.Think();
        if (result.action is null)
        {
            //end turn
            EndTurn();
            return;
        }

        if (TrySelectAction(result.action) && TryExecute(result.position))
        {
            //do nothing : Success Action
        }
        else
        {
            EndTurn();
        }
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        UIManager.instance.onActionChanged.Invoke();
    }
    
    public new void FinishAction()
    {
        ClearBusy();
        ConsumeCost(activeUnitAction.GetCost());
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
        // Debug.Log("Select Action : " + action);
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

    public EnemyAI GetAI() => ai;
}
