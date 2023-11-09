using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Castle.Core;
using Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using PassiveSkill;

public enum UnitType
{
    Player,
    Enemy,
}

[RequireComponent(typeof(HexTransform))]
public abstract class Unit : MonoBehaviour, IUnit
{
    #region SIMPLIFY
    public int currentActionPoint => stat.GetStat(StatType.CurActionPoint);
    public int hp => stat.GetStat(StatType.CurHp);

    public Transform hand => _unitModel.hand;
    public Transform back => _unitModel.back;
    public Transform waist => _unitModel.waist;
    #endregion

    public string unitName;
    
    public HexTransform hexTransform;

    //차후에 Skinned Mesh Renderer로 변경하면 됨
    public SkinnedMeshRenderer visual;
    public WeaponModel weaponModel;
    
    private UnitModel _unitModel;
    public UnitStat stat;
    public Weapon weapon;

    private List<Passive> _passiveList;
    public bool hasAttacked;
    public int currentRound;

    // ReSharper disable once InconsistentNaming
    public static readonly UnityEvent<Unit> onAnyUnitActionFinished = new UnityEvent<Unit>();
    [HideInInspector] public UnityEvent<IUnitAction> onFinishAction; //action
    [HideInInspector] public UnityEvent onBusyChanged;
    [HideInInspector] public UnityEvent<int, int> onCostChanged; // before, after
    [HideInInspector] public UnityEvent<int, int> onAmmoChanged; // before, after
    [HideInInspector] public UnityEvent<int, int> onHpChanged; // before, after
    [HideInInspector] public UnityEvent<Unit> onMoved; // me
    [HideInInspector] public UnityEvent<Unit> onDead; //me
    [HideInInspector] public UnityEvent<Unit, int> onHit; // attacker, damage
    [HideInInspector] public UnityEvent<Unit> onStartShoot; // target
    [HideInInspector] public UnityEvent<Unit, int, bool, bool> onFinishShoot; // target, totalDamage, isHit, isCritical
    [HideInInspector] public UnityEvent<Unit> onKill; // target
    [HideInInspector] public UnityEvent onSelectedChanged;
    [HideInInspector] public UnityEvent onStatusEffectChanged;

    private IUnitAction[] _unitActionArray; // All Unit Actions attached to this Unit
    protected IUnitAction activeUnitAction; // Currently active action
    private bool _isBusy;
    private bool _hasDead;

    public virtual void SetUp(string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel,
        List<Passive> passiveList)
    {
        unitName = newName;
        stat = unitStat;

        _unitActionArray = GetComponents<IUnitAction>();
        foreach (IUnitAction action in _unitActionArray)
        {
            action.SetUp(this);
        }

        _passiveList = passiveList;
        foreach (var passive in _passiveList)
        {
            if (passive is null)
            {
                Debug.LogError("passive is null");
                break;
            }

            passive.Setup();
        }

        var model = Instantiate(unitModel, transform);
        visual = model.GetComponentInChildren<SkinnedMeshRenderer>();
        _unitModel = model.GetComponent<UnitModel>();
        
        EquipWeapon(newWeapon);
        // FieldSystem.onCombatAwake.AddListener(() => {animator.SetTrigger(START);});

        onFinishAction.AddListener((action) => onAnyUnitActionFinished.Invoke(this));

        _seController = new UnitStatusEffectController(this);
    }

    public abstract void StartTurn();

    public virtual void TakeDamage(int damage)
    {
        if (gameObject == null) return;

        animator.SetTrigger(GET_HIT1);
        
        stat.Consume(StatType.CurHp, damage);
        onHit.Invoke(this, damage);

        Service.SetText(damage.ToString(), transform.position);

        if (hp <= 0 && _hasDead is false)
        {
            _hasDead = true;
            animator.SetBool(DIE, true);
            onAnyUnitActionFinished.AddListener(DeadCall);

            _attacker = FieldSystem.turnSystem.turnOwner;
        }
    }

    private Unit _attacker = null;
    public void DeadCall(Unit unit)
    {
        onDead.Invoke(this);
        _attacker.onKill.Invoke(this);
        
        onAnyUnitActionFinished.RemoveListener(DeadCall);
        Invoke(nameof(DestroyThis), 2f);
    }
    
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

