using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public enum UnitType
{
    Player,
    Enemy,
}

[RequireComponent(typeof(HexTransform))]
public abstract class Unit : MonoBehaviour
{
    [HideInInspector]
    public HexTransform hexTransform;
    
    private IUnitAction[] _unitActionArray; // All Unit Actions attached to this Unit
    private IUnitAction _activeUnitAction; // Currently active action
    public Vector3Int Position
    {
        get => hexTransform.position;
        set => hexTransform.position = value;
    }
    private int _actionPoints;
    public int speed;

    public string unitName; 
    public CombatSystem system;
    public Map map;
    public virtual void SetUp(string newName, CombatSystem _system)
    {
        map = _system.map;
        system = _system;
        unitName = newName;
    }   
    
    public abstract void Updated();
    public abstract void StartTurn();

    private void Awake()
    {
        hexTransform = GetComponent<HexTransform>();

        _unitActionArray = GetComponents<IUnitAction>();
        foreach (IUnitAction action in _unitActionArray)
        {
            action.Setup(this);
        }
    }

    public void SetActiveUnitAction(IUnitAction action)
    {
        _activeUnitAction = action;
    }
    
    public T GetAction<T>() {
        foreach (IUnitAction unitAction in _unitActionArray) {
            if (unitAction is T action) {
                return action;
            }
        }

        return default;
    }
    
    public IUnitAction[] GetUnitActionArray() {
        return _unitActionArray;
    }

    public int Mobility => speed / 10;
}

