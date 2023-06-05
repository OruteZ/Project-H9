using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction {
    ActionType GetActionType();
    
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
    
    /// <summary>
    /// UnitAction을 실행합니다.
    /// </summary>
    /// <param name="onActionComplete">UnitAction이 끝났을 때 실행 될 System.Action함수입니다. (다른 Action)</param>
    void Execute(Action onActionComplete);
    
    /// <summary>
    /// 행동 후 해당 Action이 소모한 Cost를 반환합니다.
    /// </summary>
    /// <returns></returns>
    int GetCost();
}

public enum ActionType {
    Move,
    Spin,
    Attack,
    Dynamite,
    None,
}