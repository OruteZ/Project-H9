using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitSystem : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDB;
    [SerializeField] private EnemyDatabase enemyDB;
    
    public List<Unit> units;
    public UnityEvent<Unit> onAnyUnitMoved;
    
    /// <summary>
    /// 자식오브젝트에 존재하는 모든 Unit을 찾아 Units에 등록합니다.
    /// </summary>
    public void SetUpUnits()
    {
        var childUnits = GetComponentsInChildren<Unit>();
        foreach (var unit in childUnits)
        {
            units.Add(unit);
        }
        
        foreach (var unit in units)
        {
            if (unit is Player p)
            {
                p.SetUp("Player", GameManager.instance.playerStat, 
                    weaponDB.Clone(GameManager.instance.playerWeaponIndex),
                    GameManager.instance.playerModel);
                if (GameManager.instance.CompareState(GameState.World))
                {
                    p.hexTransform.position = GameManager.instance.playerWorldPos;
                }
            }
            else if(unit is Enemy enemy)
            {
                var info = enemyDB.GetInfo(enemy.dataIndex);
                enemy.SetUp("Enemy", info.stat, weaponDB.Clone(info.weaponIndex), info.model);
            }
            unit.onDead.AddListener(OnUnitDead);
            unit.onMoved.AddListener(OnUnitMoved);
        }
    }

    public bool IsCombatFinish()
    {
        if (GameManager.instance.CompareState(GameState.World))
        {
            Debug.LogError("Wrong function Call : check finish combat in world scene");
            throw new NotSupportedException();
        }
        
        if (GetPlayer().GetStat().curHp <= 0) return true;
        if (units.Count == 1 && units[0] is Player) return true;

        return false;
    }
    
    /// <summary>
    /// 현재 플레이어 객체를 가져옵니다.
    /// </summary>
    /// <returns>Player reference, or null</returns>
    public Player GetPlayer()
    {
        foreach (var unit in units)
        {
            if (unit is Player u) return u;
        }
        
        Debug.LogError("Cant find Player");
        return null;
    }
    
    /// <summary>
    /// 특정 좌표에 존재하는 Unit을 가져옵니다.
    /// </summary>
    /// <param name="position">Hex 좌표</param>
    /// <returns>Unit Reference</returns>
    public Unit GetUnit(Vector3Int position)
    {
        foreach (var unit in units)
        {
            if (unit.hexPosition == position) return unit;
        }

        return null;
    }

    public void OnUnitMoved(Unit unit)
    {
        onAnyUnitMoved?.Invoke(unit);
    }

    private void OnUnitDead(Unit unit)
    {
        RemoveUnit(unit);

        if (IsCombatFinish()) GameManager.instance.FinishCombat();
    }

    private void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        unit.onMoved.RemoveListener(OnUnitMoved);
    }
}
