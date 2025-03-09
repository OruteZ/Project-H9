using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RepeaterAIModel", menuName = "AI/RepeaterAIModel", order = 1)]
public class RepeaterAIModel : AIModel
{
    public override AIResult CalculateAction(EnemyAI ai)
    {
        Unit unit = ai.GetUnit();
        ai.ReloadPlayerPosMemory();
        
        // �ĺ� Actions
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
        // 3 - 1. ����ϱ⿡ �ʹ� �ָ� ���� ���. ������ �̵�
        if (unit.weapon.GetRange() < Hex.Distance(unit.hexPosition, ai.playerPosMemory))
        {
            // returns move action
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        // 3 - 2. ��� ������ ������ ���, ��� ���� ���� Cost�� ���� ������ Sweet Spot�� ����������� �̵�
        if (ai.MoveCount > 0)
        { 
            // if(Hex.Distance(unit.hexPosition, ai.playerPosMemory) > 1)
            // {
            //     return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
            // }
            int sweetSpotDist = ((Repeater)unit.weapon).GetSweetSpot();

            // 1. Sweet Spot���� �ָ� ���� ��� 
            if (Hex.Distance(unit.hexPosition, ai.playerPosMemory) > sweetSpotDist)
            {
                return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
            }
            
            // 2. Sweet Spot���� ������ �������, �÷��̾�� �������󿡼� ��ĭ �ڷ� ���� �������� �̵�
            Vector3Int moveDir = ai.playerPosMemory - unit.hexPosition;
            Vector3Int targetTile = unit.hexPosition + moveDir;
            List<Vector3Int> line = Hex.DrawLine1(unit.hexPosition, targetTile);

            targetTile = line[1];
            
            // target tile movable 
            if(unit.GetAction<MoveAction>().CanExecute(targetTile))
            {
                return new AIResult(moveAction, targetTile);
            }
        }
        
        // =============== 4. Attack ===============
        return new AIResult(attackAction, ai.playerPosMemory);
    }
}