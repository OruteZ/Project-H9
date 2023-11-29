using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public abstract class StatusEffect : IDisplayableEffect
{
    protected StatusEffect(Unit creator)
    {
        this.creator = creator;
    }
    
    protected UnitStatusEffectController controller;
    public UnityEvent onStackChanged => controller?.onStatusEffectChanged;
    
    private int _damage;
    protected int damage
    {
        get => _damage;
        set
        {
            _damage = value;
            onStackChanged?.Invoke();
        }
    }

    protected Unit creator;
    public bool removable = false;

    #region ABSTRACT
    public abstract StatusEffectType GetStatusEffectType();
    public abstract StatusEffect Combine(StatusEffect other);
    public abstract void OnTurnStarted();
    public abstract void OnTurnFinished();
    public abstract int GetDuration();

    #endregion
    public bool CanDisplay()
    {
        return (removable is false);
    }

    public virtual void Setup(UnitStatusEffectController controller)
    {
        this.controller = controller;
        removable = false;
    }

    public virtual void Delete()
    {
        removable = true;
    }

    public int GetStack()
    {
        return damage;
    }


    public int GetIndex()
    {
        return (int)GetStatusEffectType();
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

//대응되는 keyword인덱스
public enum StatusEffectType
{
    None = 0,
    Burning = 10,
    Bleeding = 11,
    Stun = 12,
    UnArmed = 13,
    Concussion = 15,
    Fracture = 16,
    Blind = 17,
}