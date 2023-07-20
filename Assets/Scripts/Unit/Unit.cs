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
public abstract class Unit : MonoBehaviour, IUnit
{
    [HideInInspector] 
    public HexTransform hexTransform;

    //차후에 Skinned Mesh Renderer로 변경하면 됨
    public MeshRenderer visual;

    [Header("Status")] 
    [SerializeField] protected UnitStat stat;
    
    [HideInInspector] public UnityEvent<IUnitAction> onActionCompleted;
    [HideInInspector] public UnityEvent onBusyChanged;
    [HideInInspector] public UnityEvent<int> onCostChanged;
    [HideInInspector] public UnityEvent<Unit> onMoved;
    [HideInInspector] public UnityEvent<Unit> onDead;
    [HideInInspector] public UnityEvent<Unit, int> onHit;
    [HideInInspector] public UnityEvent<Unit, int> onSuccessAttack;
    [HideInInspector] public UnityEvent<Unit, int> onCriticalAttack;

    private IUnitAction[] _unitActionArray; // All Unit Actions attached to this Unit
    protected IUnitAction activeUnitAction; // Currently active action
    private bool _isBusy;
    
    public string unitName;
    public int currentActionPoint;
    public Weapon weapon;
    public abstract void StartTurn();
    public abstract void GetDamage(int damage);

    public bool hasAttacked;
    
    public Vector3Int hexPosition
    {
        get => hexTransform.position;
        set
        {
            bool hasMoved = hexTransform.position != value;
            hexTransform.position = value;
            
            if(hasMoved) onMoved?.Invoke(this);
        }
    }
    public virtual void SetUp(string newName, UnitStat unitStat, Weapon newWeapon)
    {
        unitName = newName;
        stat = unitStat;
        stat.curHp = stat.maxHp;

        EquipWeapon(newWeapon);
    }

    private void EquipWeapon(Weapon newWeapon)
    {
        newWeapon.unit = this;
        newWeapon.unitStat = stat;

        weapon = newWeapon;
    }

    protected virtual void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
        visual = GetComponent<MeshRenderer>();
        _unitActionArray = GetComponents<IUnitAction>();
    }

    private void Start()
    {
        foreach (IUnitAction action in _unitActionArray)
        {
            action.SetUp(this);
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

    public UnitStat GetStat()
    {
        return stat;
    }

    public bool isVisible
    {
        get => visual.enabled;
        set => visual.enabled = value;
    }

    protected bool IsMyTurn()
    {
        return FieldSystem.turnSystem.turnOwner == this;
    }

    protected void SetBusy()
    {
        bool hasChanged = _isBusy is false;
        _isBusy = true;
        
        if(hasChanged) onBusyChanged.Invoke();
    }

    protected void ClearBusy()
    {
        bool hasChanged = _isBusy;
        _isBusy = false;
        
        if(hasChanged) onBusyChanged.Invoke();
    }

    public bool IsBusy()
    {
        return _isBusy;
    }

    protected bool TryExecuteUnitAction(Vector3Int targetPosition, Action onActionFinish)
    {
        activeUnitAction.SetTarget(targetPosition);

        if (activeUnitAction.CanExecute() is not true)
        {
            Debug.Log("Can't Execute");
            return false;
        }
        
        activeUnitAction.Execute(onActionFinish);
        return true;
    }
    
    public IUnitAction GetSelectedAction()
    {
        return activeUnitAction;
    }
}

