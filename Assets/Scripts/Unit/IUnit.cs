using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

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
    void GetDamage(int damage);

    /// <summary>
    /// Unit의 상태를 설정합니다.
    /// </summary>
    /// <param name="newName">유닛의 이름</param>
    /// <param name="unitStat">유닛의 스탯</param>
    /// <param name="newWeapon">무기</param>
    /// <param name="unitModel"></param>
    void SetUp(string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel);
    
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
    [Header("추가 데미지")]
    public int revolverAdditionalDamage;
    public int repeaterAdditionalDamage;
    public int shotgunAdditionalDamage;
    [Header("추가 사거리")]
    public int revolverAdditionalRange;
    public int repeaterAdditionalRange;
    public int shotgunAdditionalRange;
    [Header("크리티컬 데미지")]
    public float revolverCriticalDamage;
    public float shotgunCriticalDamage;
    public float repeaterCriticalDamage;
}