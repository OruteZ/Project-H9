using System;
using UnityEngine;

public class StatusEffectTester : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int duration;
    [SerializeField] private StatusEffectType type;
    
    public void Effect()
    {
        StatusEffect effect;
        
        //use switch case to create every status effect;
        switch (type)
        {
            case StatusEffectType.Bleeding:
                effect = new Bleeding(damage, null);
                break;
            case StatusEffectType.Burning:
                effect = new Burning(damage, duration, null);
                break;
            case StatusEffectType.Stun:
                effect = new Stun(duration, null);
                break;
            case StatusEffectType.UnArmed:
                effect = new UnArmed(duration, null);
                break;
            case StatusEffectType.Concussion:
                //duration
                effect = new Concussion(duration, null);
                break;
            case StatusEffectType.Fracture:
                //duration
                effect = new Fracture(duration, null);
                break;
            case StatusEffectType.Blind:
                //duration
                effect = new Blind(duration, null);
                break;
            case StatusEffectType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

        FieldSystem.unitSystem.GetPlayer().TryAddStatus(effect);
    }
}