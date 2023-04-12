using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private string[] unitInfo;
    [SerializeField] private GameObject unitPrefab;

    private List<Unit> _units;

    private void Awake()
    {
        _units = new List<Unit>();

        foreach (var unitName  in unitInfo)
        {
            Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
            
            unit.SetUp(unitName);
            _units.Add(unit);
        }
    }

    private void Update()
    {
        foreach (var unit in _units) unit.Updated();
    }
    
    public void EndTurn()
    {
        
    }

    public void StartTurn(Unit target)
    {
        target.StartTurn();
    }
}
