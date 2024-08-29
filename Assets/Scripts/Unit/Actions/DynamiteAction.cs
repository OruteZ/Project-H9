
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class DynamiteAction : BaseAction
{
    [SerializeField] private float fallOff;
    [SerializeField] private float burningDamage;
    
    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        
        dynamitePrefab = Resources.Load<GameObject>("Prefab/DynamiteEffect");
    }

    public override ActionType GetActionType()
    {
        return ActionType.Dynamite;
    }
    
    public override void SetTarget(Vector3Int targetPos)
    {
        _center = targetPos;
        _targets = FieldSystem.unitSystem.GetUnitListInRange(targetPos, radius);
        _tileObjects = FieldSystem.tileSystem.GetTileObjectListInRange(targetPos, radius);
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

    protected override void SetAmount(float[] amounts)
    {
        if (amounts.Length != 2) return;
        
        fallOff = amounts[0];
        burningDamage = amounts[1];
    }

    #region PRIVATE

    private Vector3Int _center;
    private List<Unit> _targets;
    private List<TileObject> _tileObjects;
    private bool _waitingForExplosion;

    private void Explode()
    {
        foreach(Unit target in _targets)
        {
            int distance = Hex.Distance(_center, target.hexPosition);
            int calculatedDamage = damage - Mathf.RoundToInt(fallOff * (radius - distance));
            
            Damage dmgContext = new(calculatedDamage, calculatedDamage, Damage.Type.DEFAULT, unit, target);
            
            target.TakeDamage(dmgContext);
            if(target.HasDead()) continue;
            
            target.TryAddStatus(new Burning((int)burningDamage, 10, unit));  //for test
        }
        
        foreach (var tObject in _tileObjects)
        {
            if (tObject is not IDamageable damageableObject) continue;
            if (damageableObject is CoverableObj)
            {
                damage = Mathf.RoundToInt(damage * unit.coverObjDmgMultiplier);
            }
            Damage dmgCtxt = new Damage(damage, damage, Damage.Type.DEFAULT, unit, damageableObject);

            damageableObject.TakeDamage(dmgCtxt);
        }
    }

    #endregion
    
    #region THROW

    [SerializeField]
    private GameObject dynamitePrefab;

    private void Throw() 
    {
        DynamiteVisualEffect dynamiteVisualEffect = Instantiate(
            dynamitePrefab, 
            unit.transform.position, 
            Quaternion.identity)
            .GetComponent<DynamiteVisualEffect>();
        
        dynamiteVisualEffect.SetDestination(_center, () => _waitingForExplosion = false);
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