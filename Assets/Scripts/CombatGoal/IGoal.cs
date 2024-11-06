using UnityEngine;
using UnityEngine.Events;

public interface IGoal
{
    const int INFINITE = -1;
    
    UnityEvent OnInfoChanged { get; }
    bool HasSuccess();
    bool IsFinished();
    int GetStringIndex();
    string GetProgressString();
    void Setup(Vector3Int targetPos, int turnLimit, int targetEnemy);
    void AddListenerOnComplete(UnityAction<bool> onComplete);
}

public enum GoalType{
    KILL_ALL_ENEMIES,
    KILL_TARGET_ENEMY,
    SURVIVE,
    MOVE_TO_POINT,
}
