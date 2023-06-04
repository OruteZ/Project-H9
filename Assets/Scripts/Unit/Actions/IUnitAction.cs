using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction {
    ActionType GetActionType();
    void SetUp(Unit unit);
    Unit GetUnit();
    bool IsActive();
    void SetTarget(Vector3Int targetPos);
    bool CanExecute();
    void Execute(Action onActionComplete);
    int GetCost();
}

public enum ActionType {
    Move,
    Spin,
    Attack,
    Dynamite,
    FinishTurn,
}