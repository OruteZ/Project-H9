using UnityEngine;

[CreateAssetMenu(fileName = "AIModel", menuName = "AI/HEINRICH AIModel", order = 1)]
public class HeinrichAIModel : AIModel
{
    /// <summary>
    /// AI가 Decision Tree를 통해서 판단을 진행 한 후 결과를 도출합니다.
    /// </summary>
    /// <returns></returns>
    public override AIResult CalculateAction(EnemyAI ai)
    {
        Unit unit = ai.GetUnit();
        ai.ReloadPlayerPosMemory();
        
        // 후보 Actions
        ReloadAction reloadAction = unit.GetAction<ReloadAction>();
        MoveAction moveAction = unit.GetAction<MoveAction>();
        AttackAction attackAction = unit.GetAction<AttackAction>();
        HeinrichTrapAction trapAction = unit.GetAction<HeinrichTrapAction>();
        HeinrichVanishAction vanishAction = unit.GetAction<HeinrichVanishAction>();
        
        // nullcheck
        if (reloadAction == null || 
            moveAction == null || 
            attackAction == null || 
            trapAction == null || 
            vanishAction == null
            )
        {
            Debug.LogError("Action is null");
            return new AIResult(null, unit.hexPosition);
        }
        
        // 0. Vainish 조건 만족시 바로 진행
        if (vanishAction.CanExecute())
        {
            return new AIResult(vanishAction, unit.hexPosition);
        }
        
        // 0. 트랩 설치 쿨 돌았음녀 바로 진행
        if (trapAction.CanExecute())
        {
            return new AIResult(trapAction, unit.hexPosition);
        }
        
        
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

    public override void Setup(EnemyAI ai)
    {
        
    }
}