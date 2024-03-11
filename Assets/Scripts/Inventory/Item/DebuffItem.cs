using System;
using UnityEngine;

public class DebuffItem : Item
{
    private readonly StatusEffectType _statusEffectType;
    private readonly int _duration;
    private readonly int _amount;

    public DebuffItem(ItemData data) : base(data)
    {
        _statusEffectType = (StatusEffectType)data.itemEffect;
        _duration = data.itemEffectDuration;
        _amount = data.itemEffectAmount;
    }

    public override bool Use(Unit user, Vector3Int target)
    {
        // Create a new status effect based on all status effect items
        StatusEffect statusEffect = null;
        
        switch (_statusEffectType)
        {
            case StatusEffectType.None:
                break;
            case StatusEffectType.Burning:
                statusEffect = new Burning(_amount, _duration, user);
                break;
            case StatusEffectType.Bleeding:
                // Assuming you have a Bleeding class similar to Burning and Fracture
                statusEffect = new Bleeding(_amount, user);
                break;
            case StatusEffectType.Stun:
                // Assuming you have a Stun class similar to Burning and Fracture
                statusEffect = new Stun(_duration, user);
                break;
            case StatusEffectType.UnArmed:
                // Assuming you have an UnArmed class similar to Burning and Fracture
                statusEffect = new UnArmed(_duration, user);
                break;
            case StatusEffectType.Concussion:
                // Assuming you have a Concussion class similar to Burning and Fracture
                statusEffect = new Concussion(_duration, user);
                break;
            case StatusEffectType.Fracture:
                statusEffect = new Fracture(_duration, user);
                break;
            case StatusEffectType.Blind:
                // Assuming you have a Blind class similar to Burning and Fracture
                statusEffect = new Blind(_duration, user);
                break;
        }

        if (statusEffect != null)
        {
            // Assuming the target unit has a method to add status effects
            var targetUnit = FieldSystem.unitSystem.GetUnit(target);
            targetUnit.TryAddStatus(statusEffect);
        }
        else return false;
        
        return true;
    }

    public override bool TryEquip()
    {
        return false;
    }
}