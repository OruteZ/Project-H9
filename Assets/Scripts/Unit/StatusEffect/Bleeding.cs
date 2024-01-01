using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Bleeding : StatusEffect
{
    public Bleeding(int damage, Unit creator) : base(creator)
    {
        base.damage = damage;
    }
    public override StatusEffectType GetStatusEffectType() => StatusEffectType.Bleeding;
    
    public override StatusEffect Combine(StatusEffect other)
    {
        damage += other.GetStack();
        return this;
    }

    public override void OnTurnStarted()
    {
        controller.GetUnit().TakeDamage(damage, creator);
    }
    
    public override void OnTurnFinished() { }
    public override int GetDuration()
    {
        return 0; 
    }
}