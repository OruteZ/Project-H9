using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitSystem : MonoBehaviour
{
    public List<Unit> units;
    public UnityEvent<Unit> onAnyUnitMoved;
    //
    // [Serializable]
    //  private struct UnitInfo
    //  {
    //      public string unitName;
    //     public UnitType type;
    //      public Vector3Int spawnPosition;
    //  };
    //  [SerializeField] private UnitInfo[] _unitInfo;
    // [SerializeField] private GameObject _playerPrefab;
    // [SerializeField] private GameObject _enemyPrefab;

    private void Awake()
    {
        var childUnits = GetComponentsInChildren<Unit>();
        foreach (var unit in childUnits)
        {
            units.Add(unit);
        }
    }

    private void Update()
    {
        foreach (var unit in units) unit.Updated();
    }

    public Player GetPlayer()
    {
        foreach (var unit in units)
        {
            if (unit is Player u) return u;
        }
        return null;
    }
    

    public Unit GetUnit(Vector3Int position)
    {
        foreach (var unit in units)
        {
            if (unit.hexPosition == position) return unit;
        }

        return null;
    }

    private void OnUnitMoved(Unit unit)
    {
        onAnyUnitMoved?.Invoke(unit);
    }

    private void OnUnitDead(Unit unit)
    {
        RemoveUnit(unit);
    }

    private void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        unit.onMoved.RemoveListener(OnUnitMoved);
    }

    public void SetUpUnits()
    {
        foreach (var unit in units)
        {
            // Unit unit;
            //
            // switch (info.type)
            // {
            //     case UnitType.Player:
            //         unit = Instantiate(_playerPrefab).GetComponent<Unit>();
            //         unit.position = info.spawnPosition;
            //         break;
            //     case UnitType.Enemy:
            //         unit = Instantiate(_enemyPrefab).GetComponent<Unit>();
            //         unit.position = info.spawnPosition;
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            if (unit is Player)
            {
                unit.SetUp("Player", GameManager.instance.playerStat, 0);
                if (GameManager.instance.CompareState(GameState.World))
                {
                    unit.hexPosition = GameManager.instance.playerWorldPos;
                }
            }
            else
            {
                unit.SetUp("EEEEEnemy", GameManager.instance.playerStat, 0);
            }
            unit.onDead.AddListener(OnUnitDead);
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
}
