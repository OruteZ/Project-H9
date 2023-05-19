using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CombatSystem : Generic.Singleton<CombatSystem>
{
    [Serializable]
    private struct UnitInfo
    {
        public string unitName;
        public UnitType type;
        public Vector3Int spawnPosition;
    }

    public UnityEvent onTurnChanged;
    [SerializeField] private UnitInfo[] unitInfo;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int turnNumber;
    private List<Unit> _units;

    public Unit turnOwner;

    public Map map;

    public Player Player
    {
        get
        {
            foreach (var unit in _units)
            {
                if (unit is Player u)
                    return u;
            }

            return null;
        }
    }

    public bool IsPlayerTurn()
    {
        return Player == turnOwner;
    }
    private void Awake()
    {
        base.Awake();
        
        map = GetComponent<Map>();
        _units = new List<Unit>();

        foreach (var info  in unitInfo)
        {
            Unit unit;

            switch (info.type)
            {
                case UnitType.Player:
                    unit = Instantiate(playerPrefab).GetComponent<Unit>();
                    unit.Position = info.spawnPosition;
                    break;
                case UnitType.Enemy:
                    unit = Instantiate(enemyPrefab).GetComponent<Unit>();
                    unit.Position = info.spawnPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            unit.SetUp(info.unitName, this);
            _units.Add(unit);
        }

        turnNumber = 0;
        onTurnChanged.AddListener(() => { turnNumber++;});
    }

    private void Update()
    {
        foreach (var unit in _units) unit.Updated();
    }

    private void Start()
    {
        EndTurn();
    }
    
    public void EndTurn()
    {
        //todo : if combat has finished, End Combat Scene
        //else

        CalculateTarget();
        StartTurn();
    }
    
    public void StartTurn()
    {
        turnOwner.StartTurn();
        onTurnChanged.Invoke();
    }

    public void CalculateTarget()
    {
        turnOwner = _units[0];
    }

    public Unit GetUnit(Vector3Int position)
    {
        return _units.FirstOrDefault(unit => unit.Position == position);
    }
}
