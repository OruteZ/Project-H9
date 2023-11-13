using System;
using UnityEngine;

public class StatusEffectTester : MonoBehaviour
{
    [SerializeField] private int _stack;
    [SerializeField] private int _duration;
    [SerializeField] private StatusEffectType type;
    
    public void Effect()
    {
        StatusEffect effect;
        
        //use switch case to create every status effect;
        switch (type)
        {
            case StatusEffectType.Bleeding:
                effect = new Bleeding(_stack, null);
                break;
            case StatusEffectType.Burning:
                effect = new Burning(_stack, _duration, null);
                break;
            case StatusEffectType.Stun:
                effect = new Stun(_duration, null);
                break;
            case StatusEffectType.UnArmed:
                effect = new UnArmed(_duration, null);
                break;
            case StatusEffectType.Concussion:
                //duration
                effect = new Concussion(_duration, null);
                break;
            case StatusEffectType.Fracture:
                //duration
                effect = new Fracture(_duration, null);
                break;
            case StatusEffectType.Blind:
                //duration
                effect = new Blind(_duration, null);
                break;
            case StatusEffectType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

        FieldSystem.unitSystem.GetPlayer().TryAddStatus(effect);
    }
}