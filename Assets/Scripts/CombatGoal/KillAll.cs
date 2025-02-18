using UnityEngine;
using UnityEngine.Events;

public class KillAll : IGoal
{
    private UnityAction<bool> _onComplete;
    public UnityEvent OnInfoChanged { get; private set; }
    
    private int _initialEnemyCount;
    private int _turnLimit;

    private bool _finishFlag = false;
    private bool _completeActionInvoked = false;
    
    public bool HasSuccess()
    {
        if (_finishFlag) return true;
        
        return _finishFlag = (FieldSystem.unitSystem.GetEnemyCount() == 0);
    }

    public bool IsFinished()
    {
        if (HasSuccess()) return true;
        
        
        if (FieldSystem.unitSystem.GetPlayer() == null)
        {
            return true;
        }
        
        if (FieldSystem.unitSystem.GetPlayer().HasDead())
        {
            return true;
        }

        if (_turnLimit != IGoal.INFINITE && FieldSystem.turnSystem.GetTurnNumber() >= _turnLimit)
        {
            return true;
        }

        return false;
    }


    public int GetStringIndex()
    {
        return -131;
    }

    public string GetProgressString()
    {
        int currentEnemyCount = FieldSystem.unitSystem.GetEnemyCount();
        
        return $" {currentEnemyCount} / {_initialEnemyCount} ";
    }

    public void Setup(Vector3Int targetPos, int turnLimit, int targetEnemy)
    {
        _onComplete = null;
        OnInfoChanged = new UnityEvent();
        
        FieldSystem.unitSystem.onAnyUnitDead.AddListener(CheckGoal);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
        
        _initialEnemyCount = FieldSystem.unitSystem.GetEnemyCount();
        _turnLimit = turnLimit;
    }

    public void AddListenerOnComplete(UnityAction<bool> onComplete)
    {
        _onComplete += onComplete;
    }
    
    #region PRIVATE
    
    private void CheckGoal(Unit none)
    {
        Debug.Log("CheckGoal");
        
        if (_completeActionInvoked) return;
        
        if (none == FieldSystem.unitSystem.GetPlayer())
        {
            PlayerDead();
        }
        else
        {
            if (HasSuccess())
            {
                _onComplete.Invoke(true);
                _completeActionInvoked = true;
            }
        }
    }

    private void PlayerDead()
    {
        Debug.Log("PlayerDead");
        if (_completeActionInvoked) return;
        
        _onComplete.Invoke(false);
        _completeActionInvoked = true;
    }
    
    private void OnTurnEnd()
    {
        
        OnInfoChanged.Invoke();
    }
    
    #endregion
}