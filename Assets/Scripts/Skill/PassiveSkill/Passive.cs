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
            //return condition using switch
            ICondition ret = type switch
            {
                ConditionType.Null => new NullCondition(amount),
                ConditionType.TargetHpMax => new TargetHpMaxCondition(amount),
                ConditionType.TargetLowHp => new TargetLowHpCondition(amount),
                ConditionType.TargetHighHp => new TargetHighHpCondition(amount),
                ConditionType.TargetHpIs => new TargetHpIsCondition(amount),
                ConditionType.LowAmmo => new LowAmmoCondition(amount),
                ConditionType.HighAmmo => new HighAmmoCondition(amount),
                ConditionType.AmmoIs => new AmmoIsCondition(amount),
                ConditionType.LessTargetRange => new LessTargetRangeCondition(amount),
                ConditionType.MoreTargetRange => new MoreTargetRangeCondition(amount),
                ConditionType.SameTargetRange => new SameTargetRangeCondition(amount),
                ConditionType.LowHp => new LowHpCondition(amount),
                ConditionType.HighHp => new HighHpCondition(amount),
                ConditionType.HpIs => new HpIsCondition(amount),
                ConditionType.ReloadedInThisTurn => new ReloadedInThisTurnCondition(amount),
                ConditionType.MovedInThisTurn => new MovedInThisTurnCondition(amount),
                ConditionType.NotMovedInThisTurn => new NotMovedInThisTurnCondition(amount),
                ConditionType.Revenge => new RevengeCondition(amount),
                ConditionType.Dying => new DyingCondition(amount),
                ConditionType.Snipe => new SnipeCondition(amount),
                ConditionType.Fighter => new FighterCondition(amount),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return ret;
        }
        public static IEffect CloneEffect(PassiveEffectType type, StatType stat, int amount)
        {
            IEffect ret = type switch
            {
                PassiveEffectType.StatUpDependedOnCondition => new StatUpDependedOnCondition(stat, amount),
                PassiveEffectType.InfinityShootPoint => new InfinityShootPoint(stat, amount),
                PassiveEffectType.LightFootStep => new LightFootStep(stat, amount),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return ret;
        }
        #endregion
        
        private readonly ICondition _condition;
        private readonly IEffect _effect;
        public readonly Unit unit;
        public readonly int index;
    
        public Passive(Unit unit, int index, ICondition condition, IEffect effect)
        {
            this.unit = unit;
            _condition = condition;
            _effect = effect;
            this.index = index;
        }
    
        public void Enable()
        {
            _effect.OnConditionEnable();
        }

        public void Disable()
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
        
        public bool TryGetDisplayableEffect(out IDisplayableEffect displayableEffect)
        {
            if(_effect is IDisplayableEffect effect)
            {
                displayableEffect = effect;
                return true;
            }

            displayableEffect = null;
            return false;
        }
        
        public ConditionType GetConditionType()
        {
            return _condition.GetConditionType();
        }
        
        public PassiveEffectType GetEffectType()
        {
            return _effect.GetEffectType();
        }

        public void Delete()
        {
            _effect.OnDelete();
            _condition.OnDelete();
        }
    }
}
