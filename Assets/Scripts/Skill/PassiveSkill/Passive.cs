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
        public static ICondition CloneTrigger(ConditionType type, float amount)
        {
            ICondition condition = type switch
            {
                ConditionType.Null => new NullCondition(amount),
                ConditionType.TargetHpMax => new TargetHpMax(amount),
                ConditionType.TargetLowHp => new TargetLowHp(amount),
                ConditionType.TargetHighHp => new TargetHighHp(amount),
                ConditionType.TargetHpIs => new TargetHpIs(amount),
                ConditionType.LowAmmo => new LowAmmo(amount),
                ConditionType.HighAmmo => new HighAmmo(amount),
                ConditionType.AmmoIs => new AmmoIs(amount),
                ConditionType.LessTargetRange => new LessTargetRange(amount),
                ConditionType.MoreTargetRange => new MoreTargetRange(amount),
                ConditionType.SameTargetRange => new SameTargetRange(amount),
                ConditionType.LowHp => new LowHp(amount),
                ConditionType.HighHp => new HighHp(amount),
                ConditionType.HpIs => new HpIs(amount),
                ConditionType.ReloadedInThisTurn => new ReloadedInThisTurn(amount),
                ConditionType.MovedInThisTurn => new MovedInThisTurn(amount),
                ConditionType.NotMovedInThisTurn => new NotMovedInThisTurn(amount),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return condition;
        }
        public static IEffect CloneEffect(PassiveEffectType type, UnitStatType stat, float amount)
        {
            IEffect effect = type switch
            {
                PassiveEffectType.StatUpDependedOnCondition => new StatUpDependedOnCondition(stat, amount),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return effect;
        }
        #endregion
        
        private readonly ICondition _condition;
        private readonly IEffect _effect;
        public readonly Unit unit;
    
        public Passive(Unit unit, ICondition condition, IEffect effect)
        {
            this.unit = unit;
            _condition = condition;
            _effect = effect;
        }
    
        public void EnableCondition()
        {
            _effect.OnConditionEnable();
        }

        public void DisableCondition()
        {
            _effect.OnConditionDisable();
        }

        public void Setup()
        {
            _condition.Setup(this);
            _effect.Setup(this);
        }

        public bool IsEffectEnable()
        {
            return _effect.IsEnable();
        }
    }
}
