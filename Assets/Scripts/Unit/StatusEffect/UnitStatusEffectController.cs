using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Unity.VisualScripting;
using UnityEngine.Events;

public class UnitStatusEffectController
{
    private readonly Unit _unit;
    public ref UnityEvent onStatusEffectChanged => ref _unit.onStatusEffectChanged;
    private List<StatusEffect> _statusEffects = new ();

    public UnitStatusEffectController(Unit unit)
    {
        _unit = unit;
        
        unit.onTurnStart.AddListener((u) => OnTurnStart());
        unit.onTurnEnd.AddListener((u) => OnTurnFinished());
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if (_statusEffects is null) _statusEffects = new List<StatusEffect>();
        
        //find same type in list
        foreach (var effect in _statusEffects)
        {
            if (effect.GetStatusEffectType() == statusEffect.GetStatusEffectType())
            {
                //combine
                _statusEffects.Remove(effect);
                _statusEffects.Add(effect + statusEffect);
                onStatusEffectChanged.Invoke();
                return;
            }
        }
        
        //else, add
        _statusEffects.Add(statusEffect);
        statusEffect.Setup(this);
        onStatusEffectChanged.Invoke();
    }
    
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        if (_statusEffects is null) return;
        
        //find same type and remove
        foreach (var effect in _statusEffects)
        {
            if (effect.GetStatusEffectType() == statusEffect.GetStatusEffectType())
            {
                effect.removable = true;
                return;
            }
        }
    }

    public void RemoveStatusEffect(StatusEffectType type)
    {
        //find same type and remove
        foreach (var effect in _statusEffects)
        {
            if (effect.GetStatusEffectType() == type)
            {
                _statusEffects.Remove(effect);
                onStatusEffectChanged.Invoke();
                return;
            }
        }
    }
    
    public void RemoveAllStatusEffect()
    {
        if (_statusEffects is null) return;
        
        _statusEffects.Clear();
        onStatusEffectChanged.Invoke();
    }
    
    public List<IDisplayableEffect> GetAllStatusEffectInfo()
    {
        if (_statusEffects is null) return null;
        
        var list = new List<IDisplayableEffect>();
        foreach (var statusEffect in _statusEffects)
        {
            list.Add(statusEffect);
        }

        return list;
    }
    
    public bool HasStatusEffect(StatusEffectType type)
    {
        if (_statusEffects is null) return false;
        
        foreach (var statusEffect in _statusEffects)
        {
            if (statusEffect.GetStatusEffectType() == type)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool TryGetStatusEffect(StatusEffectType type, out StatusEffect statusEffect)
    {
        if (_statusEffects is null)
        {
            statusEffect = null;
            return false;
        }
        
        foreach (var effect in _statusEffects)
        {
            if (effect.GetStatusEffectType() == type)
            {
                statusEffect = effect;
                return true;
            }
        }

        statusEffect = null;
        return false;
    }

    public Unit GetUnit() => _unit;
    
    #region PRIVATE

    private void OnTurnStart()
    {
        //use for, every IStatusEffect call OnTurnStart
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnStarted();
        }
    }

    private void OnTurnFinished()
    {
        //every IStatusEffect call OnTurnFinished
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnFinished();
        }
        
        LazyRemove();
    }

    private void LazyRemove()
    {
        //find removable all
        bool removeAny = _statusEffects.Any((effect) => effect.removable);
        _statusEffects.RemoveAll((effect) => effect.removable);

        if (removeAny)
        {
            onStatusEffectChanged.Invoke();
        }
    }
    
    #endregion
}