using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PassiveSkill;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    AIModel model;
    
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
        Debug.Log("AI : GetUnit, instanceID : " + name + ", unit : " + unit.name);
        
        return unit;
    }

    public void Setup(Unit unit, AIModel tree)
    {
        this.unit = unit;
        this.model = tree;
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

        tree.Setup(this);
    }
    
    /// <summary>
    /// AI가 Decision Tree를 통해서 판단을 진행 한 후 결과를 도출합니다.
    /// </summary>
    /// <returns></returns>
    public AIResult Think()
    {
        return model.CalculateAction(this);
    }

    public void ReloadPlayerPosMemory()
    {
        Vector3Int playerPos = FieldSystem.unitSystem.GetPlayer().hexPosition;

        if (playerPosMemory == playerPos)
        {
            return;
        }
        
        if (FieldSystem.tileSystem.VisionCheck(GetUnit().hexPosition, playerPos, lookInside:true) is false)
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
