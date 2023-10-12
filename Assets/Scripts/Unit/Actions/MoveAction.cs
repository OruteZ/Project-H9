using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        //return _path is null ? 1 : 0;
        return 1;
    }

    public override int GetAmmoCost()
    {
        return 0;
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
            Debug.Log("start : " + _startPosition);
            Debug.Log("dest : " + _destinationPosition);
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
        // transform.forward =
        //     (Hex.Hex2World(_path[_currentPositionIndex].hexPosition)
        //      - transform.position).normalized;
    }

    private void Awake()
    {
        _currentPositionIndex = -1;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.SetTrigger(MOVE);
        while(true)
        {
            //다음 움직임 타겟 타일 지정
            if (_path != null)
            {
                var targetTile = _path[_currentPositionIndex];
                if (targetTile.inSight) unit.visual.enabled = true;
            }
            else
            {
                throw new NullReferenceException("Move Path is Null");
            }

            //다음 움직임 타겟 타일의 위치 가져오기
            Vector3 targetPos = Hex.Hex2World(_path[_currentPositionIndex].hexPosition);
            targetPos.y = transform.position.y;

            //moveDirection 설정
            Vector3 moveDirection = (targetPos - transform.position).normalized;
            
            //회전
            Vector3 rotateDirection = moveDirection;
            Vector3 forwardVec2 = Vector3.Slerp(transform.forward,
                rotateDirection, rotationSpeed * Time.deltaTime);

            transform.forward = forwardVec2;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                if (unit.stat.TryConsume(StatType.CurActionPoint, 1) is false)
                {
                    Debug.LogError("Fail to consume");
                    #if UNITY_EDITOR
                    EditorApplication.isPaused = true;
                    #endif
                };
                unit.hexPosition = _path[_currentPositionIndex].hexPosition;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_path is null) yield break;

                _currentPositionIndex++;
                if (_currentPositionIndex >= _path.Count)
                {
                    FinishAction();
                    unit.animator.SetTrigger(IDLE);
                    _path = null;
                    yield break;
                }
            }
            
            yield return null;
        }
    }

    private Tile GetNextTile()
    {
        return _path[_currentPositionIndex];
    }

    public override void ForceFinish()
    {
        _path = null;
        base.ForceFinish();
    }
}
