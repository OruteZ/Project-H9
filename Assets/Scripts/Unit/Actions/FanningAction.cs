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

    private float _hitRateModifier;

    protected override void SetAmount(float[] amounts)
    {
        if (amounts.Length is not 1)
        {
            Debug.LogError($"HitRate modifier needs only one amounts. Length of this amounts is {amounts.Length}");
        }
        
        _hitRateModifier = amounts[0];
    }
    
    public float GetHitRateModifier()
    {
        return _hitRateModifier;
    }

    public override ActionType GetActionType()
    {
        return ActionType.FANNING;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _target = FieldSystem.unitSystem.GetUnit(targetPos);
        Debug.Log("Panning Target : " + _target);
    }

    public override int GetAmmoCost()
    {
        return Mathf.Max(unit.weapon.CurrentAmmo, 1);
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        if (FieldSystem.unitSystem.GetUnit(targetPos) == unit) return false;
        if (IsThereWallBetweenUnitAndTarget(targetPos)) return false;
        
        return true;
    }

    public override bool IsSelectable()
    {
        if (unit.weapon.CurrentAmmo == 0) return false;
        if (unit.CheckAttackedTrigger()) return false;
        if (unit.HasStatusEffect(StatusEffectType.UnArmed)) return false;
        if (unit.weapon.GetWeaponType() != ItemType.REVOLVER) return false;
        
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
        
        yield return new WaitUntil(() => unit.weapon.CurrentAmmo == 0);

        unit.animator.SetTrigger(FANNING_FINISH);
        yield return new WaitForSeconds(FINISH_TIME);
        unit.TryAddStatus(new Recoil(unit));
    }

    public override void TossAnimationEvent(string eventString)
    {
        if (eventString != AnimationEventNames.GUN_FIRE) return;
        
        unit.TryAttack(_target, _hitRateModifier);
    }
    public override int GetSkillIndex()
    {
        return 22001;
    }
}
