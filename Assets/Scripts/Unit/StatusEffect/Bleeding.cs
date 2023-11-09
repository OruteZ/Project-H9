using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Mono.Collections.Generic;

public class Bleeding : StatusEffect
{
    public Bleeding(int damage)
    {
        Damage = damage;
    }
    public override StatusEffectType GetStatusEffectType() => StatusEffectType.Bleeding;
    
    public override StatusEffect Combine(StatusEffect other)
    {
        Damage += other.GetStack();
        return this;
    }

    public override void OnTurnStarted()
    {
        controller.GetUnit().TakeDamage(Damage);
    }
    
    public override void OnTurnFinished() { }
    public override int GetDuration()
    {
        return 0; 
    }
}