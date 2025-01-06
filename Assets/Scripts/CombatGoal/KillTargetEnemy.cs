using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Ư�� ���� ���̴� Goal�Դϴ�. targetIndex�� ���� ��� ���� ����ȴٸ� �����մϴ�.
/// </summary>
public class KillTargetEnemy : IGoal
{
    public UnityEvent OnInfoChanged { get; }
    
    private UnityAction<bool> _onComplete = null;
    private int _turnLimit;
    private int _targetEnemy;
    private int _targetEnemyStartCount;
    
    private int GetTargetEnemyCount()
    {
        return FieldSystem.unitSystem.units.FindAll(unit =>
        {
            if (unit is Enemy e)
            {
                return e.dataIndex == _targetEnemy;
            }

            return false;
        }).Count;
    }
    
    private void PlayerDead(Unit player)
    {
        _onComplete?.Invoke(false);
    }
    
    public bool HasSuccess()
    {
        return GetTargetEnemyCount() == 0;
    }

    public bool IsFinished()
    {
        if(HasSuccess()) return true;
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
        // todo : �� ��򰡿��� ���� ��ǥ string�� �ְ���?
        return -132;
    }

    public string GetProgressString()
    {
        int currentEnemyCount = GetTargetEnemyCount();
        
        return $" {currentEnemyCount} / {_targetEnemyStartCount} ";
    }

    public void Setup(Vector3Int targetPos, int turnLimit, int targetEnemy)
    {
        _turnLimit = turnLimit;
        _targetEnemy = targetEnemy;
        
        _onComplete = null;
        
        // setup event
        FieldSystem.unitSystem.GetPlayer().onDead.AddListener(PlayerDead);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnEnd);
        FieldSystem.unitSystem.onAnyUnitDead.AddListener(CheckGoal);
        
        _targetEnemyStartCount = GetTargetEnemyCount();
    }

    private void CheckGoal(Unit unit)
    {
        if (unit is not Enemy e) return;
        if (HasSuccess())
        {
            _onComplete?.Invoke(true);
        }
    }

    private void OnTurnEnd()
    {
        if (IsFinished())
        {
            _onComplete?.Invoke(HasSuccess());
        }
    }

    public void AddListenerOnComplete(UnityAction<bool> onComplete)
    {
        _onComplete = onComplete;
    }
}