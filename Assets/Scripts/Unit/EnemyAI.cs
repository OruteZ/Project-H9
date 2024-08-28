using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KieranCoppins.DecisionTrees;
using PassiveSkill;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Action = KieranCoppins.DecisionTrees.Action;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private DecisionTree tree;
    
    [SerializeField]
    private Unit unit;
    
    [SerializeField]
    private int attackCnt = 0;
    
    [SerializeField]
    private int moveCnt = 0;
    
    public int AtkCount => attackCnt;
    public int MoveCount => moveCnt;
    
    public Vector3Int playerPosMemory;
    public Unit GetUnit()
    {
        return unit;
    }

    public void Setup(Unit unit, DecisionTree tree)
    {
        this.unit = unit;
        playerPosMemory = FieldSystem.unitSystem.GetPlayer().hexPosition;

        FieldSystem.unitSystem.onAnyUnitMoved.AddListener((u) =>
        {
            if (u == this.unit)
                ReloadPlayerPosMemory();

            else if (u is Player)
                ReloadPlayerPosMemory();
        });
        this.unit.onTurnStart.AddListener((n) =>
        {
            ReloadCounts();
        });
        this.unit.onFinishAction.AddListener(OnFinishAction);

        this.tree = Instantiate(tree);
        this.tree.Initialise(this);
    }
    
    /// <summary>
    /// AI가 Decision Tree를 통해서 판단을 진행 한 후 결과를 도출합니다.
    /// </summary>
    /// <returns></returns>
    public AIResult Think()
    {
        var result = tree.Root.MakeDecision();
        if (result is FinishTurn)
        {
            return new AIResult(null, Hex.none);
        }
        
        if(result == null)
        {
            Debug.LogError("result is null");
            return new AIResult(null, Hex.none);
        }

        if (result is not IAiResult ret)
        {
            Debug.LogError("result is not IAiResult");
            return new AIResult(null, Hex.none);
        }
        
        // StartCoroutine(ret.Execute());
        return ret.GetResult();
    }

    private void ReloadPlayerPosMemory()
    {
        Vector3Int playerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;

        if (playerPosMemory == playerPos)
        {
            return;
        }
        
        if (FieldSystem.tileSystem.VisionCheck(GetUnit().hexPosition, playerPos) is false)
        {
            return;
        }
        
        if(GetUnit().stat.sightRange < Hex.Distance(GetUnit().hexPosition, playerPos))
        {
            return;
        }
        
        playerPosMemory = playerPos;
    }

    private void ReloadCounts()
    {
        int ap = unit.stat.GetStat(StatType.CurActionPoint);
        bool hasInfShoot = unit.GetAllPassiveList().Any(p
            => p.GetEffectType().Any(e => e == PassiveEffectType.InfinityShootPoint)
            );
        bool hasDoubleShoot = unit.GetAllPassiveList().
            Any(p => p.GetEffectType().Any(e => e == PassiveEffectType.DoubleShootPoint)
            );
        
        int atkCost = unit.GetAction<AttackAction>().GetCost();
        int moveCost = unit.GetAction<MoveAction>().GetCost();

        if (hasInfShoot)
        {
            attackCnt = ap / atkCost;
        }
        else if (hasDoubleShoot)
        {
            attackCnt = Mathf.Clamp(ap / atkCost, 0, 2);
        }
        else
        {
            attackCnt = Mathf.Clamp(ap / atkCost, 0, 1);
        }

        int restAp = ap - attackCnt * atkCost;
        moveCnt = restAp / moveCost;
    }

    private void OnFinishAction(IUnitAction ac)
    {
        switch (ac)
        {
            case AttackAction:
                attackCnt--;
                break;
            case MoveAction:
                moveCnt--;
                break;
        }
    }
}

[Serializable]
public struct AIResult
{
    //constructor
    public AIResult(IUnitAction action, Vector3Int position)
    {
        this.action = action;
        this.position = position;
    }
    
    public IUnitAction action;
    public Vector3Int position;
}
