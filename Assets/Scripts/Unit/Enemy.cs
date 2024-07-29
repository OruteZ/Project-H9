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
    
    [SerializeField] EnemyAI ai;
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int START_TURN = Animator.StringToHash("StartTurn");

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetUp(int index, string newName, UnitStat unitStat, Weapon weapon, GameObject unitModel, List<Passive> passiveList)
    {
        base.SetUp(index, newName, unitStat, weapon, unitModel, passiveList);
    }

    public void SetupAI(DecisionTree tree)
    {
        if (ai is null)
        {
            Debug.LogError("Ai is null");
            
            #if UNITY_EDITOR
            EditorApplication.isPaused = true;
            #else
            throw new System.Exception("Ai is null");
            #endif
        }
    
        ai.Setup(this, tree);
    }
    
    public void Update()
    {
        if (IsBusy()) return;
        if (!IsMyTurn()) return;
        if (FieldSystem.unitSystem.IsCombatFinish(out var none)) return;
        
        var result = ai.Think();
        if (result.action is null)
        {
            //end turn
            FieldSystem.turnSystem.EndTurn();
            return;
        }

        if (TrySelectAction(result.action) && TryExecute(result.position))
        {
            //do nothing : Success Action
        }
        else
        {
            FieldSystem.turnSystem.EndTurn();
        }
    }

    public override void TakeDamage(int damage, Unit attacker = null,  Damage.Type type = Damage.Type.Default)
    {
        base.TakeDamage(damage, attacker, type);
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
