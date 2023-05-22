using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;

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
    protected TileSystem TileSystem => CombatManager.Instance.tileSystem;
    
    [HideInInspector] 
    public HexTransform hexTransform;

    //차후에 Skinned Mesh Renderer로 변경하면 됨
    public MeshRenderer visual;

    
    [HideInInspector] public UnityEvent<IUnitAction> onActionCompleted;
    [HideInInspector] public UnityEvent onBusyChanged;
    [HideInInspector] public UnityEvent<int> onCostChanged;
    [HideInInspector] public UnityEvent<Unit> onMoved;
    [HideInInspector] public UnityEvent<Unit> onDead;

    private IUnitAction[] _unitActionArray; // All Unit Actions attached to this Unit
    public IUnitAction activeUnitAction; // Currently active action

    public string unitName;
    private int _actionPoints;
    public Weapon weapon;
 
    public int speed;
    public int sightRange;
    public Vector3Int Position
    {
        get => hexTransform.position;
        set
        {
            bool hasMoved = hexTransform.position != value;
            hexTransform.position = value;
            
            if(hasMoved) onMoved?.Invoke(this);
        }
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
        visual = GetComponent<MeshRenderer>();

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

    public bool IsVisible
    {
        get => visual.enabled;
        set => visual.enabled = value;
    } 
}

