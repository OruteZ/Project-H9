using System.Collections.Generic;
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
    public Transform chest => _unitModel.chest;
    public Transform waist => _unitModel.waist;
    
    public Animator animator => _unitModel.animator;
    
    public bool isVisible
    {
        get => _unitModel.isVisible;
        set => _unitModel.isVisible = value;
    }
    #endregion

    public string unitName;
    
    public HexTransform hexTransform;
    
    private UnitModel _unitModel;
    public UnitStat stat;
    public Weapon weapon;

    private List<Passive> _passiveList;
    public bool hasAttacked;
    public int currentRound;

    // ReSharper disable once InconsistentNaming
    public static readonly UnityEvent<Unit> onAnyUnitActionFinished = new UnityEvent<Unit>();
    [HideInInspector] public UnityEvent<Unit> onTurnStart; // me
    [HideInInspector] public UnityEvent<Unit> onTurnEnd; // me
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
        _unitModel = model.GetComponent<UnitModel>();
        _unitModel.Setup(this);
        
        EquipWeapon(newWeapon);

        onFinishAction.AddListener((action) => onAnyUnitActionFinished.Invoke(this));

        _seController = new UnitStatusEffectController(this);
    }

    public virtual void StartTurn()
    {
#if UNITY_EDITOR
        Debug.Log(unitName + " Turn Started");
#endif
        
        hasAttacked = false;
        
        onTurnStart.Invoke(this);
        
        stat.Recover(StatType.CurActionPoint, stat.maxActionPoint);
        
        if (hp <= 0) DeadCall(this);
        else SelectAction(GetAction<IdleAction>());
    }

    public void EndTurn()
    {
#if UNITY_EDITOR
        Debug.Log(unitName + " Turn Ended");
#endif
        onTurnEnd.Invoke(this);
        FieldSystem.turnSystem.EndTurn();
    }

    public virtual void TakeDamage(int damage, Unit attacker)
    {
        if (gameObject == null) return;
        
        stat.Consume(StatType.CurHp, damage);
        onHit.Invoke(FieldSystem.turnSystem.turnOwner, damage);

        Service.SetText(damage.ToString(), transform.position);

        if (hp <= 0 && _hasDead is false)
        {
            _hasDead = true;
            onAnyUnitActionFinished.AddListener(DeadCall);

            _killer = attacker;
        }
    }

    private Unit _killer;
    public void DeadCall(Unit unit)
    {
        onDead.Invoke(this);

        // ReSharper disable once Unity.NoNullPropagation
        _killer?.onKill.Invoke(this);

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
        else
        {
            _unitModel.SetupWeaponModel(newWeapon);
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
            var gunpointPos = _unitModel.GetGunpointPosition();
            VFXManager.instance.TryInstantiate(fxGunFireKey, fxGunFireTime, gunpointPos);
        }
        if (VFXHelper.TryGetTraceOfBulletFXKey(weapon.GetWeaponType(), out var fxBulletLine, out var traceTime))
        {
            var startPos = _unitModel.GetGunpointPosition();
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
        if (HasStatusEffect(StatusEffectType.Stun)) return;
        
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

    #region STATUE EFFECT

    private UnitStatusEffectController _seController;

    public bool HasStatusEffect(StatusEffectType type)
    {
        return _seController.HasStatusEffect(type);
    }

    public bool TryAddStatus(StatusEffect effect)
    {
        if (effect.GetDuration() <= 0 && effect.GetStatusEffectType() is not StatusEffectType.Bleeding)
        {
            Debug.LogError("지속시간이 0 이하인 상태이상");
            return false;
        }
        if (effect.GetStatusEffectType() is StatusEffectType.Bleeding or StatusEffectType.Burning)
        {
            if (effect.GetStack() <= 0)
            {
                Debug.LogError("출혈 또는 화상에 데미지 0");
                return false;
            }
        }
        
        _seController.AddStatusEffect(effect);
        UIManager.instance.onPlayerStatChanged.Invoke();
        return true;
    }

    public bool TryRemoveStatus(StatusEffectType type)
    {
        if (_seController.HasStatusEffect(type) is false)
        {
            return false;
        }
        
        _seController.RemoveStatusEffect(type);
        return true;
    }

    public bool TryRemoveStatus(StatusEffect effect)
    {
        if (_seController.HasStatusEffect(effect.GetStatusEffectType()) is false)
        {
            return false;
        }

        _seController.RemoveStatusEffect(effect);
        return true;
    }

    #endregion
}

