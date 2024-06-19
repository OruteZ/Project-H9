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
        public static ICondition[] CloneTrigger(ConditionType[] type, float[] amount)
        {
            //return condition using switch
            ICondition[] ret = new ICondition[type.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ICondition r = type[i] switch
                {
                    ConditionType.Null => new NullCondition(amount[i]),
                    ConditionType.TargetHpMax => new TargetHpMaxCondition(amount[i]),
                    ConditionType.TargetLowHp => new TargetLowHpCondition(amount[i]),
                    ConditionType.TargetHighHp => new TargetHighHpCondition(amount[i]),
                    ConditionType.TargetHpIs => new TargetHpIsCondition(amount[i]),
                    ConditionType.LowAmmo => new LowAmmoCondition(amount[i]),
                    ConditionType.HighAmmo => new HighAmmoCondition(amount[i]),
                    ConditionType.AmmoIs => new AmmoIsCondition(amount[i]),
                    ConditionType.LessTargetRange => new LessTargetRangeCondition(amount[i]),
                    ConditionType.MoreTargetRange => new MoreTargetRangeCondition(amount[i]),
                    ConditionType.SameTargetRange => new SameTargetRangeCondition(amount[i]),
                    ConditionType.LowHp => new LowHpCondition(amount[i]),
                    ConditionType.HighHp => new HighHpCondition(amount[i]),
                    ConditionType.HpIs => new HpIsCondition(amount[i]),
                    ConditionType.ReloadedInThisTurn => new ReloadedInThisTurnCondition(amount[i]),
                    ConditionType.MovedInThisTurn => new MovedInThisTurnCondition(amount[i]),
                    ConditionType.NotMovedInThisTurn => new NotMovedInThisTurnCondition(amount[i]),

                    ConditionType.UseFanning => new UseFanningCondition(amount[i]),
                    ConditionType.UseFanningAndCheckChance => new UseFanningAndCheckChanceCondition(amount[i]),
                    ConditionType.HitSixFanningShot => new HitSixFanningShotCondition(amount[i]),
                    ConditionType.ShootAGoldenBullet => new ShootAGoldenBulletCondition(amount[i]),
                    ConditionType.TargetIsHitByGoldenBulletInThisTurn => new TargetIsHitByGoldenBulletInThisTurn(amount[i]),
                    ConditionType.TargetOnSweetSpot => new TargetOnSweetSpotCondition(amount[i]),
                    ConditionType.KillEnemy => new KillCondition(amount[i]),
                    ConditionType.KillEnemyOnSweetSpot => new KillOnSweetSpotCondition(amount[i]),
                    ConditionType.Critical => new CriticalShotCondition(amount[i]),
                    ConditionType.NonCritical => new NonCriticalShotCondition(amount[i]),
                    ConditionType.EquipRevolver => new EquipRevolverCondition(amount[i]),
                    ConditionType.EquipRepeater => new EquipRepeaterCondition(amount[i]),
                    ConditionType.EquipShotgun => new EquipShotgunCondition(amount[i]),

                    //ConditionType.Revenge => new RevengeCondition(amount[i]),
                    //ConditionType.Dying => new DyingCondition(amount[i]),
                    //ConditionType.Snipe => new SnipeCondition(amount[i]),
                    //ConditionType.Fighter => new FighterCondition(amount[i]),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type[i], null)
                };
                ret[i] = r;
            }

            return ret;
        }
        public static IEffect[] CloneEffect(PassiveEffectType[] type, StatType[] stat, int[] amount)
        {
            IEffect[] ret = new IEffect[type.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                IEffect r = type[i] switch
                {
                    PassiveEffectType.StatUpDependedOnCondition => new StatUpDependedOnCondition(stat[i], amount[i]),
                    PassiveEffectType.InfinityShootPoint => new InfinityShootPoint(stat[i], amount[i]),
                    PassiveEffectType.LightFootStep => new LightFootStep(stat[i], amount[i]),
                    PassiveEffectType.DoubleShootPoint => new DoubleShootPoint(stat[i], amount[i]),
                    PassiveEffectType.FreeReload => new FreeReload(stat[i], amount[i]),
                    PassiveEffectType.GoldenBullet => new GoldenBulletEffect(stat[i], amount[i]),
                    PassiveEffectType.TwoGoldenBullets => new TwoGoldenBulletsEffect(stat[i], amount[i]),
                    PassiveEffectType.StatUpDuringThreeTurns => new StatUpDuringThreeTurns(stat[i], amount[i]),
                    PassiveEffectType.SweetSpot => new SweetSpotEffect(stat[i], amount[i]),
                    PassiveEffectType.StatUpWhileAction => new StatUpWhileAction(stat[i], amount[i]),
                    PassiveEffectType.StatUpDuringATurn => new StatUpDuringATurn(stat[i], amount[i]),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type[i], null)
                };
                ret[i] = r;
            }

            return ret;
        }
        #endregion
        
        private readonly ICondition[] _condition;
        private readonly IEffect[] _effect;
        public readonly Unit unit;
        public readonly int index;

        private bool[] _ischeckedCondition;
    
        public Passive(Unit unit, int index, ICondition[] condition, IEffect[] effect)
        {
            this.unit = unit;
            _condition = condition;
            _ischeckedCondition = new bool[_condition.Length];
            for (int i = 0; i < _ischeckedCondition.Length; i++) _ischeckedCondition[i] = false;
            _effect = effect;
            this.index = index;
        }

        public void FullfillCondition(ICondition enabledCondition)
        {
            for (int i = 0; i < _condition.Length; i++)
            {
                if (_condition[i] == enabledCondition)
                {
                    //Debug.LogError("check " + enabledCondition);
                    _ischeckedCondition[i] = true;
                }
            }
            for (int i = 0; i < _ischeckedCondition.Length; i++)
            {
                if (!_ischeckedCondition[i]) return;
            }
            Enable();
        }
        public void NotFullfillCondition(ICondition disabledCondition)
        {
            for (int i = 0; i < _condition.Length; i++)
            {
                if (_condition[i] == disabledCondition)
                {
                    //Debug.LogError("cancel "+disabledCondition);
                    _ischeckedCondition[i] = false;
                }
            }
            Disable();
        }
        public void Enable()
        {

            if (IsEffectEnable() && _condition[0] is NullCondition) return;
            //Debug.LogError("Effect On: " + index);
            foreach (IEffect effect in _effect)
            {
                effect.OnConditionEnable();
            }
        }
        public void Disable()
        {

            //Debug.LogError("Effect Off: " + index);
            foreach (IEffect effect in _effect)
            {
                effect.OnConditionDisable();
            }
        }

        public void Setup()
        {
            foreach (ICondition condition in _condition)
            {
                condition.Setup(this);
            }
            foreach (IEffect effect in _effect)
            {
                effect.Setup(this);
            }

            if (_condition[0].GetConditionType() is ConditionType.Null)
            {
                Enable();
            }
        }

        public bool IsEffectEnable()
        {
            foreach (IEffect effect in _effect)
            {
                if (!effect.IsEnable()) return false;
            }
            return true;
        }
        
        public bool TryGetDisplayableEffect(out List<IDisplayableEffect> displayableEffect)
        {
            displayableEffect = new List<IDisplayableEffect>();
            foreach (IEffect effect in _effect)
            {
                if (effect is not IDisplayableEffect)
                {
                    displayableEffect = null;
                    return false;
                }
                displayableEffect.Add((IDisplayableEffect)effect);
            }

            return true;
        }
        
        public ConditionType[] GetConditionType()
        {
            ConditionType[] ret = new ConditionType[_condition.Length];
            for (int i = 0; i < _condition.Length; i++) 
            {
                ret[i] = _condition[i].GetConditionType();
            }
            return ret;
        }
        
        public PassiveEffectType[] GetEffectType()
        {
            PassiveEffectType[] ret = new PassiveEffectType[_effect.Length];
            for (int i = 0; i < _effect.Length; i++)
            {
                ret[i] = _effect[i].GetEffectType();
            }
            return ret;
        }

        public void Delete()
        {
            for (int i = 0; i < _effect.Length; i++)
            {
                _condition[i].OnDelete();
            }
            for (int i = 0; i < _effect.Length; i++)
            {
                _effect[i].OnDelete();
            }
        }
    }
}
