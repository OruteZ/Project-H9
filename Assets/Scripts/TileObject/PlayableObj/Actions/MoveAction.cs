using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Move;

    private int MaxMoveDistance => unit.Mobility;
    private List<Tile> _path;
    private int _currentPositionIndex;

    public float rotationSpeed;
    public float moveSpeed;

    public override int GetCost()
    {
        return _currentPositionIndex < 0 ? 0 : _currentPositionIndex - 1;
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        if (CombatManager.Instance.unitSystem.GetUnit(targetPos) != null) return false;
        
        _path = CombatManager.Instance.tileSystem.FindPath(unit.Position, targetPos, MaxMoveDistance);
        if (_path == null)
        {
            Debug.Log("impossible path");
            return false;
        }

        if (_path.Count <= 1)
        {
            Debug.Log("제자리");
            return false;
        }

        return true;
    }

    // ReSharper disable once InconsistentNaming
    public override void Execute(Vector3Int targetPos, Action _onActionComplete)
    {
        StartAction(_onActionComplete);

        //_path = unit.hexTransform.Map.FindPath(unit.Position, targetPos, _maxMoveDistance) as List<Vector3Int>;
        _currentPositionIndex = 1;  
        transform.forward =
            (Hex.Hex2World(_path[_currentPositionIndex].Position)
            - transform.position)
            .normalized;
    }

    private void Awake()
    {
        _currentPositionIndex = -1;
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPos = Hex.Hex2World(_path[_currentPositionIndex].Position);
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            unit.Position = _path[_currentPositionIndex].Position;
            
            _currentPositionIndex++;
            if (_currentPositionIndex >= _path.Count)
            {
                FinishAction();
            }
        }
    }
}
