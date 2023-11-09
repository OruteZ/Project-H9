using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public abstract class StatusEffect : IUIDisplayableEffect
{
    protected UnitStatusEffectController controller;
    public UnityEvent OnStackChanged => controller.OnStatusEffectChanged;
    
    private int _damage;
    protected int Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            OnStackChanged.Invoke();
        }
    }

    #region ABSTRACT
    public abstract StatusEffectType GetStatusEffectType();
    public abstract StatusEffect Combine(StatusEffect other);
    public abstract void OnTurnStarted();
    public abstract void OnTurnFinished();
    public abstract int GetDuration();
    #endregion

    public virtual void Setup(UnitStatusEffectController controller)
    {
        this.controller = controller;
    }

    public virtual void Delete()
    {
        controller.RemoveStatusEffect(this);
    }

    public int GetStack()
    {
        return Damage;
    }


    public string GetEffectName()
    {
        return GetStatusEffectType().ToString();
    }
    
    #region STATIC
    public static StatusEffect operator + (StatusEffect a, StatusEffect b)
    {
        if(CanCombine(a, b))
            return a.Combine(b);
        
        return null;
    }

    private static bool CanCombine(StatusEffect statusEffect, StatusEffect statusEffect1)
    {
        return statusEffect.GetStatusEffectType() == statusEffect1.GetStatusEffectType();
    }
    #endregion
}

public enum StatusEffectType
{
    None,
    Burning,
    Bleeding,
    Stun,
    UnArmed,
    Concussion,
    Fracture,
    Blind,
}