using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CombatSystem : MonoBehaviour
{
    private struct UnitInfo
    {
        public string unitName;
        public UnitType type;
    }
    
    [SerializeField] private UnityEvent onTurnChanged;
    [SerializeField] private UnitInfo[] _unitInfo;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    private List<Unit> _units;

    public Unit turnOwner;

    private void Awake()
    {    
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
    }

    private void Update()
    {
        foreach (var unit in _units) unit.Updated();
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
