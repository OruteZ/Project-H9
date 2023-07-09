using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface IUnit
{
    /// <summary>
    /// 매 프레임 호출되는 함수입니다. Update대신 UnitSystem에서 호출하여 관리하도록 합니다.
    /// </summary>
    void Updated();
    
    /// <summary>
    /// 해당 유닛이 턴을 시작합니다.
    /// </summary>
    void StartTurn();
    
    /// <summary>
    /// damage 만큼 피해를 입습니다.
    /// </summary>
    /// <param name="damage">피해량</param>
    void GetDamage(int damage);
    
    /// <summary>
    /// Unit의 상태를 설정합니다.
    /// </summary>
    /// <param name="newName">유닛의 이름</param>
    /// <param name="unitStat">유닛의 스탯</param>
    /// <param name="weaponIndex">무기의 index</param>
    void SetUp(string newName, UnitStat unitStat, int weaponIndex);
    
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
}

[System.Serializable]
public struct UnitStat
{
    public int maxHp;
    public int curHp;
    public int concentration; 
    public int sightRange; 
    public int speed;
    public int actionPoint;
    public float additionalHitRate;
    public float criticalChance;
    public int revolverAdditionalDamage;
    public int repeaterAdditionalDamage;
    public int shotgunAdditionalDamage;
    public int revolverAdditionalRange;
    public int repeaterAdditionalRange;
    public int shotgunAdditionalRange;
    public float revolverCriticalDamage;
    public float shotgunCriticalDamage;
    public float repeaterCriticalDamage;
}