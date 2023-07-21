using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanningAction : BaseAction
{
    private Unit _target;
    private int _shotCount;

    private State _state;
    private float _stateTimer;
    
    private enum State
    {
        Aiming,
        Shooting,
        CoolOff
    }
    
    public override ActionType GetActionType()
    {
        throw new NotImplementedException();
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _target = FieldSystem.unitSystem.GetUnit(targetPos);
        Debug.Log("Panning Target : " + _target);
    }

    public override int GetCost()
    {
        return 4;
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override bool IsSelectable()
    {
        return unit.weapon.currentEmmo > 0;
    }

    public override bool CanExecute()
    {
        if (_target is null) return false;
        if (IsThereWallBetweenUnitAnd(_target.hexPosition)) return false;
        
        return true;
    }

    public override void Execute(Action onActionComplete)
    {
        _shotCount = unit.weapon.currentEmmo;
        _state = State.Aiming;
        _stateTimer = 0.5f;
        StartAction(onActionComplete);
    }
    
    private bool IsThereWallBetweenUnitAnd(Vector3Int targetPos)
    {
        return !FieldSystem.tileSystem.RayThroughCheck(unit.hexPosition, targetPos);
    }

    private void Update()
    {
        if (!isActive) return;

        switch (_state)
        {
            default:
            case State.Aiming:
                Transform tsf;
                Vector3 aimDirection = (Hex.Hex2World(_target.hexPosition) - (tsf = transform).position).normalized;

                float rotationSpeed = 10f;
                transform.forward = Vector3.Lerp(tsf.forward, aimDirection, Time.deltaTime * rotationSpeed);

                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0f)
                {
                    _state = State.Shooting;
                    _stateTimer = .5f;

                    for (int i = 0; i < _shotCount; i++)
                    {
                        bool hit = unit.weapon.GetFinalHitRate(_target) - 0.1f > UnityEngine.Random.value;
                        Debug.Log(hit ? "뱅" : "빗나감");

                        if (hit)
                        {
                            unit.weapon.Attack(_target, out var isHeadShot);
                        }
                    }
                    
                }
                break;
            
            case State.Shooting:
                _state = State.CoolOff;
                _stateTimer = .5f;
                break;
            
            case State.CoolOff:
                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0f)
                {
                    FinishAction();
                }
                break;
        }
    }
}