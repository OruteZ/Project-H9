
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamiteAction : BaseAction
{
    public override ActionType GetActionType()
    {
        return ActionType.Dynamite;
    }
    
    public override void SetTarget(Vector3Int targetPos)
    {
        _center = targetPos;
        _targets = FieldSystem.unitSystem.GetUnitListInRange(targetPos, explosionRange);
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override int GetAmmoCost()
    {
        return 0;
    }

    public override int GetCost()
    {
        return 1;
    }

    public override bool IsSelectable()
    {
        return GetCost() <= unit.currentActionPoint;
    }

    public override bool CanExecute()
    {
        if (FieldSystem.tileSystem.GetTile(_center) is null)
        {
            Debug.LogWarning("center tile is null");
            return false;
        }

        if (throwingRange < Hex.Distance(unit.hexPosition, _center))
        {
            Debug.LogWarning("Too Far to throw bomb");
            return false;
        }

        return true;
    }

    public override void SetAmount(float[] amounts)
    {
        if (amounts.Length is not 3)
        {
            Debug.LogError($"Dynamite Action needs 3 amounts. length of amounts is {amounts.Length}");
            return;
        }

        damage = (int)amounts[0];
        explosionRange = (int)amounts[1];
        throwingRange = (int)amounts[2];
    }

    public int GetExplosionRange() => explosionRange;
    public int GetThrowingRange() => Mathf.Clamp(throwingRange, 0, unit.stat.sightRange);

    protected override IEnumerator ExecuteCoroutine()
    {
        Explode();
        yield break;
    }

    #region PRIVATE

    private Vector3Int _center;
    private List<Unit> _targets;

    [SerializeField] private int damage;
    [SerializeField] private int explosionRange;
    [SerializeField] private int throwingRange;

    private void Explode()
    {
        foreach(var target in _targets)
        {
            target.TakeDamage(damage, unit);
        }
    }

    #endregion
}