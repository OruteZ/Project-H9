using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CombatSystem : MonoBehaviour
{
    [Serializable]
    private struct UnitInfo
    {
        public string unitName;
        public UnitType type;
        public Vector3Int position;
    }
    
    [SerializeField] private UnityEvent onTurnChanged;
    [SerializeField] private UnitInfo[] _unitInfo;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int turnNumber;
    private List<Unit> _units;

    public Unit turnOwner;

    public Map map;

    private void Awake()
    {
        map = GetComponent<Map>();
        _units = new List<Unit>();

        foreach (var info  in _unitInfo)
        {
            Unit unit;

            switch (info.type)
            {
                case UnitType.Player:
                    unit = Instantiate(playerPrefab).GetComponent<Unit>();
                    break;
                case UnitType.Enemy:
                    unit = Instantiate(enemyPrefab).GetComponent<Unit>();
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
}
