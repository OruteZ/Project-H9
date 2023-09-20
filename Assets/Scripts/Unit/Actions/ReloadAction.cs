using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ReloadAction : BaseAction
{
    private Weapon weapon => unit.weapon;

    private const float ANIM_TIME = 0.5f;
    private const float RELOAD_TIME = 0.2f;
    private const float COOL_OFF_TIME = 0.5f;

    public override ActionType GetActionType()
    {
        return ActionType.Reload;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
    }

    public override int GetCost()
    {
        return 3;
    }

    public override int GetAmmoCost()
    {
        return 0;
    }

    public override bool IsSelectable()
    {
        return weapon.currentAmmo < weapon.maxAmmo;
    }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    public override bool CanExecute()
    {
        return IsSelectable();
    }

    public override void Execute(Action onActionComplete)
    {
        Debug.Log("Reload weapon");

        StartAction(onActionComplete);
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(RELOAD);

        yield return new WaitForSeconds(ANIM_TIME);

        for (int i = 0; i < weapon.maxAmmo; i++)
        {
            if (weapon.maxAmmo <= weapon.currentAmmo) weapon.currentAmmo = weapon.maxAmmo;
            else weapon.currentAmmo++;

            yield return new WaitForSeconds(RELOAD_TIME);
        }

        yield return new WaitForSeconds(COOL_OFF_TIME);
        
        unit.animator.SetTrigger(IDLE);
        FinishAction();
    }
}
