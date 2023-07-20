using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Attack;

    private enum State
    {
        Aiming,
        Shooting,
        CoolOff
    }

    private Weapon weapon => unit.weapon;
    private Unit _target;
    private State _state;
    private float _stateTimer;

    public override bool IsSelectable()
    {
        if (weapon.currentEmmo == 0) return false;
        if (unit.hasAttacked) return false;

        return true;
    }

    public override bool ExecuteImmediately()
    {
        return false;
    }

    public override void Execute(Action onActionComplete)
    {
        Debug.Log("Attack Action call");
        StartAction(onActionComplete);
        unit.hasAttacked = true;

        _state = State.Aiming;
        _stateTimer = 1f;
    }

    public override int GetCost()
    {
        return 1;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _target = FieldSystem.unitSystem.GetUnit(targetPos);
        Debug.Log("Attack Target : " + _target);
    }

    public override bool CanExecute()
    {
        if (_target == null) return false;
        if (IsThereWallBetweenUnitAnd(_target.hexPosition)) return false;
        
        return true;
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

                    bool hit = weapon.GetFinalHitRate(_target) > UnityEngine.Random.value;

                    #if UNITY_EDITOR
                    Debug.Log(hit ? "뱅" : "빗나감");
                    #endif

                    if (hit)
                    {
                        weapon.Attack(_target, out var isHeadShot);
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
