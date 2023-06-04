using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    void Updated();
    void StartTurn();
    void GetDamage(int damage);
    void SetUp(string newName, UnitStat unitStat, int weaponIndex);
    T GetAction<T>();
    IUnitAction[] GetUnitActionArray();
}
