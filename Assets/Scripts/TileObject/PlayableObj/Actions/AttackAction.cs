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

    public Weapon weapon;
    private Unit _target;
    private State _state;
    private float _stateTimer;

    public override void Execute(Vector3Int targetPos, Action _onActionComplete)
    {
        Debug.Log("Attack Action call");
        StartAction(_onActionComplete);

        _state = State.Aiming;
        _stateTimer = 1f;
    }

    public override int GetCost()
    {
        return 1;
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        weapon = unit.weapon;
        _target = CombatManager.Instance.unitSystem.GetUnit(targetPos);
        #if UNITY_EDITOR
        Debug.Log("Attack Target : " + _target);
        #endif

        if (_target == null)
        {
#if UNITY_EDITOR
            Debug.Log("There is no tile. attack failed");
#endif
            return false;
        }

        if (Hex.Distance(unit.Position, targetPos) > weapon.range)
        {
#if UNITY_EDITOR
            Debug.Log("Target position is so far. attack failed");
#endif
            return false;
        }

        if (!CombatManager.Instance.tileSystem.RayCast(unit.Position, targetPos))
        {
#if UNITY_EDITOR
            Debug.Log("There is wall between target and player. attack failed");
#endif
            return false;
        }

        return true;
    }

    private void Update()
    {
        if (!isActive) return;

        switch (_state)
        {
            default:
            case State.Aiming:
                Vector3 aimDirection = (Hex.Hex2World(_target.Position) - transform.position).normalized;

                float rotationSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);

                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0f)
                {
                    _state = State.Shooting;
                    _stateTimer = .5f;

                    bool hit = true;
                    //todo : 명중률 가져와서 명중여부 계산
                    //todo : OnShoot 이벤트 호출

                    if (hit)
                    {
                        weapon.Attack(_target);
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
