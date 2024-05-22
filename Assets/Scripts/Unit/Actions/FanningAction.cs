using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FanningAction : BaseAction, IShootingAction
{
    private const float TURNING_TIME = 0.5f;
    private const float DRAW_TIME = 1f;
    private const float SHOT_TIME = 0.15f;
    private const float FINISH_TIME = 1f;
    
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
        return Mathf.Max(unit.weapon.currentAmmo, 1);
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override bool IsSelectable()
    {
        if (unit.weapon.currentAmmo == 0) return false;
        if (unit.CheckAttackedTrigger()) return false;
        if (unit.HasStatusEffect(StatusEffectType.UnArmed)) return false;
        if (unit.weapon.GetWeaponType() != ItemType.Revolver) return false;
        
        return true;
    }

    public override bool CanExecute()
    {
        if (_target is null || _target == unit) return false;
        if (IsThereWallBetweenUnitAndTarget(_target.hexPosition)) return false;
        
        return true;
    }
    
    private bool IsThereWallBetweenUnitAndTarget(Vector3Int targetPos)
    {
        return !FieldSystem.tileSystem.RayThroughCheck(unit.hexPosition, targetPos);
    }

    private const float ROTATION_SPEED = 10f;
    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(FANNING);
        
        Transform tsf;
        Vector3 aimDirection = (Hex.Hex2World(_target.hexPosition) - (tsf = transform).position).normalized;
        aimDirection.y = 0;

        float totalTime = 0;
        while ((totalTime += Time.deltaTime) > DRAW_TIME)
        {
            transform.forward = Vector3.Lerp(tsf.forward, aimDirection, Time.deltaTime * ROTATION_SPEED);
            yield return null;
        }
        transform.forward = aimDirection;
        
        unit.animator.SetTrigger(FANNING_FIRE);
        _shotCount = unit.weapon.currentAmmo;
        yield return new WaitUntil(() => _shotCount == 0);

        unit.animator.SetTrigger(FANNING_FINISH);
        yield return new WaitForSeconds(FINISH_TIME);
        unit.TryAddStatus(new Recoil(unit));
    }

    public override void TossAnimationEvent(string eventString)
    {
        if (eventString != AnimationEventNames.GUN_FIRE) return;
        if (_shotCount <= 0) return;
        
        unit.TryAttack(_target, _hitRateModifier);
        _shotCount--;
    }
}
