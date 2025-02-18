using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction {
    ActionType GetActionType();

    /// <summary>
    /// UnitAction의 Damage, range, cost와 같은 값들을 설정합니다.
    /// UnitDatabase의 GetAction함수에서 Action을 반환 하기 전 호출됩니다.
    /// </summary>
    /// <param name="info"> 해당 Active Action의 정보입니다.</param>
    void SetData(ActiveInfo info);

    /// <summary>
    /// UnitAction의 초기값을 설정합니다.
    /// </summary>
    /// <param name="unit">해당 UnitAction의 주체 unit입니다.</param>
    void SetUp(Unit unit);
    
    /// <summary>
    /// 해당 UnitAction의 주체인 Unit을 반환합니다.
    /// </summary>
    /// <returns>Unit 객체</returns>
    Unit GetUnit();
    
    /// <summary>
    /// 해당 UnitAction이 실행중인지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    bool IsActive();
    
    /// <summary>
    /// 해당 UnitAction의 타겟이 되는 위치를 지정합니다.
    /// </summary>
    /// <param name="targetPos">타겟의 Hex위치</param>
    void SetTarget(Vector3Int targetPos);
    
    /// <summary>
    /// 해당 UnitAction을 unit이 실행 할 수 있는지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    bool CanExecute();
    
    bool CanExecute(Vector3Int targetPos);

    /// <summary>
    /// 해당 UnitAction을 선택할 수 있는지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    bool IsSelectable();
    
    /// <summary>
    /// UnitAction을 실행합니다.
    /// </summary>
    void Execute();
    
    /// <summary>
    /// 행동 후 해당 Action이 소모한 Cost를 반환합니다.
    /// </summary>
    /// <returns></returns>
    int GetCost();
    
    /// <summary>
    /// 해당 행동을 선택한 후 바로 실행되는지에 대한 참 / 거짓을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public bool CanExecuteImmediately();

    public int GetAmmoCost();

    public void ForceFinish();

    public void TossAnimationEvent(string eventString);

    public int GetSkillIndex();
}

/// <summary>
/// UnitAction의 종류를 나타내는 Enum입니다.
/// Action Component의 이름과 일치 시켜야 합니다.
/// 따라서 항상 Pascal Case를 유지합니다.
/// </summary>
public enum ActionType {
    None,
    Move,
    Spin,
    Attack,
    Dynamite,
    Idle,
    Reload,
    Fanning,
    Hemostasis,
    ItemUsing,
    // Cover,
    
    //Aden Boss Action
    SpreadDynamite,
    SuicideDynamite,
    
    //Heinrich Boss Action
    HeinrichTrap,
    HeinrichVanish
}