using UnityEngine;

[CreateAssetMenu(fileName = "AIModel", menuName = "AI/HEINRICH AIModel", order = 1)]
public class HeinrichAIModel : AIModel
{
    /// <summary>
    /// AI�� Decision Tree�� ���ؼ� �Ǵ��� ���� �� �� ����� �����մϴ�.
    /// </summary>
    /// <returns></returns>
    public override AIResult CalculateAction(EnemyAI ai)
    {
        Unit unit = ai.GetUnit();
        ai.ReloadPlayerPosMemory();
        
        // �ĺ� Actions
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
        
        // 0. Vainish ���� ������ �ٷ� ����
        if (vanishAction.CanExecute())
        {
            return new AIResult(vanishAction, unit.hexPosition);
        }
        
        // 0. Ʈ�� ��ġ �� �������� �ٷ� ����
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
        // 3 - 1. ����ϱ⿡ �ʹ� �ָ� ���� ���. ������ �̵�
        if (unit.weapon.GetRange() < Hex.Distance(unit.hexPosition, ai.playerPosMemory))
        {
            // returns move action
            return new AIResult(moveAction, GetOneTileMove(unit.hexPosition, ai.playerPosMemory));
        }
        
        // 3 - 2. ��� ������ ������ ���, ��� ���� ���� Cost�� ���� ������ ������ �̵�
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