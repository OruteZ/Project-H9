using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KieranCoppins.DecisionTrees;
using PassiveSkill;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;
using Action = KieranCoppins.DecisionTrees.Action;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private H9DecisionTree _tree;
    
    [SerializeField]
    private Unit _unit;
    
    [SerializeField]
    private int attackCnt = 0;
    
    [SerializeField]
    private int moveCnt = 0;
    
    public int AtkCount => attackCnt;
    public int MoveCount => moveCnt;
    
    public Vector3Int playerPosMemory;

    public void Awake()
    {
        _tree.Initialise(this);
    }

    public void Start()
    {
        Think();
    }

    public Unit GetUnit()
    {
        return _unit;
    }

    public void Think()
    {
        Debug.Log("run");
        Action result = _tree.Run();
        
        Debug.Log("result : " + result);
        StartCoroutine(result.Execute());
    }
    
    public bool GetPlayerPosMemory(out Vector3Int result)
    {
        ref Vector3Int playerMemory = ref playerPosMemory;
        Vector3Int playerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;

        if (playerMemory == playerPos)
        {
            result = playerMemory;
            return true;
        }
        if (FieldSystem.tileSystem.VisionCheck(GetUnit().hexPosition, playerPos))
        {
            playerMemory = playerPos;
            result = playerMemory;

            return true;
        }
        if(GetUnit().stat.sightRange < Hex.Distance(GetUnit().hexPosition, playerPos))
        {
            playerMemory = playerPos;
            result = playerMemory;
            
            return true;
        }

        result = Hex.none;
        return false;
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
