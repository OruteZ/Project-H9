using UnityEngine;
using UnityEngine.Events;

public interface IGoal
{
    bool HasSuccess();
    bool IsFinished();
    
    int GetStringIndex();
    string GetGoalString();
    void Setup(Vector3Int targetPos, int turnLimit, int targetEnemy);
    
    void AddListenerOnComplete(UnityAction<bool> onComplete);
}

public enum GoalType{
    KILL_ALL_ENEMIES,
    KILL_BOSS,
    SURVIVE,
    MOVE_TO_POINT,
}
