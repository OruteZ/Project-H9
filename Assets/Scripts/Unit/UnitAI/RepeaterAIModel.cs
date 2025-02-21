using System.Collections.Generic;
using UnityEngine;

public class RepeaterAIModel : AIModel
{
    public override AIResult CalculateAction(EnemyAI ai)
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
        
        // 3 - 2. 사격 가능한 상태일 경우, 사격 전에 여유 Cost를 보고 가능한 Sweet Spot에 가까워지도록 이동
        if (ai.MoveCount > 0)
        { 
            // if(Hex.Distance(unit.hexPosition, ai.playerPosMemory) > 1)
            // {
            //     return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
            // }
            int sweetSpotDist = ((Repeater)unit.weapon).GetSweetSpot();

            // 1. Sweet Spot보다 멀리 있을 경우 
            if (Hex.Distance(unit.hexPosition, ai.playerPosMemory) > sweetSpotDist)
            {
                return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
            }
            
            // 2. Sweet Spot보다 가까이 있을경우, 플레이어와 일직선상에서 한칸 뒤로 가는 방향으로 이동
            Vector3Int moveDir = ai.playerPosMemory - unit.hexPosition;
            Vector3Int targetTile = unit.hexPosition + moveDir;
            List<Vector3Int> line = Hex.DrawLine1(unit.hexPosition, targetTile);

            targetTile = line[1];
            
            return new AIResult(moveAction, targetTile);
        }
        
        return new AIResult(attackAction, ai.playerPosMemory);
    }
}