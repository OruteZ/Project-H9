using System;
using CombatGoal;
using UnityEngine;

public static class GoalBuilder
{
    public static IGoal BuildGoal(GoalInfo goalInfo)
    {
        IGoal goal = null;
        
        switch (goalInfo.goalType)
        {
            case GoalType.KILL_ALL_ENEMIES:
                goal = new KillAll();
                break;
            case GoalType.KILL_TARGET_ENEMY:
                Debug.LogError($"아직 미구현 Goal인 {goalInfo.goalType}입니다. " +
                               $"기본값인 Kill ALl로 설정됩니다.");
                goal = new KillAll();
                break;
            case GoalType.SURVIVE:
                goal = new Survive();
                break;
            case GoalType.MOVE_TO_POINT:
                Debug.LogError($"아직 미구현 Goal인 {goalInfo.goalType}입니다. " +
                               $"기본값인 Kill ALl로 설정됩니다.");
                goal = new KillAll();
                break;
            default:
                goal = new KillAll();
                break;
        }
        
        //setup
        goal.Setup(goalInfo.targetPosition, goalInfo.turnLimit, goalInfo.targetEnemy);
        return goal;
    }
}

[System.Serializable]
public struct GoalInfo
{
    public GoalType goalType;
    
    public Vector3Int targetPosition;
    public int turnLimit;
    public int targetEnemy;
}