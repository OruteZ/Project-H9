using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SuicideDynamiteAction : BaseAction
{
    [SerializeField] private int executableHp = 10;
    [SerializeField] private int explosionRange = 3;
    [SerializeField] private SuicideDisplayableEffect effect;

    private List<Vector3Int> _targetPos;
    
    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        
        effect = new SuicideDisplayableEffect();
        effect.Setup(null, unit, explosionRange);
        effect.SetActive(false);
    }
    
    public override ActionType GetActionType()
    {
        return ActionType.SuicideDynamite;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        Unit target = FieldSystem.unitSystem.GetUnit(targetPos);
        if (target is null) return;
        
        effect.SetTarget(target);
    }

    public override bool CanExecute()
    {
        return CanExecute(Hex.zero);
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        return unit.stat.curHp <= executableHp && 
               effect.GetActive() is false;
    }

    public override bool IsSelectable()
    {
        return CanExecute();
    }

    public override bool CanExecuteImmediately()
    {
        return true;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        effect.SetActive(true);
        
        yield return new WaitForSeconds(1f);
        
        yield return true;
    }
}

[Serializable]
public class SuicideDisplayableEffect : IDisplayableEffect
{
    private bool _active;
    private int _range;

    [SerializeField]
    private Unit _unit, _target;
    
    public int GetIndex()
    {
        return -1;
    }

    public int GetStack()
    {
        return 0;
    }

    public int GetDuration()
    {
        return 0;
    }

    public bool CanDisplay()
    {
        return _active;
    }

    public void SetActive(bool active)
    {
        _active = active;
    }
    
    public bool GetActive()
    {
        return _active;
    }
    
    public void Setup(Unit target, Unit owner, int range)
    {
        _range = range;
        _unit = owner;
        _target = target;
        
        FieldSystem.unitSystem.onAnyUnitMoved.AddListener(OnAnyUnitMoved);
        
        _unit.AddDisplayableEffect(this);
    }
    
    public void SetTarget(Unit target)
    {
        _target = target;
    }

    private void OnAnyUnitMoved(Unit u)
    { 
        if (u != _target && u != _unit) return;
        if (_target is null) return;
        if (u is null) return;
        
        int distance = Hex.Distance(_unit.hexPosition, _target.hexPosition);
        if (distance > _range) return;
        
        _target.TakeDamage(new Damage(
            ushort.MaxValue, 
            ushort.MaxValue,
            Damage.Type.DEFAULT, 
            _unit,
            null,
            _target)
        );
    }
}