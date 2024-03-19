using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NSubstitute.Exceptions;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing.Extension;

public interface IUnit
{
    /// <summary>
    /// 해당 유닛이 턴을 시작합니다.
    /// </summary>
    void StartTurn();

    /// <summary>
    /// damage 만큼 피해를 입습니다.
    /// </summary>
    /// <param name="damage">피해량</param>
    /// <param name="attacker">공격을 가한 개체입니다.</param>
    void TakeDamage(int damage, Unit attacker = null, eDamageType.Type type = eDamageType.Type.Default);

    /// <summary>
    /// Unit의 상태를 설정합니다.
    /// </summary>
    /// <param name="newName">유닛의 이름</param>
    /// <param name="unitStat">유닛의 스탯</param>
    /// <param name="newWeapon">무기</param>
    /// <param name="unitModel"></param>
    /// <param name="passiveList">패시브 스킬의 List</param>
    void SetUp(string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel, List<Passive> passiveList);
    
    /// <summary>
    /// 특정 타입에 해당하는 액션을 반환합니다. 가지고 있지 않을경우 NoneAction을 반환합니다.
    /// </summary>
    /// <typeparam name="T">Action의 타입입니다.</typeparam>
    /// <returns>유닛이 가진 해당 Action을 반환합니다.</returns>
    T GetAction<T>();
    
    /// <summary>
    /// 해당 Unit이 가진 모든 Action을 반환합니다.
    /// </summary>
    /// <returns>UnitAction의 배열</returns>
    IUnitAction[] GetUnitActionArray();
    
    /// <summary>
    /// 현재 UI에 표시할 모든 상태이상, 패시브등을 반환합니다.
    /// </summary>
    /// <returns></returns>
    IDisplayableEffect[] GetDisplayableEffects();
}
