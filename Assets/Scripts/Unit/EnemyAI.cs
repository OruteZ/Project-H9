using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KieranCoppins.DecisionTrees;
using PassiveSkill;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Action = KieranCoppins.DecisionTrees.Action;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private DecisionTree _tree;
    
    [SerializeField]
    private Unit _unit;
    
    [SerializeField]
    private int attackCnt = 0;
    
    [SerializeField]
    private int moveCnt = 0;
    
    public int AtkCount => attackCnt;
    public int MoveCount => moveCnt;
    
    public Vector3Int playerPosMemory;
    public Unit GetUnit()
    {
        return _unit;
    }

    public void Setup(Unit unit, DecisionTree tree)
    {
        _unit = unit;
        playerPosMemory = FieldSystem.unitSystem.GetPlayer().hexPosition;

        FieldSystem.unitSystem.onAnyUnitMoved.AddListener((u) =>
        {
            if (u == _unit)
                ReloadPlayerPosMemory();

            else if (u is Player)
                ReloadPlayerPosMemory();
        });

        _tree = Instantiate(tree);
        _tree.Initialise(this);
    }
    
    /// <summary>
    /// AI가 Decision Tree를 통해서 판단을 진행 한 후 결과를 도출합니다.
    /// </summary>
    /// <returns></returns>
    public AIResult Think()
    {
        var result = _tree.Root.MakeDecision();
        if (result is FinishTurn)
        {
            return new AIResult(null, Hex.none);
        }
        
        if(result == null)
        {
            Debug.LogError("result is null");
            return new AIResult(null, Hex.none);
        }
        
        var ret = result as ExecuteAction;
        
        StartCoroutine(ret.Execute());
        return ret.GetResult();
    }
    
    public void ReloadPlayerPosMemory()
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

    public void ReloadCounts()
    {
        int cost = _unit.stat.GetStat(StatType.CurActionPoint);
        bool hasInfShoot = _unit.GetAllPassiveList().Any(p
            => p.GetEffectType().Any(e => e == PassiveEffectType.InfinityShootPoint)
            );
        bool hasDoubleShoot = _unit.GetAllPassiveList().
            Any(p => p.GetEffectType().Any(e => e == PassiveEffectType.DoubleShootPoint)
            );

        if (hasInfShoot)
        {
            attackCnt = cost;
            moveCnt = 0;
        }
        
        else if (hasDoubleShoot)
        {
            attackCnt = 2;
            moveCnt = cost - 2;
        }
        
        else
        {
            attackCnt = 1;
            moveCnt = cost - 1;
        }

        _unit.onFinishAction.AddListener(OnFinishAction);
        _unit.onTurnEnd.AddListener((n) =>
        {
            _unit.onFinishAction.RemoveListener(OnFinishAction);
        });
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
