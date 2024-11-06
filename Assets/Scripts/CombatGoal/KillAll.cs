using UnityEngine;
using UnityEngine.Events;

public class KillAll : IGoal
{
    private UnityAction<bool> _onComplete;
    public UnityEvent OnInfoChanged { get; private set; }
    
    private int _initialEnemyCount;
    private int _turnLimit;

    private bool _finishFlag = false;
    
    public bool HasSuccess()
    {
        if (_finishFlag) return true;
        
        return _finishFlag = (FieldSystem.unitSystem.GetEnemyCount() == 0);
    }

    public bool IsFinished()
    {
        if (HasSuccess()) return true;
        
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
        FieldSystem.unitSystem.GetPlayer().onDead.AddListener(none => PlayerDead());
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
        if (FieldSystem.unitSystem.GetEnemyCount() == 0)
        {
            _onComplete.Invoke(true);
        }
    }

    private void PlayerDead()
    {
        _onComplete.Invoke(false);
    }
    
    private void OnTurnEnd()
    {
        OnInfoChanged.Invoke();
        
        if (IsFinished())
        {
            _onComplete.Invoke(false);
        }
    }
    
    #endregion
}