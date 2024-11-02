using UnityEngine;
using UnityEngine.Events;

public class KillAll : IGoal
{
    private UnityAction<bool> _onComplete;
    
    private int _initialEnemyCount;

    private bool _finishFlag = false;
    
    public bool HasSuccess()
    {
        if (_finishFlag) return true;
        
        return _finishFlag = (FieldSystem.unitSystem.GetEnemyCount() == 0);
    }

    public bool IsFinished()
    {
        return !HasSuccess();
    }

    public int GetStringIndex()
    {
        return -131;
    }

    public string GetGoalString()
    {
        int currentEnemyCount = FieldSystem.unitSystem.GetEnemyCount();
        
        return $" {currentEnemyCount} / {_initialEnemyCount} ";
    }

    public void Setup(Vector3Int targetPos, int turnLimit, int targetEnemy)
    {
        _onComplete = null;
        FieldSystem.unitSystem.onAnyUnitDead.AddListener(CheckGoal);
        FieldSystem.unitSystem.GetPlayer().onDead.AddListener(none => PlayerDead());
        
        _initialEnemyCount = FieldSystem.unitSystem.GetEnemyCount();
    }

    public void AddListenerOnComplete(UnityAction<bool> onComplete)
    {
        _onComplete += onComplete;
    }
    
    #region PRIVATE
    
    private void CheckGoal(Unit none)
    {
        if (FieldSystem.unitSystem.GetEnemyCount() == 0)
        {
            _onComplete.Invoke(true);
        }
    }

    private void PlayerDead()
    {
        _onComplete.Invoke(false);
    }
    
    #endregion
}