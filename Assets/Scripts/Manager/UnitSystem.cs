using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitSystem : MonoBehaviour
{
    private List<Unit> _units;
    public UnityEvent<Unit> onAnyUnitMoved;
    
    [Serializable]
    private struct UnitInfo
    {
        public string unitName;
        public UnitType type;
        public Vector3Int spawnPosition;
    }
    ;
    [SerializeField] private UnitInfo[] unitInfo;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    private void Awake()
    {
        
        _units = new List<Unit>();
    }

    private void Update()
    {
            foreach (var unit in _units) unit.Updated();
    }

    public Player GetPlayer()
    {
        foreach (var unit in _units)
        {
            if (unit is Player u) return u;
        }
        return null;
    }
    

    public Unit GetUnit(Vector3Int position)
    {
        return _units.FirstOrDefault(unit => unit.position == position);
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
        _units.Remove(unit);
        unit.onMoved.RemoveListener(OnUnitMoved);
    }

    public void SpawnUnits()
    {
        foreach (var info  in unitInfo)
        {
            Unit unit;

            switch (info.type)
            {
                case UnitType.Player:
                    unit = Instantiate(playerPrefab).GetComponent<Unit>();
                    unit.position = info.spawnPosition;
                    break;
                case UnitType.Enemy:
                    unit = Instantiate(enemyPrefab).GetComponent<Unit>();
                    unit.position = info.spawnPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            unit.SetUp(info.unitName);
            _units.Add(unit);
            unit.onDead.AddListener(OnUnitDead);
        }
    }
}
