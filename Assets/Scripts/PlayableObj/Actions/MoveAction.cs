using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Move;

    private int MaxMoveDistance => unit.Mobility;
    private List<Vector3Int> _path;
    private int _currentPositionIndex;

    private const float RotationSpeed = 10f;
    private const float MoveSpeed = 4f;

    private void Awake()
    {
        _currentPositionIndex = -1;
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        _path = unit.hexTransform.Map.FindPath(unit.Position, targetPos, MaxMoveDistance) as List<Vector3Int>;
        return (_path != null);
    }

    public override void Execute(Vector3Int targetPos, Action onActionComplete = null)
    {
        StartAction(onActionComplete);

        //_path = unit.hexTransform.Map.FindPath(unit.Position, targetPos, _maxMoveDistance) as List<Vector3Int>;
        _currentPositionIndex = 0;  
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPos = Hex.Hex2World(_path[_currentPositionIndex]);
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        
        //todo : look at dest

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * RotationSpeed);
        transform.position += moveDirection * (MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            unit.Position = _path[_currentPositionIndex];
            
            _currentPositionIndex++;
            if (_currentPositionIndex >= _path.Count)
            {
                FinishAction();
            }
        }
    }
}
