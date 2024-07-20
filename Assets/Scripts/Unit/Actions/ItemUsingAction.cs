using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemUsingAction : BaseAction
{
    private Vector3Int _targetPos;
    private IItem _item;
    private bool _itemUsedTrigger;

    public override ActionType GetActionType()
    {
        return ActionType.ItemUsing;
    }

    public bool GetItemUsedTrigger()
    {
        return _itemUsedTrigger;
    }

    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        _itemUsedTrigger = false;
        unit.onTurnStart.AddListener((u) => ResetUseTrigger());
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _targetPos = targetPos;
    }

    public void SetItem(IItem item)
    {
        _item = item;
        range = item.GetData().itemRange;
    }

    public override bool CanExecute()
    {
        if (_item is null) return false;
        if (_itemUsedTrigger) return false;
        if (_item.IsImmediate()) return true;
        
        if (FieldSystem.tileSystem.GetTile(_targetPos) is null)
        {
            Debug.LogWarning("center tile is null");
            return false;
        }
        if (range < Hex.Distance(unit.hexPosition, _targetPos))
        {
            Debug.LogWarning("Too Far to throw Item");
            return false;
        }

        return true;
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        if (_item is null) return false;
        if (_itemUsedTrigger) return false;
        if (_item.IsImmediate()) return true;
        
        if (FieldSystem.tileSystem.GetTile(targetPos) is null)
        {
            Debug.LogWarning("center tile is null");
            return false;
        }
        if (range < Hex.Distance(unit.hexPosition, targetPos))
        {
            Debug.LogWarning("Too Far to throw Item");
            return false;
        }

        return true;
    }

    public override bool IsSelectable()
    {
        if (_itemUsedTrigger) return false;
        if (unit.HasStatusEffect(StatusEffectType.Recoil)) return false;

        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return _item.IsImmediate();
    }
    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(DYNAMITE);
        
        //look at target
        unit.transform.LookAt(FieldSystem.tileSystem.GetTile(_targetPos).transform);
        
        //rotation of z and x set 0
        var euler = unit.transform.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        unit.transform.eulerAngles = euler;
        
        _waitingForExplosion = true;
        yield return new WaitUntil(() => !_waitingForExplosion);

        _item.Use(unit, _targetPos);
        IInventory.OnInventoryChanged?.Invoke();
        _itemUsedTrigger = true;

        yield return new WaitForSeconds(1f);
    }

    private void ResetUseTrigger()
    {
        _itemUsedTrigger = false;
    }

    public bool IsItemUsed() 
    {
        return _itemUsedTrigger;
    }
    public IItem GetItem()
    {
        return _item;
    }

    #region THROW

    [SerializeField]
    private GameObject _dynamitePrefab;

    [SerializeField]
    private bool _waitingForExplosion = false;

    private void Throw() 
    {
        if(_dynamitePrefab == null || _item.GetData().itemType is not ItemType.Damage)
        {
            Debug.LogError("Dynamite Prefab is null");
            //stop action immediately
            _waitingForExplosion = false;
            return;
        }

        Dynamite dynamite = Instantiate(_dynamitePrefab, unit.transform.position, Quaternion.identity).GetComponent<Dynamite>();
        dynamite.SetDestination(_targetPos, () => _waitingForExplosion = false);
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