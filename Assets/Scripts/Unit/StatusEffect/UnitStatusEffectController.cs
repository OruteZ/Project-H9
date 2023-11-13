﻿using System.Collections.Generic;
using Castle.Core;
using Unity.VisualScripting;
using UnityEngine.Events;

public class UnitStatusEffectController
{
    private readonly Unit _unit;
    public ref UnityEvent OnStatusEffectChanged => ref _unit.onStatusEffectChanged;

    private bool _unitTurn;
    
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
        statusEffect.Setup(this);
        
        //find same type in list
        foreach (var effect in _statusEffects)
        {
            if (effect.GetStatusEffectType() == statusEffect.GetStatusEffectType())
            {
                //combine
                _statusEffects.Remove(effect);
                _statusEffects.Add(effect + statusEffect);
                OnStatusEffectChanged.Invoke();
                return;
            }
        }
        
        //else, add
        _statusEffects.Add(statusEffect);
        OnStatusEffectChanged.Invoke();
    }
    
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        if (_statusEffects is null) return;
        
        //find same type and remove
        foreach (var effect in _statusEffects)
        {
            if (effect.GetStatusEffectType() == statusEffect.GetStatusEffectType())
            {
                _statusEffects.Remove(effect);
                OnStatusEffectChanged.Invoke();
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
                OnStatusEffectChanged.Invoke();
                return;
            }
        }
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

    public Unit GetUnit() => _unit;
    
    #region PRIVATE

    private void OnTurnStart()
    {
        _unitTurn = true;
        
        //use for, every IStatusEffect call OnTurnStart
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnStarted();
        }
    }

    private void OnTurnFinished()
    {
        _unitTurn = false;
        
        //every IStatusEffect call OnTurnFinished
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnFinished();
        }
    }
    
    #endregion
}