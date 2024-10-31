using UnityEngine.Events;

public interface IGoal
{
    bool HasSuccess();
    bool IsFinished();
    
    int GetStringIndex();
    string GetGoalString();
    void Setup();
    
    void AddListenerOnComplete(UnityAction<bool> onComplete);
}

public enum GoalType{
    KILL_ALL_ENEMIES,
    KILL_BOSS,
    SURVIVE,
    MOVE_TO_POINT,
}
