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

    public override bool IsSelectable()
    {
        if (unit.hasAttacked) return false;
        
        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _startPosition = unit.hexPosition;
        _destinationPosition = targetPos;
        _currentPositionIndex = 1;  
    }

    public override bool CanExecute()
    {
        if (FieldSystem.unitSystem.GetUnit(_destinationPosition) != null)
        {
            Debug.Log("there is unit, cant move");
            return false;
        }
        
        _path = FieldSystem.tileSystem.FindPath(_startPosition, _destinationPosition, maxMoveDistance);
        if (_path == null)
        {
            Debug.Log("There is no path");
            return false;
        }
        if (_path.Count < 1)
        {
            Debug.Log("제자리");
            return false;
        }

        return true;
    }

    // ReSharper disable once InconsistentNaming
    public override void Execute(Action onActionComplete)
    {
        StartAction(onActionComplete);

        //_path = unit.hexTransform.Map.FindPath(unit.Position, targetPos, _maxMoveDistance) as List<Vector3Int>;
        transform.forward =
            (Hex.Hex2World(_path[_currentPositionIndex].hexPosition)
            - transform.position).normalized;
    }

    private void Awake()
    {
        _currentPositionIndex = -1;
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPos = Hex.Hex2World(_path[_currentPositionIndex].hexPosition);
        targetPos.y = transform.position.y;
        
        Vector3 moveDirection = (targetPos - transform.position).normalized;

        Vector2 forwardVec2 = Vector2.Lerp(new Vector2(transform.forward.x, transform.forward.z),
            new Vector2(moveDirection.x, moveDirection.z), Time.deltaTime * rotationSpeed);

        transform.forward = new Vector3(forwardVec2.x, 0, forwardVec2.y);

        transform.position += moveDirection * (moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            unit.hexPosition = _path[_currentPositionIndex].hexPosition;
            
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