    private void EquipWeapon(Weapon newWeapon)
    {
        newWeapon.unit = this;
        weapon = newWeapon;

        if (weapon.model == null)
        {
            Debug.LogError("Weapon Model Is NULL");
        }
        
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            weaponModel = Instantiate(weapon.model, hand).GetComponent<WeaponModel>();
            weaponModel.SetHandPosRot();
            newWeapon.weaponModel = weaponModel;
            SetAnimatorController(weapon.GetWeaponType());
        }
        else
        {
            weaponModel = Instantiate(weapon.model, waist).GetComponent<WeaponModel>();
            weaponModel.SetStandPosRot();
            newWeapon.weaponModel = weaponModel;
            SetAnimatorController(WeaponType.Null);
        }
    }

    protected virtual void Awake()
    {
        hexTransform = GetComponent<HexTransform>();
    }

    private void Start()
    {
        _hasDead = false;
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

    public IDisplayableEffect[] GetDisplayableEffects()
    {
        //search passive that has displayable effect
        var displayableEffects = new List<IDisplayableEffect>();
        foreach (var passive in _passiveList)
        {
            if (passive.TryGetDisplayableEffect(out var displayableEffect))
            {
                displayableEffects.Add(displayableEffect);
            }
        }
        
        //search status effect that has displayable effect
        var statusEffects = _seController.GetAllStatusEffectInfo();
        if (statusEffects is not null)
        {
            displayableEffects.AddRange(statusEffects);
        }
        
        return displayableEffects.ToArray();
    }

    public bool isVisible
    {
        get => visual.enabled;
        set
        {
            visual.enabled = value;
            weaponModel.visual = value;
        }
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
    public bool HasDead() 
    {
        return _hasDead;
    }

    protected bool TryExecuteUnitAction(Vector3Int targetPosition)
    {
        if (activeUnitAction is null)
        {
            Debug.Log("Active action is null");
            return false;
        }

        activeUnitAction.SetTarget(targetPosition);

        if (activeUnitAction.CanExecute() is not true)
        {
            Debug.Log("Can't Execute");
            return false;
        }

        if (activeUnitAction.IsActive())
        {
            Debug.Log("Already Executing");
            return false;
        }

        activeUnitAction.Execute();
        return true;
    }
    
    public IUnitAction GetSelectedAction()
    {
        if (activeUnitAction is null) return GetAction<IdleAction>();
        return activeUnitAction;
    }

    public bool TryAttack(Unit target, float hitRateOffset)
    {
        onStartShoot.Invoke(target);

        bool isCritical = false;
        bool hit = weapon.GetFinalHitRate(target) + hitRateOffset > UnityEngine.Random.value * 100;

        if (VFXHelper.TryGetGunFireFXInfo(weapon.GetWeaponType(), out var fxGunFireKey, out var fxGunFireTime))
        {
            var gunpointPos = weapon.weaponModel.GetGunpointPosition();
            VFXManager.instance.TryInstantiate(fxGunFireKey, fxGunFireTime, gunpointPos);
        }
        if (VFXHelper.TryGetTraceOfBulletFXKey(weapon.GetWeaponType(), out var fxBulletLine, out var traceTime))
        {
            var startPos = weapon.weaponModel.GetGunpointPosition();
            var destPos = target.transform.position + Vector3.up;
            if (!hit) destPos += new Vector3(UnityEngine.Random.value*2-1, UnityEngine.Random.value*2-1, UnityEngine.Random.value*2-1);
            VFXManager.instance.TryLineRender(fxBulletLine, traceTime, startPos, destPos);
        }

        if (hit)
        {
            weapon.Attack(target, out isCritical);
            var existKey = VFXHelper.TryGetBloodingFXKey(weapon.GetWeaponType(), out var fxBloodingKey, out var bloodingTime);
            if (existKey)
            {
                var targetPos = target.transform.position + Vector3.up;
                VFXManager.instance.TryInstantiate(fxBloodingKey, bloodingTime, targetPos);
            }
        }
        else
        {
            Service.SetText("MISS", target.transform.position);
        }
        weapon.currentAmmo--;

        int damage = hit ? isCritical ? weapon.GetFinalCriticalDamage() : weapon.GetFinalDamage() : 0;
        onFinishShoot.Invoke(target, damage, hit, isCritical);
        return hit;
    }
    
    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    public void ConsumeCost(int value)
    {
        if (stat.TryConsume(StatType.CurActionPoint, value) is false)
        {
            Debug.LogError("    Consumed More Cost");
            return;
        }

        if (value == 0) return;

        onCostChanged.Invoke(currentActionPoint + value, currentActionPoint);
    }

    public void SelectAction(IUnitAction action)
    {
        if (IsBusy()) return;
        if (IsMyTurn() is false) return;
        if (action.IsSelectable() is false) return;
        if (action.GetCost() > currentActionPoint)
        {  
            Debug.Log("Cost is loss, Cost is " + action.GetCost());
            return;
        }
#if UNITY_EDITOR
        Debug.Log("Select Action : " + action);
#endif

        activeUnitAction = action;
        onSelectedChanged.Invoke();

        if (activeUnitAction.CanExecuteImmediately())
        { 
            if (activeUnitAction is not IdleAction) SetBusy();
            var actionSuccess = TryExecuteUnitAction(Vector3Int.zero);
            Debug.Log("actionSuccess: " + actionSuccess);
            
            if(actionSuccess is false) ClearBusy();
        }
    }

    public void FinishAction()
    {
        var action = activeUnitAction;
        if(activeUnitAction is not MoveAction) ConsumeCost(activeUnitAction.GetCost());
        
        ClearBusy();
        if(GameManager.instance.CompareState(GameState.Combat))
        {
            var idleAction = GetAction<IdleAction>();
            SelectAction(idleAction is null ? GetAction<MoveAction>() : idleAction);
        }
        else
        {
            SelectAction(GetAction<IdleAction>());
            SelectAction(GetAction<MoveAction>());
        }
        onFinishAction.Invoke(action);
    }
    
    #region ANIMATION
    private Animator _animator;
    private static readonly int GET_HIT1 = Animator.StringToHash("GetHit1");
    private static readonly int DIE = Animator.StringToHash("Die");

    public Animator animator
    {
        get
        {
            if (_animator is null)
            {
                if (TryGetComponent(out _animator) is false)
                {
                    _animator = GetComponentInChildren<Animator>();
                }
            }

            return _animator;
        }
    }
    
    private void SetAnimatorController(WeaponType type)
    {
        animator.runtimeAnimatorController =
            (RuntimeAnimatorController)Resources.Load("Animator/" + (type is WeaponType.Null ? "Standing" : type) + " Animator Controller");
    }
    #endregion ANIMATION
    
    #region STATUE EFFECT

    private UnitStatusEffectController _seController;

    public bool HasStatus(StatusEffectType type)
    {
        return _seController.HasStatusEffect(type);
    }

    #endregion
}

