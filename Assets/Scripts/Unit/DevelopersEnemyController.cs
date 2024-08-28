using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class DevelopersEnemyController : MonoBehaviour
{
    private Enemy _enemy;
    
    [SerializeField] private IUnitAction _currentAction;
    [SerializeField] private Vector3Int _targetPos;
    
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }
    
    [ContextMenu("Execute Action")]
    public void ExecuteAction()
    {
        if (_currentAction is null) return;

        _enemy.TrySelectAction(_currentAction);
        _enemy.TryExecute(_targetPos);
    }
    
    [ContextMenu("End Turn")]
    public void EndTurn()
    {
        _enemy.EndTurn();
    }
    
}