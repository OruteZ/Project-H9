using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.iOS;

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
        //build damage context
        Damage damageContext
            = new(damage, damage, Damage.Type.BLOODED, creator, controller.GetUnit());
        
        
        controller.GetUnit().TakeDamage(damageContext);
    }
    
    public override void OnTurnFinished() { }
    public override int GetDuration()
    {
        return 0; 
    }
}