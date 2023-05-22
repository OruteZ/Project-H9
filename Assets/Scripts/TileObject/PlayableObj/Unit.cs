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
    protected TurnSystem TurnSystem => CombatManager.Instance.turnSystem;
    protected UnitSystem UnitSystem => CombatManager.Instance.unitSystem;
    
    [HideInInspector]
    public HexTransform hexTransform;
    
    private IUnitAction[] _unitActionArray; // All Unit Actions attached to this Unit
    public IUnitAction activeUnitAction; // Currently active action

    public string unitName;
    private int _actionPoints;
    public Weapon weapon;
 
    public int speed;  
    public Vector3Int Position
    {
        get => hexTransform.position;
        set => hexTransform.position = value;
    }
    public virtual void SetUp(string newName)
    {
        unitName = newName;
    }   
    
    public abstract void Updated();
    public abstract void StartTurn();
    public abstract void OnHit(int damage);

    private void Awake()
    {
        hexTransform = GetComponent<HexTransform>();

        _unitActionArray = GetComponents<IUnitAction>();
        foreach (IUnitAction action in _unitActionArray)
        {
            action.Setup(this);
        }
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

