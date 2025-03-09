using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JacksonAIModel", menuName = "AI/JacksonAIModel", order = 1)]
public class JacksonAIModel : AIModel
{
    
    [SerializeField, Space(10)] 
    private int grabCooldown;
    [SerializeField] private int curGrabCooldown;
    
    public override AIResult CalculateAction(EnemyAI ai)
    {
        Unit unit = ai.GetUnit();
        ai.ReloadPlayerPosMemory();
        
        // Get Actions
        ReloadAction reloadAction = unit.GetAction<ReloadAction>();
        MoveAction moveAction = unit.GetAction<MoveAction>();
        AttackAction attackAction = unit.GetAction<AttackAction>();
        SickleAttackAction sickleAction = unit.GetAction<SickleAttackAction>();
        SickleGrabAction sickleGrabAction = unit.GetAction<SickleGrabAction>();
        
        // each null check
        if (reloadAction == null || moveAction == null || attackAction == null || sickleAction == null || sickleGrabAction == null)
        {
            Debug.LogError("Action is null");
            return new AIResult(null, unit.hexPosition);
        }
        
        
        // === 0. Sickle Grab ===
        
        if (curGrabCooldown <= 0 && sickleGrabAction.CanExecute(ai.playerPosMemory))
        {
            curGrabCooldown = grabCooldown;
            return new AIResult(sickleGrabAction, ai.playerPosMemory);
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

        // =============== 4. Attack. If close enough, use sickle atk ===============
        return sickleAction.CanExecute(ai.playerPosMemory) ? 
            new AIResult(sickleAction, ai.playerPosMemory) : new AIResult(attackAction, ai.playerPosMemory);
    }
    
    public override void Setup(EnemyAI ai)
    {
        ai.GetUnit().onTurnEnd.AddListener(OnTurnEnd);
    }

    private void OnTurnEnd(Unit unit)
    {
        // 모든 cooldown 1 감
        curGrabCooldown = Mathf.Max(0, curGrabCooldown - 1);
    }
}