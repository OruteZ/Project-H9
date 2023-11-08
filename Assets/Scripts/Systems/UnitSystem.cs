using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PassiveSkill;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class UnitSystem : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDB;
    [SerializeField] private EnemyDatabase enemyDB;
    [SerializeField] private PassiveDatabase passiveDB;
    [SerializeField] private ActiveDatabase activeDB;
    [SerializeField] private BehaviourTreeDatabase aiDB;
    
    public List<Unit> units;
    public UnityEvent<Unit> onAnyUnitMoved;
    public UnityEvent<Unit> onAnyUnitDead;

    private int _totalExp;
    
    /// <summary>
    /// 자식오브젝트에 존재하는 모든 Unit을 찾아 Units에 등록합니다.
    /// </summary>
    public void SetUpUnits()
    {
        _totalExp = 0;
        
        var childUnits = GetComponentsInChildren<Unit>();
        foreach (var unit in childUnits)
        {
            units.Add(unit);
        }
        
        foreach (var unit in units)
        {
            if (unit is Player p)
            {
                #region PLYAER PASSIVE SETUP
                var playerPassiveIndexList = GameManager.instance.playerPassiveIndexList;
                if (playerPassiveIndexList == null)
                {
                    Debug.LogError("GameManager.playerPassiveList is null");
                    return;
                }
                
                var playerPassiveList = playerPassiveIndexList.Select(idx => passiveDB.GetPassive(idx, unit)).ToList();
                #endregion

                Debug.Log("???");
                var activeList = GameManager.instance.playerActiveIndexList;
                foreach (var activeIdx in activeList)
                {
                    activeDB.GetAction(p, activeIdx);
                }
                p.AddComponent<IdleAction>();

                p.SetUp("Player", GameManager.instance.playerStat, 
                    weaponDB.Clone(GameManager.instance.playerWeaponIndex),
                    GameManager.instance.playerModel, playerPassiveList);
                if (GameManager.instance.CompareState(GameState.World))
                {
                    p.hexTransform.position = GameManager.instance.playerWorldPos;
                }
            }
            else if(unit is Enemy enemy)
            {
                var info = enemyDB.GetInfo(enemy.dataIndex);
                info.stat.ResetModifier();
                
                enemy.SetUp("Enemy", (UnitStat)info.stat.Clone(), weaponDB.Clone(info.weaponIndex), info.model, new List<Passive>());
                enemy.SetupAI(aiDB.GetTree(info.btIndex));
                enemy.isVisible = false;

                _totalExp += info.rewardExp;
            }
            unit.onDead.AddListener(OnUnitDead);
            unit.onMoved.AddListener(OnUnitMoved);
        }
    }

    public bool IsCombatFinish(out bool hasPlayerWin)
    {
        if (GameManager.instance.CompareState(GameState.World))
        {
            Debug.LogError("Wrong function Call : check finish combat in world scene");
            throw new NotSupportedException();
        }

        hasPlayerWin = false;
        if (GetPlayer() is null) return true;
        if (GetPlayer().hp <= 0) return true;
        
        if (units.Count == 1 && units[0] is Player)
        {
            hasPlayerWin = true;
            return true;
        }

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
        return null;
    }
    
    /// <summary>
    /// 특정 좌표에 존재하는 Unit을 가져옵니다.
    /// </summary>
    /// <param name="position">Hex 좌표</param>
    /// <returns>Unit Reference</returns>
    public Unit GetUnit(Vector3Int position)
    {
        if (units is null) return null;
        
        foreach (var unit in units)
        {
            if (unit.hexPosition == position) return unit;
        }

        return null;
    }

    public bool TryGetUnit(Vector3Int position, out Unit unit)
    {
        unit = null;
        if (units is null)
        {
            return false;
        }
        
        foreach (var u in units)
        {
            if (u.hexPosition == position)
            {
                unit = u;
                return true;
            }
        }

        return false;
    }

    public List<Unit> GetUnitListInRange(IEnumerable<Vector3Int> positions)
    {
        var result = new List<Unit>();
        if (units is null)
        {
            return result;
        }

        foreach (var pos in positions)
        {
            if (TryGetUnit(pos, out var u))
            {
                result.Add(u);
            }
        }

        return result;
    }
    public List<Unit> GetUnitListInRange(Vector3Int start, int range)
    {
        var positions = Hex.GetCircleGridList(range, start);
        var result = new List<Unit>();
        if (units is null)
        {
            return result;
        }

        foreach (var pos in positions)
        {
            if (TryGetUnit(pos, out var u))
            {
                result.Add(u);
            }
        }

        return result;
    }

    public void OnUnitMoved(Unit unit)
    {
        onAnyUnitMoved?.Invoke(unit);
    }

    private void OnUnitDead(Unit unit)
    {
        RemoveUnit(unit);
        onAnyUnitDead.Invoke(unit);
        
        if (IsCombatFinish(out var playerWin))
        {
            if (playerWin)
            {
                LevelSystem.ReservationExp(_totalExp);
            }
            FieldSystem.onCombatFinish.Invoke(playerWin);
        }
    }

    private void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        unit.onMoved.RemoveListener(OnUnitMoved);
    }
}
