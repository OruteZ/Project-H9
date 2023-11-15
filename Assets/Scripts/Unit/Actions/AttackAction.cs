using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackAction : BaseAction, IShootingActionKind
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
    // private State _state;
    // private float _stateTimer;

    public override bool IsSelectable()
    {
        if (weapon.currentAmmo == 0) return false;
        if (unit.hasAttacked) return false;
        if (unit.HasStatusEffect(StatusEffectType.UnArmed)) return false;

        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override int GetCost()
    {
        return 1;
    }

    public override int GetAmmoCost()
    {
        return 1;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _target = FieldSystem.unitSystem.GetUnit(targetPos);
        Debug.Log("Attack Target : " + _target);
    }
    public Unit GetTarget() 
    {
        return _target;
    }

    public override bool CanExecute()
    {
        if (_target == null)
        {
            Debug.Log("Target is null, cant attack");
            return false;
        }

        if (IsThereWallBetweenUnitAnd(_target.hexPosition))
        {
            Debug.Log("There is wall. cant attack");
            return false;
        }
        
        return true;
    }

    private bool IsThereWallBetweenUnitAnd(Vector3Int targetPos)
    {
        return !FieldSystem.tileSystem.RayThroughCheck(unit.hexPosition, targetPos);
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.SetTrigger(IDLE);
        
        unit.hasAttacked = true;
        
        float timer = 1f;
        while ((timer -= Time.deltaTime) > 0)
        {
            Transform tsf;
            Vector3 aimDirection = (Hex.Hex2World(_target.hexPosition) - (tsf = transform).position).normalized;
        
            float rotationSpeed = 10f;
            transform.forward = Vector3.Slerp(tsf.forward, aimDirection, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(SHOOT);

        //총을 드는 시간 (애니메이션에 따라 다른 상수 값 , 무조건 프레임 단위로) 
        int cnt = 20;
        for(int i = 0; i < cnt; i++) yield return null;
        
        unit.TryAttack(_target, 0);

        //Animation이 끝나고 IdleAction으로 돌아올 때 까지 대기 
        cnt = 69 - cnt;
        while (cnt-- > 0) yield return null;
        
        unit.animator.SetTrigger(IDLE);
    }
}
