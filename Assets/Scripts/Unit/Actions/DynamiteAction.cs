
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class DynamiteAction : BaseAction
{
    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        
        dynamitePrefab = Resources.Load<GameObject>("Prefab/" + nameof(Dynamite));
    }

    public override ActionType GetActionType()
    {
        return ActionType.DYNAMITE;
    }
    
    public override void SetTarget(Vector3Int targetPos)
    {
        _center = targetPos;
        _targets = FieldSystem.unitSystem.GetUnitListInRange(targetPos, radius);
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        if (FieldSystem.tileSystem.GetTile(targetPos) is null)
        {
            Debug.LogWarning("center tile is null");
            return false;
        }

        if (range < Hex.Distance(unit.hexPosition, targetPos))
        {
            Debug.LogWarning("Too Far to throw bomb");
            return false;
        }

        return true;
    }

    public override bool IsSelectable()
    {
        if (unit.GetAction<ItemUsingAction>().GetItemUsedTrigger()) return false;
        
        return GetCost() <= unit.currentActionPoint;
    }

    public override bool CanExecute()
    {
        if (FieldSystem.tileSystem.GetTile(_center) is null)
        {
            Debug.LogWarning("center tile is null");
            return false;
        }

        if (range < Hex.Distance(unit.hexPosition, _center))
        {
            Debug.LogWarning("Too Far to throw bomb");
            return false;
        }

        return true;
    }

    public int GetExplosionRange() => radius;
    public int GetThrowingRange() => Mathf.Clamp(range, 0, unit.stat.sightRange);

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(DYNAMITE);
        
        //look at target
        unit.transform.LookAt(FieldSystem.tileSystem.GetTile(_center).transform);
        
        //rotation of z and x set 0
        var euler = unit.transform.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        unit.transform.eulerAngles = euler;
        
        _waitingForExplosion = true;
        yield return new WaitUntil(() => !_waitingForExplosion);
        
        Explode();
    }
    public override int GetSkillIndex() 
    {
        return 12001;
    }

    #region PRIVATE

    private Vector3Int _center;
    private List<Unit> _targets;
    private bool _waitingForExplosion;

    private void Explode()
    {
        foreach(var target in _targets)
        {
            target.TakeDamage(damage, unit);
            if(target.HasDead()) continue;
            
            target.TryAddStatus(new Burning(damage, 10, unit));  //for test
        }
    }

    #endregion
    
    #region THROW

    [SerializeField]
    private GameObject dynamitePrefab;

    private void Throw() 
    {
        Dynamite dynamite = Instantiate(dynamitePrefab, unit.transform.position, Quaternion.identity).GetComponent<Dynamite>();
        dynamite.SetDestination(_center, () => _waitingForExplosion = false);
    }

    public override void TossAnimationEvent(string args)
    {
        if (args != AnimationEventNames.THROW) return;
        
        {
            Throw();
        }
    }
    #endregion
}