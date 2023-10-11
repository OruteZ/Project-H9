// using System;
// using System.Collections.Generic;
//
// public class StatValueController
// {
//     private readonly List<float> _additionalValueList;
//     private readonly List<float> _multiplyValueList;
//     private Unit _unit;
//
//     StatValueController(Unit unit)
//     {
//         _additionalValueList = new List<float>((int)UnitStatType.Length - 3);
//         _multiplyValueList = new List<float>((int)UnitStatType.Length - 3);
//         _unit = unit;
//     }
//
//     public void NewAdditionalValue(UnitStatType type, float value)
//     {
//         if (type is UnitStatType.AllAdditionalDamage)
//         {
//             _additionalValueList[(int)UnitStatType.RevolverAdditionalDamage] += value;
//             _additionalValueList[(int)UnitStatType.RepeaterAdditionalDamage] += value;
//             _additionalValueList[(int)UnitStatType.ShotgunAdditionalDamage] += value;
//             return;
//         }
//
//         if (type is UnitStatType.AllAdditionalRange)
//         {
//             _additionalValueList[(int)UnitStatType.RevolverAdditionalRange] += value;
//             _additionalValueList[(int)UnitStatType.RepeaterAdditionalRange] += value;
//             _additionalValueList[(int)UnitStatType.ShotgunAdditionalRange] += value;
//             return;
//         }
//
//         if (type is UnitStatType.AllCriticalDamage)
//         {
//             _additionalValueList[(int)UnitStatType.RevolverCriticalDamage] += value;
//             _additionalValueList[(int)UnitStatType.RepeaterCriticalDamage] += value;
//             _additionalValueList[(int)UnitStatType.ShotgunCriticalDamage] += value;
//             return;
//         }
//
//         _additionalValueList[(int)type] += value;
//     }
//
//     public void NewMultiplyValue(UnitStatType type, float value)
//     {
//         if (type is UnitStatType.AllAdditionalDamage)
//         {
//             _multiplyValueList[(int)UnitStatType.RevolverAdditionalDamage] += value;
//             _multiplyValueList[(int)UnitStatType.RepeaterAdditionalDamage] += value;
//             _multiplyValueList[(int)UnitStatType.ShotgunAdditionalDamage] += value;
//             return;
//         }
//
//         if (type is UnitStatType.AllAdditionalRange)
//         {
//             _multiplyValueList[(int)UnitStatType.RevolverAdditionalRange] += value;
//             _multiplyValueList[(int)UnitStatType.RepeaterAdditionalRange] += value;
//             _multiplyValueList[(int)UnitStatType.ShotgunAdditionalRange] += value;
//             return;
//         }
//
//         if (type is UnitStatType.AllCriticalDamage)
//         {
//             _multiplyValueList[(int)UnitStatType.RevolverCriticalDamage] += value;
//             _multiplyValueList[(int)UnitStatType.RepeaterCriticalDamage] += value;
//             _multiplyValueList[(int)UnitStatType.ShotgunCriticalDamage] += value;
//             return;
//         }
//
//         _multiplyValueList[(int)type] += value;
//     }
//
//     public void DelAdditionalValue(UnitStatType type, float value)
//     {
//         if (type is UnitStatType.AllAdditionalDamage)
//         {
//             _additionalValueList[(int)UnitStatType.RevolverAdditionalDamage] -= value;
//             _additionalValueList[(int)UnitStatType.RepeaterAdditionalDamage] -= value;
//             _additionalValueList[(int)UnitStatType.ShotgunAdditionalDamage] -= value;
//             return;
//         }
//
//         if (type is UnitStatType.AllAdditionalRange)
//         {
//             _additionalValueList[(int)UnitStatType.RevolverAdditionalRange] -= value;
//             _additionalValueList[(int)UnitStatType.RepeaterAdditionalRange] -= value;
//             _additionalValueList[(int)UnitStatType.ShotgunAdditionalRange] -= value;
//             return;
//         }
//
//         if (type is UnitStatType.AllCriticalDamage)
//         {
//             _additionalValueList[(int)UnitStatType.RevolverCriticalDamage] -= value;
//             _additionalValueList[(int)UnitStatType.RepeaterCriticalDamage] -= value;
//             _additionalValueList[(int)UnitStatType.ShotgunCriticalDamage]-= value;
//             return;
//         }
//
//         _additionalValueList[(int)type] -= value;
//     }
//
//     public void DelMultiplyValue(UnitStatType type, float value)
//     {
//         if (type is UnitStatType.AllAdditionalDamage)
//         {
//             _multiplyValueList[(int)UnitStatType.RevolverAdditionalDamage] -= value;
//             _multiplyValueList[(int)UnitStatType.RepeaterAdditionalDamage] -= value;
//             _multiplyValueList[(int)UnitStatType.ShotgunAdditionalDamage] -= value;
//             return;
//         }
//
//         if (type is UnitStatType.AllAdditionalRange)
//         {
//             _multiplyValueList[(int)UnitStatType.RevolverAdditionalRange] -= value;
//             _multiplyValueList[(int)UnitStatType.RepeaterAdditionalRange] -= value;
//             _multiplyValueList[(int)UnitStatType.ShotgunAdditionalRange] -= value;
//             return;
//         }
//
//         if (type is UnitStatType.AllCriticalDamage)
//         {
//             _multiplyValueList[(int)UnitStatType.RevolverCriticalDamage] -= value;
//             _multiplyValueList[(int)UnitStatType.RepeaterCriticalDamage] -= value;
//             _multiplyValueList[(int)UnitStatType.ShotgunCriticalDamage] -= value;
//             return;
//         }
//
//         _multiplyValueList[(int)type] -= value;
//     }
//     
//     #region PRIVATE
//
//     private void GotoOriginal(UnitStatType type)
//     {
//         int index = (int)type;
//         float target = type switch
//         {
//             UnitStatType.MaxHp => _unit.GetStat().maxHp,
//             UnitStatType.CurHp => _unit.GetStat().curHp,
//             UnitStatType.Concentration => _unit.GetStat().concentration,
//             UnitStatType.SightRange => _unit.GetStat().sightRange,
//             UnitStatType.Speed => _unit.GetStat().speed,
//             UnitStatType.ActionPoint => _unit.currentActionPoint,
//             UnitStatType.AdditionalHitRate => _unit.GetStat().additionalHitRate,
//             UnitStatType.CriticalChance => _unit.GetStat().criticalChance,
//             UnitStatType.RevolverAdditionalDamage => _unit.GetStat().revolverAdditionalDamage,
//             UnitStatType.RepeaterAdditionalDamage => _unit.GetStat().repeaterAdditionalDamage,
//             UnitStatType.ShotgunAdditionalDamage => _unit.GetStat().shotgunAdditionalDamage,
//             UnitStatType.RevolverAdditionalRange => _unit.GetStat().revolverAdditionalRange,
//             UnitStatType.RepeaterAdditionalRange => _unit.GetStat().repeaterAdditionalRange,
//             UnitStatType.ShotgunAdditionalRange => _unit.GetStat().shotgunAdditionalRange,
//             UnitStatType.RevolverCriticalDamage => _unit.GetStat().revolverCriticalDamage,
//             UnitStatType.RepeaterCriticalDamage => _unit.GetStat().repeaterCriticalDamage,
//             UnitStatType.ShotgunCriticalDamage => _unit.GetStat().shotgunCriticalDamage,
//             _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
//         };
//     }
//
//     private void ApplyValue(UnitStatType type)
//     {
//         float value;
//     }
//     
//     #endregion
// }
