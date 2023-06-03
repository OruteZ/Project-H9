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
    protected static TurnSystem turnSystem => CombatSystem.instance.turnSystem;
    protected static UnitSystem unitSystem => CombatSystem.instance.unitSystem;
    protected static TileSystem tileSystem => CombatSystem.instance.tileSystem;
    
    [HideInInspector] 
    public HexTransform hexTransform;

    //차후에 Skinned Mesh Renderer로 변경하면 됨
    public MeshRenderer visual;

    [Header("Status")] 
    public UnitStat stat;
    
    
    [HideInInspector] public UnityEvent<IUnitAction> onActionCompleted;
    [HideInInspector] public UnityEvent onBusyChanged;
    [HideInInspector] public UnityEvent<int> onCostChanged;
    [HideInInspector] public UnityEvent<Unit> onMoved;
    [HideInInspector] public UnityEvent<Unit> onDead;
    [HideInInspector] public UnityEvent<Unit, int> onHit;

    private IUnitAction[] _unitActionArray; // All Unit Actions attached to this Unit
    public IUnitAction activeUnitAction; // Currently active action
    private bool _isBusy;
    public bool isBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                onBusyChanged.Invoke();
            }
        }
    }
    
    public string unitName;
    public int currentActionPoint;
    public Weapon weapon;
    
    public abstract void Updated();
    public abstract void StartTurn();
    public abstract void GetDamage(int damage);

    public Vector3Int position
    {
        get => hexTransform.position;
        set
        {
            bool hasMoved = hexTransform.position != value;
            hexTransform.position = value;
            
            if(hasMoved) onMoved?.Invoke(this);
        }
    }
    public virtual void SetUp(string newName, UnitStat unitStat, int weaponIndex)
    {
        unitName = newName;
        stat = unitStat;
        WeaponData newWeaponData = WeaponManager.GetWeaponData(weaponIndex);
        weapon = Weapon.Clone(newWeaponData, unit : this);
    }   

    protected void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        visual = GetComponent<MeshRenderer>();
        _unitActionArray = GetComponents<IUnitAction>();
    }

    private void Start()
    {
        foreach (IUnitAction action in _unitActionArray)
        {
            action.Setup(this);
        }
    }
    
    public T GetAction<T>()
    {
        foreach (IUnitAction unitAction in _unitActionArray)
        {
            if (unitAction is T action)
            {
                return action;
            }
        }

        return default;
    }
    
    public IUnitAction[] GetUnitActionArray() 
    {
        return _unitActionArray;
    }

    public bool isVisible
    {
        get => visual.enabled;
        set => visual.enabled = value;
    }

    public bool IsMyTurn()
    {
        return turnSystem.turnOwner == this;
    }
}

[Serializable]
public struct UnitStat
{
    public int hp;
    public int concentration; 
    public int sightRange; 
    public int speed;
    public int actionPoint;
    public float additionalHitRate;
    public float criticalChance;
    public int revolverAdditionalDamage;
    public int repeaterAdditionalDamage;
    public int shotgunAdditionalDamage;
    public int revolverAdditionalRange;
    public int repeaterAdditionalRange;
    public int shotgunAdditionalRange;
    public float revolverCriticalDamage;
    public float shotgunCriticalDamage;
    public float repeaterCriticalDamage;
}

