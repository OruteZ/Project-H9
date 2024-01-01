using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FanningAction : BaseAction, IShootingAction
{
    //todo : 애니메이션 확정되면 다 Frame단위 Int로 변경
    private const float TURNING_TIME = 0.5f;
    private const float SHOT_TIME = 0.1f;
    private const float COOL_OFF_TIME = 0.5f;
    
    private Unit _target;
    private int _shotCount;

    private float _hitRateModifier;

    protected override void SetAmount(float[] amounts)
    {
        if (amounts.Length is not 1)
        {
            Debug.LogError($"HitRate modifier needs only one amounts. Length of this amounts is {amounts.Length}");
        }
        
        _hitRateModifier = amounts[0];
    }

    public override ActionType GetActionType()
    {
        return ActionType.Fanning;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _target = FieldSystem.unitSystem.GetUnit(targetPos);
        Debug.Log("Panning Target : " + _target);
    }

    public override int GetAmmoCost()
    {
        return unit.weapon.currentAmmo;
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override bool IsSelectable()
    {
        if (unit.weapon.currentAmmo == 0) return false;
        if (unit.CheckAttackTrigger()) return false;
        if (unit.HasStatusEffect(StatusEffectType.UnArmed)) return false;

        return true;
    }

    public override bool CanExecute()
    {
        if (_target is null) return false;
        if (IsThereWallBetweenUnitAndThis(_target.hexPosition)) return false;
        
        return true;
    }
    
    private bool IsThereWallBetweenUnitAndThis(Vector3Int targetPos)
    {
        return !FieldSystem.tileSystem.RayThroughCheck(unit.hexPosition, targetPos);
    }

    private const float ROTATION_SPEED = 10f;
    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(FANNING);
        
        _shotCount = unit.weapon.currentAmmo;
        
        Transform tsf;
        Vector3 aimDirection = (Hex.Hex2World(_target.hexPosition) - (tsf = transform).position).normalized;
        aimDirection.y = 0;

        float totalTime = 0;
        while ((totalTime += Time.deltaTime) > TURNING_TIME)
        {
            transform.forward = Vector3.Lerp(tsf.forward, aimDirection, Time.deltaTime * ROTATION_SPEED);
            yield return null;
        }
        transform.forward = aimDirection;

        for (int i = 0; i < _shotCount; i++)
        {
            yield return new WaitForSeconds(SHOT_TIME);
            unit.TryAttack(_target, _hitRateModifier);
        }

        yield return new WaitForSeconds(COOL_OFF_TIME);
        
        unit.SetAttacked();
    }
}
