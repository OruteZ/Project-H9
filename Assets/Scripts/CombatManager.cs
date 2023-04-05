using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private string[] unitNamesArray;
    [SerializeField] private GameObject unitPrefab;

    private List<Unit> _units;

    private void Awake()
    {
        _units = new List<Unit>();

        foreach (var unitName  in unitNamesArray)
        {
            Unit unit = Instantiate(unitPrefab).GetComponent<Unit>();
            
            unit.Setup(unitName);
            _units.Add(unit);
        }
    }

    private void Update()
    {
        foreach (Unit unit in _units) unit.Updated();
    }
    
    public void EndTurn()
    {
        
    }

    public void StartTurn(Unit target)
    {
        target.StartTurn();
    }
}
