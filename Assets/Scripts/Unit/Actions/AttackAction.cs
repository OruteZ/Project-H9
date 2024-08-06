using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackAction : BaseAction, IShootingAction
{

    private int _atkCount;
    public override ActionType GetActionType() => ActionType.ATTACK;

    private Weapon weapon => unit.weapon;
    private IDamageable _target;
    // private State _state;
    // private float _stateTimer;

    public override bool CanExecute(Vector3Int targetPos)
    {
        if (FieldSystem.GetDamageable(targetPos) is null)
        {
            Debug.Log("There is no target, cant attack");
            return false;
        }
        
        if (IsThereWallBetweenUnitAnd(targetPos))
        {
            Debug.Log("There is wall. cant attack");
            return false;
        }
        
        //vision check
        if (FieldSystem.tileSystem.VisionCheck(targetPos, unit.hexPosition) is false)
        {
            Debug.Log("Target is not in vision, cant attack");
            return false;
        }

        if (weapon.GetWeaponType() == ItemType.Shotgun)
        {
            // if distance is greater than range, return false
            if (Hex.Distance(unit.hexPosition, _target.GetHex()) > weapon.GetRange())
            {
                Debug.Log("Distance is greater than range, cant attack");
                return false;
            }
        }

        return true;
    }

    public override bool IsSelectable()
    {
        if (weapon.CurrentAmmo == 0) return false;
        if (unit.CheckAttackedTrigger()) return false;
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
        _target = FieldSystem.GetDamageable(targetPos);
        Debug.Log("Attack Target : " + _target);
    }
    public IDamageable GetTarget() 
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

        if (IsThereWallBetweenUnitAnd(_target.GetHex()))
        {
            Debug.Log("There is wall. cant attack");
            return false;
        }

        if (weapon.GetWeaponType() == ItemType.Shotgun)
        {
            // if distance is greater than range, return false
            if (Hex.Distance(unit.hexPosition, _target.GetHex()) > weapon.GetRange())
            {
                Debug.Log("Distance is greater than range, cant attack");
                return false;
            }
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
        
        float timer = 1f;
        while ((timer -= Time.deltaTime) > 0)
        {
            Transform tsf;
            Vector3 aimDirection = (Hex.Hex2World(_target.GetHex()) - (tsf = transform).position).normalized;
        
            float rotationSpeed = 10f;
            transform.forward = Vector3.Slerp(tsf.forward, aimDirection, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(SHOOT);

        //총을 드는 시간 (애니메이션에 따라 다른 상수 값 , 무조건 프레임 단위로) 
        int cnt = 20;
        for(int i = 0; i < cnt; i++) yield return null;
        
        unit.TryAttack(_target);

        //Animation이 끝나고 IdleAction으로 돌아올 때 까지 대기 
        cnt = 69 - cnt;
        while (cnt-- > 0) yield return null;
        
        unit.animator.SetTrigger(IDLE);
        
        _atkCount++;
        if(unit.maximumShootCountInTurn <= _atkCount)
        {
            unit.TryAddStatus(new Recoil(unit));
        }
    }

    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        
        unit.onTurnStart.AddListener(OnTurnStart);
    }

    private void OnTurnStart(Unit arg0)
    {
        _atkCount = 0;
    }
}
