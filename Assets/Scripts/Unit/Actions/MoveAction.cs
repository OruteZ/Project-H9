using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Move;

    private int maxMoveDistance => unit.currentActionPoint;
    private List<Tile> _path;
    private int _currentPositionIndex;

    public float rotationSpeed;
    public float moveSpeed;

    private Vector3Int _startPosition;
    private Vector3Int _destinationPosition;

    public override int GetCost()
    {
        return _currentPositionIndex < 0 ? 0 : _currentPositionIndex - 1;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _startPosition = unit.position;
        _destinationPosition = targetPos;
    }

    public override bool CanExecute()
    {
        if (MainSystem.instance.unitSystem.GetUnit(_destinationPosition) != null) return false;
        
        _path = MainSystem.instance.tileSystem.FindPath(_startPosition, _destinationPosition, maxMoveDistance);
        if (_path == null)
        {
            return false;
        }
        if (_path.Count <= 1)
        {
            return false;
        }

        return true;
    }

    // ReSharper disable once InconsistentNaming
    public override void Execute(Action onActionComplete)
    {
        StartAction(onActionComplete);

        //_path = unit.hexTransform.Map.FindPath(unit.Position, targetPos, _maxMoveDistance) as List<Vector3Int>;
        _currentPositionIndex = 1;  
        transform.forward =
            (Hex.Hex2World(_path[_currentPositionIndex].position)
            - transform.position).normalized;
    }

    private void Awake()
    {
        _currentPositionIndex = -1;
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPos = Hex.Hex2World(_path[_currentPositionIndex].position);
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            unit.position = _path[_currentPositionIndex].position;
            
            _currentPositionIndex++;
            if (_currentPositionIndex >= _path.Count)
            {
                FinishAction();
            }
        }
    }

    private Tile GetNextTile()
    {
        return _path[_currentPositionIndex];
    }
}
