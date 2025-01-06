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
                goal = new KillTargetEnemy();
                break;
            case GoalType.SURVIVE:
                goal = new Survive();
                break;
            case GoalType.MOVE_TO_POINT:
                goal = new MoveToPoint();
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

[Serializable]
public struct GoalInfo
{
    public GoalType goalType;
    
    public Vector3Int targetPosition;
    public int turnLimit;
    public int targetEnemy;
}