using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace PassiveSkill
{
    public class Passive
    {
        #region STATIC
        public static ITrigger CloneTrigger(TriggerType type, float amount)
        {
            ITrigger trigger = type switch
            {
                TriggerType.Null => new NullTrigger(amount),
                TriggerType.TargetLowHp => new TargetLowHp(amount),
                TriggerType.TargetHighHp => new TargetHighHp(amount),
                TriggerType.TargetHpIs => new TargetHpIs(amount),
                TriggerType.LowAmmo => new LowAmmo(amount),
                TriggerType.HighAmmo => new HighAmmo(amount),
                TriggerType.AmmoIs => new AmmoIs(amount),
                TriggerType.LowHp => new LowHp(amount),
                TriggerType.HighHp => new HighHp(amount),
                TriggerType.HpIs => new HpIs(amount),
                TriggerType.ReloadedInThisTurn => new ReloadedInThisTurn(amount),
                TriggerType.MovedInThisTurn => new MovedInThisTurn(amount),
                TriggerType.NotMovedInThisTurn => new NotMovedInThisTurn(amount),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return trigger;
        }
        public static IEffect CloneEffect(PassiveEffectType type, float amount)
        {
            IEffect effect = type switch
            {
                PassiveEffectType.RevolverAdditionalDamageUp => new RevolverDamageUp(amount),
                _ => throw new ArgumentOutOfRangeException()
            };

            return effect;
        }
        #endregion
        
        private readonly ITrigger _trigger;
        private readonly IEffect _effect;
        public readonly Unit unit;

        private bool _enable;
    
        public Passive(Unit unit, ITrigger trigger, IEffect effect)
        {
            this.unit = unit;
            _trigger = trigger;
            _effect = effect;
        }
    
        public void TurnOnPassive()
        {
            if (_enable) return;
            
            _enable = true;
            _effect.Enable();
        }

        public void TurnOffPassive()
        {
            if (_enable is false) return;
            
            _enable = false;
            _effect.Disable();
        }

        public void Setup()
        {
            _trigger.Setup(this);
            _effect.Setup(this);
            _enable = false;
        }

        public bool IsEnable()
        {
            return _enable;
        }
    }
}
