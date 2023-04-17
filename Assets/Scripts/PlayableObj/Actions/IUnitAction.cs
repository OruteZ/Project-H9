using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction {
    ActionType GetActionType();
    void Setup(Unit unit);
    Unit GetUnit();
    bool IsActive();
    bool CanExecute(Vector3Int target);
    int GetCost();
    void Execute(Vector3Int targetPos, Action onActionComplete);
}

public enum ActionType {
    Move,
    Spin,
    Shoot,
    Dynamite,
}