using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIModel", menuName = "AI/BaseAIModel", order = 1)]
public class AIModel : ScriptableObject
{
    /// <summary>
    /// AI가 Decision Tree를 통해서 판단을 진행 한 후 결과를 도출합니다.
    /// </summary>
    /// <returns></returns>
    public virtual AIResult CalculateAction(EnemyAI ai)
    {
        Unit unit = ai.GetUnit();
        ai.ReloadPlayerPosMemory();
        
        // 후보 Actions
        ReloadAction reloadAction = unit.GetAction<ReloadAction>();
        MoveAction moveAction = unit.GetAction<MoveAction>();
        AttackAction attackAction = unit.GetAction<AttackAction>();
        
        
        // =============== 1. Reload ===============
        if (IsOutOfAmmo(ai) && reloadAction.CanExecute())
        {
            // returns reload action
            return new AIResult(reloadAction, unit.hexPosition);
        }
        
        // =============== 2. Out Of Sight : Move to Last Position ===============
        if (IsEnemyOutOfSight(ai))
        {
            // returns move action
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        // =============== 3. In Sight ================
        // 3 - 1. 사격하기에 너무 멀리 있을 경우. 가까이 이동
        if (unit.weapon.GetRange() < Hex.Distance(unit.hexPosition, ai.playerPosMemory))
        {
            // returns move action
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        // 3 - 2. 사격 가능한 상태일 경우, 사격 전에 여유 Cost를 보고 가능한 가까이 이동
        if (ai.MoveCount > 0 && Hex.Distance(unit.hexPosition, ai.playerPosMemory) > 1)
        {
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        return new AIResult(attackAction, ai.playerPosMemory);
    }

    public virtual void Setup(EnemyAI ai)
    {
        
    }
    
    protected bool IsOutOfAmmo(EnemyAI ai)
    {
        return ai.GetUnit().weapon.CurrentAmmo <= 0;
    }
    
    protected bool IsEnemyOutOfSight(EnemyAI ai)
    {
        return FieldSystem.unitSystem.GetPlayer().GetHex() != ai.playerPosMemory;
    }

    protected Vector3Int GetOneTileMove(Vector3Int start, Vector3Int dest)
    {
        List<Tile> path = FieldSystem.tileSystem.FindPath(start, dest);
        if (path.Count <= 1) return start;
        
        return path[1].hexPosition;
    }
    
    protected bool Available(IUnitAction action, Vector3Int target)
    {
        return  action.GetUnit() == FieldSystem.unitSystem.GetPlayer() &&
                action.IsSelectable() &&
                action.GetCost() <= FieldSystem.unitSystem.GetPlayer().stat.GetStat(StatType.CurActionPoint) &&
                action.CanExecute(target);
    }
}