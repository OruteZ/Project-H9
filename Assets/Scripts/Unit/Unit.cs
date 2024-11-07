using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PassiveSkill;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum UnitType
{
    Player,
    Enemy,
}

[RequireComponent(typeof(HexTransform))]
public abstract class Unit : MonoBehaviour, IUnit, IDamageable
{
    #region SIMPLIFY

    public int currentActionPoint => stat.GetOriginalStat(StatType.CurActionPoint);
    public int hp => stat.GetOriginalStat(StatType.CurHp);

    public Transform hand => _unitModel.hand;
    public Transform chest => _unitModel.chest;
    public Transform waist => _unitModel.waist;

    public Animator animator => _unitModel.animator;

    public bool isVisible
    {
        get => _unitModel.isVisible;
        set
        {
            if (vanishTrigger) _unitModel.isVisible = false;
            else _unitModel.isVisible = value;
        }
    }

    #endregion

    private int _index; // 플레이어는 인덱스를 쓰지 않지만, 퀘스트 등 "이름"이 아닌 식별코드가 필요하여 index를 field로 추가해둠. 임시로 player는 -1 인덱스를 갖게 함.
    public int Index => _index;
    public string unitName;

    public HexTransform hexTransform;

    private UnitModel _unitModel;
    public UnitStat stat;
    public Weapon weapon;

    private List<Passive> _passiveList;
    public List<int> passiveIndexList;

    #region Skill_Effects
    private readonly List<int> _shootCntList = new(){1};
    public int maximumShootCountInTurn
    {
        get => _shootCntList.Max();
        set
        {
            // removing
            if (value < 0)
            {
                _shootCntList.Remove(value);
                return;
            }
            
            if (_shootCntList.Contains(value)) return;
            _shootCntList.Add(value);
        }
    }

    public bool isAimming = false;
    public bool vanishTrigger = false;
    
    public bool infiniteActionPointTrigger = false;
    public bool lightFootTrigger = false;

    public float tacticalReloadChance = 0;
    public bool freeReloadTrigger = false;

    public float goldenBulletChance = 0;
    public int goldenBulletCount = 0;
    public BulletData goldenBulletEffect = new();
    public bool isHitedByGoldenBulletThisTurn = false;

    public List<CoverableObj> currentCoverables;
    public float coverObjDmgMultiplier = 1;
    #endregion

    public int currentRound;

    private List<IDisplayableEffect> _displayableEffects;

    // ReSharper disable once InconsistentNaming
    private static readonly UnityEvent<Unit> onAnyUnitActionFinished = new UnityEvent<Unit>();
    [HideInInspector] public UnityEvent<Unit> onTurnStart; // me
    [HideInInspector] public UnityEvent<Unit> onTurnEnd; // me
    [HideInInspector] public UnityEvent<IUnitAction, Vector3Int> onActionStart; // action, target position
    [HideInInspector] public UnityEvent<IUnitAction> onFinishAction; //action
    [HideInInspector] public UnityEvent onBusyChanged;
    [HideInInspector] public UnityEvent<int, int> onCostChanged; // before, after
    [HideInInspector] public UnityEvent<int, int> onAmmoChanged; // before, after
    [HideInInspector] public UnityEvent<int, int> onHpChanged; // before, after
    [HideInInspector] public UnityEvent<Weapon> onWeaponChange; // after
    [HideInInspector] public UnityEvent<Unit> onMoved; // me
    [HideInInspector] public UnityEvent<Unit> onDead; //me
    [HideInInspector] public UnityEvent<Damage> onHit; // Damage Context
    [HideInInspector] public UnityEvent<IDamageable> onStartShoot; // target
    [HideInInspector] public UnityEvent<Damage> onFinishShoot; // Damage Context
    [HideInInspector] public UnityEvent<IUnitAction, Vector3Int> OnAimStart;
    [HideInInspector] public UnityEvent<IUnitAction> OnAimEnd;

    [HideInInspector] public UnityEvent<Unit> onKill; // target
    [HideInInspector] public UnityEvent onUnitActionDataChanged;
    [HideInInspector] public UnityEvent onSelectedChanged;
    [HideInInspector] public UnityEvent onStatusEffectChanged;

    private IUnitAction[] _unitActionArray; // All Unit Actions attached to this Unit
    protected IUnitAction activeUnitAction; // Currently active action
    private bool _isBusy;
    private bool _hasDead;

    public virtual void SetUp(int index, string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel,
        List<Passive> passiveList)
    {
        _displayableEffects = new List<IDisplayableEffect>();
        _index = index;
        unitName = newName;
        stat = unitStat;
        currentCoverables = new List<CoverableObj>();
        
        var model = Instantiate(unitModel, transform);
        _unitModel = model.GetComponent<UnitModel>();
        _unitModel.Setup(this);

        _unitActionArray = GetComponents<IUnitAction>();
        foreach (IUnitAction action in _unitActionArray)
        {
            action.SetUp(this);
        }

        _passiveList = passiveList;
        foreach (Passive passive in _passiveList)
        {
            if (passive is null)
            {
                Debug.LogError("passive is null");
                break;
            }

            passive.Setup();

            passiveIndexList.Add(passive.index);
        }

        EquipWeapon(newWeapon, true);
        if (this is Player)
        {
            PlayerEvents.OnWeaponChanged.AddListener(wpn => EquipWeapon(wpn));
        }

        onFinishAction.AddListener((action) => onAnyUnitActionFinished.Invoke(this));
        FieldSystem.onCombatFinish.AddListener(OnCombatFinish);
        FieldSystem.onCombatEnter.AddListener(OnCombatFinish);

        _seController = new UnitStatusEffectController(this);

        goldenBulletEffect.criticalChance = 100;
    }

    public virtual void StartTurn()
    {
        onTurnStart.Invoke(this);

        stat.Recover(StatType.CurActionPoint, stat.maxActionPoint, out int appliedValue);
        animator.SetBool(COVER, false);

        if (hp <= 0)
        {
            DeadCall(this);
            EndTurn();
        }
        else SelectAction(GetAction<IdleAction>());
    }

    public void EndTurn()
    {
        // find all coverables
        var curTile = FieldSystem.tileSystem.GetTile(hexPosition);
        if (curTile is not null)
        {
            foreach (var obj in curTile.tileObjects)
            {
                if (obj is CoverableObj coverableObj)
                {
                    coverableObj.SetUnit(this);
                }
            }
        }
        
        onTurnEnd.Invoke(this);

        // reset idle trigger animator
        animator.SetTrigger("Idle");

        FieldSystem.turnSystem.EndTurn();
        isHitedByGoldenBulletThisTurn = false;
    }

    private Unit _killer;

    protected virtual void DeadCall(Unit unit)
    {
        UIManager.instance.combatUI.turnOrderUI.DeleteDeadUnitTurnOrderUI(this);
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

            if (hasMoved) onMoved?.Invoke(this);
        }
    }

    public void MoveForcely(Vector3Int value)
    {
        hexTransform.position = value;
    }

    private void EquipWeapon(Weapon newWeapon, bool isOnSetup = false)
    {
        bool changingInCombat = (GameManager.instance.CompareState(GameState.COMBAT) && isOnSetup == false);
        if (changingInCombat && stat.curActionPoint < 4) return;

        newWeapon.unit = this;
        weapon = newWeapon;

        SetGoldBullet();

        if (weapon.model == null)
        {
            Debug.LogError("Weapon Model Is NULL");
        }
        else
        {
            _unitModel.SetupWeaponModel(newWeapon);
        }

        if (changingInCombat) ConsumeCost(4);
        onWeaponChange.Invoke(weapon);
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
        _unitActionArray = GetComponents<IUnitAction>();
        foreach (IUnitAction unitAction in _unitActionArray)
        {
            if (unitAction is T action)
            {
                return action;
            }
        }

        return default;
    }
    
    public IUnitAction GetAction(ActionType actionType)
    {
        return _unitActionArray.FirstOrDefault(unitAction => unitAction.GetActionType() == actionType);
    }

    public IUnitAction[] GetUnitActionArray()
    {
        return _unitActionArray;
    }

    public void AddDisplayableEffect(IDisplayableEffect effect)
    {
        _displayableEffects.Add(effect);
    }

    public void RemoveDisplayableEffect(IDisplayableEffect effect)
    {
        _displayableEffects.Remove(effect);
    }

    public IDisplayableEffect[] GetDisplayableEffects()
    {
        //search passive that has displayable effect
        var displayableEffects = new List<IDisplayableEffect>();
        foreach (var passive in _passiveList)
        {
            if (passive.TryGetDisplayableEffect(out var displayableEffect))
            {
                displayableEffects.AddRange(displayableEffect);
            }
        }

        //search status effect that has displayable effect
        var statusEffects = _seController.GetAllStatusEffectInfo();
        if (statusEffects is not null)
        {
            displayableEffects.AddRange(statusEffects);
        }

        //add displayable other effects
        displayableEffects.AddRange(_displayableEffects);

        return displayableEffects.ToArray();
    }

    public Passive[] GetAllPassiveList()
    {
        return _passiveList.ToArray();
    }

    protected bool IsMyTurn()
    {
        if (hp <= 0) return false;
        return FieldSystem.turnSystem.turnOwner == this;
    }

    protected void SetBusy()
    {
        bool hasChanged = _isBusy is false;
        _isBusy = true;

        if (hasChanged) onBusyChanged.Invoke();
    }

    protected void ClearBusy()
    {
        bool hasChanged = _isBusy;
        _isBusy = false;

        if (hasChanged) onBusyChanged.Invoke();
    }

    public bool IsBusy()
    {
        return _isBusy;
    }

    public bool HasDead()
    {
        return _hasDead;
    }

    public void SelectItem(IItem item)
    {
        if (item is null) return;
        if (!item.IsUsable()) return;
        if (GameManager.instance.CompareState(GameState.WORLD))
        {
            if (item.IsImmediate())
            {
                item.Use(this);
                IInventory.OnInventoryChanged?.Invoke();
            }

            return;
        }


        var itemUsingAction = GetAction<ItemUsingAction>();
        itemUsingAction.SetItem(item);
        SelectAction(itemUsingAction);
    }

    /// <summary>
    /// 이 함수는 TryExecute에 의해서만 호출되어야 합니다. 그러지 않을경우, Busy처리가 되지 않습니다.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected bool TryExecuteUnitAction(Vector3Int targetPosition)
    {
        if (IsExecutable(targetPosition) is false) return false;

        onActionStart.Invoke(activeUnitAction, targetPosition);
        activeUnitAction.Execute();
        if (this is Player) UIManager.instance.combatUI.combatPopupTextUI.ClearText();
        return true;
    }

    public bool IsExecutable(Vector3Int targetPosition)
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
        
        if (activeUnitAction.GetCost() > currentActionPoint)
        {
            Debug.Log("Cost is loss, Cost is " + activeUnitAction.GetCost());
            return false;
        }

        return true;
    }

    public IUnitAction GetSelectedAction()
    {
        if (activeUnitAction is null) return GetAction<IdleAction>();
        return activeUnitAction;
    }

    public void TryAttack(IDamageable target)
    {
        onStartShoot.Invoke(target);

        bool hit = weapon.GetFinalHitRate(target)> Random.value * 100;

        if (VFXHelper.TryGetGunFireFXInfo(weapon.GetWeaponType(), out var fxGunFireKey, out var fxGunFireTime))
        {
            var gunpointPos = _unitModel.GetGunpointPosition();
            VFXManager.instance.TryInstantiate(fxGunFireKey, fxGunFireTime, gunpointPos);
        }

        if (VFXHelper.TryGetTraceOfBulletFXKey(weapon.GetWeaponType(), out var fxBulletLine, out var traceTime))
        {
            var startPos = _unitModel.GetGunpointPosition();
            var destPos = Hex.Hex2World(target.GetHex()) + Vector3.up;
            if (!hit)
                destPos += new Vector3(Random.value * 2 - 1, UnityEngine.Random.value * 2 - 1,
                    UnityEngine.Random.value * 2 - 1);
            VFXManager.instance.TryLineRender(fxBulletLine, traceTime, startPos, destPos);
        }
        
        
        //===============build context================================
        Damage.Type type = hit ? Damage.Type.DEFAULT : Damage.Type.MISS;
        if (hit)
        {
            float criticalRate = weapon.GetFinalCriticalRate();
            bool isCriticalHit = criticalRate >= Random.value * 100;
            
            if (isCriticalHit)
            {
                type |= Damage.Type.CRITICAL;
            }
        }
        Damage context = 
            new (weapon.GetFinalDamage(), weapon.GetFinalCriticalDamage(), type, this, null, target);
        //================================================================

        if (hit)
        {
            bool existKey =
                VFXHelper.TryGetBloodingFXKey(weapon.GetWeaponType(), out string fxBloodingKey, out float bloodingTime);
            if (existKey)
            {
                Vector3 targetPos = Hex.Hex2World(target.GetHex()) + Vector3.up;
                VFXManager.instance.TryInstantiate(fxBloodingKey, bloodingTime, targetPos);
            }
        }
        else
        {
            UIManager.instance.onNonHited.Invoke(target);
        }
        
        target.TakeDamage(context);
        weapon.CurrentAmmo--;
        UIManager.instance.onPlayerStatChanged.Invoke();

        onFinishShoot.Invoke(context);
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
            //Debug.Log("Cost is loss, Cost is " + action.GetCost());
            return;
        }

        if (HasStatusEffect(StatusEffectType.Stun)) return;

#if UNITY_EDITOR
        //Debug.Log("Select Action : " + action);
#endif

        activeUnitAction = action;
        onSelectedChanged.Invoke();

        if (activeUnitAction.CanExecuteImmediately())
        {
            if (activeUnitAction is not IdleAction) SetBusy();
            var actionSuccess = TryExecuteUnitAction(Vector3Int.zero);
            Debug.Log("actionSuccess: " + actionSuccess);

            if (actionSuccess is false) ClearBusy();
        }
        else
        {
            UIManager.instance.combatUI.combatPopupTextUI.SetActionSeleteTextUI(action);
        }
    }

    public void FinishAction()
    {
        var action = activeUnitAction;

        if (activeUnitAction is not MoveAction) ConsumeCost(activeUnitAction.GetCost());

        ClearBusy();
        if (GameManager.instance.CompareState(GameState.COMBAT))
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

    private int _atkCount;

    ///<summary>
    /// 한 턴에 한번만 사격 가능합니다. "단 Infinite Action Point 스킬을 배우지 않았을 경우"
    /// </summary>
    public bool CheckAttackedTrigger()
    {
        return HasStatusEffect(StatusEffectType.Recoil) && 
               infiniteActionPointTrigger is false;
    }

    /// <summary>
    /// 사격 후 이동이 불가합니다. "단 Light Foot 스킬을 배우지 않았을 경우"
    /// </summary>
    /// <returns></returns>
    public bool CheckAttackMoveTrigger() => HasStatusEffect(StatusEffectType.Recoil) && lightFootTrigger is false;

    #region STATUE EFFECT

    private UnitStatusEffectController _seController;
    private static readonly int COVER = Animator.StringToHash("Cover");

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

    public bool TryGetStatusEffect(StatusEffectType type, out StatusEffect effect)
    {
        return _seController.TryGetStatusEffect(type, out effect);
    }

    #endregion

    #region UNITY_EVENT

    private void OnCombatFinish(bool playerWin)
    {
        // disable all status effect, and passive
        _seController.RemoveAllStatusEffect();

        foreach (var passive in _passiveList)
        {
            passive.Disable();
            passive.Delete();
        }

        _passiveList.Clear();
        stat.ResetModifier();
    }

    #endregion

    public void SetPassive(List<Passive> passiveList)
    {
        _passiveList = passiveList;
        foreach (var passive in _passiveList)
        {
            if (passive is null)
            {
                Debug.LogError("passive is null");
                break;
            }
            passive.Setup();

            //passiveIndexList.Add(passive.index);
        }
    }

    public void SetGoldBullet()
    {
        float chance = goldenBulletChance;
        goldenBulletCount = Mathf.FloorToInt(chance / 100.0f);
        chance -= goldenBulletCount * 100;
        System.Random rand = new System.Random();
        if (rand.Next(0, 100) < chance) goldenBulletCount++;

        if (weapon.GetWeaponType() == ItemType.Revolver && goldenBulletCount != 0)
        {
            weapon.magazine.ClearEffectAll();

            List<int> candidate = new();
            List<int> selectedNumber = new();
            for (int i = 0; i < weapon.maxAmmo; i++) candidate.Add(i);

            for (int i = 0; i < goldenBulletCount; i++)
            {
                int select = Random.Range(0, candidate.Count);
                selectedNumber.Add(candidate[select]);
                candidate.RemoveAt(select);
            }

            for (int i = 0; i < selectedNumber.Count; i++)
            {
                weapon.magazine.SetGold(selectedNumber[i], goldenBulletEffect);
            }
        }
    }


    #region IDAMAGEABLE

    public virtual void TakeDamage(Damage damageContext)
    {
        if (gameObject == null) return;

        stat.Consume(StatType.CurHp, damageContext.GetFinalAmount()); //for test
        UIManager.instance.onTakeDamaged.Invoke(this, damageContext.GetFinalAmount(), damageContext.type);
        onHit.Invoke(damageContext);

        if (hp <= 0 && _hasDead is false)
        {
            _hasDead = true;
            onAnyUnitActionFinished.AddListener(DeadCall);

            _killer = damageContext.attacker;
        }
    }

    public Vector3Int GetHex()
    {
        return hexTransform.position;
    }

    public int GetCurrentHp()
    {
        return stat.GetOriginalStat(StatType.CurHp);
    }

    public int GetMaxHp()
    {
        return stat.GetOriginalStat(StatType.MaxHp);
    }

    public Transform GetTsf => transform;

    public int GetHitRateModifier(Unit attacker = null)
    {
        int modifier = 0;

        if (attacker != null)
        {
            foreach(CoverableObj obj in currentCoverables)
            {
                bool coverable = CoverableObj.CanCover(
                    attacker.hexPosition,
                    obj.hexPosition,
                    obj.GetCoverDirections()
                );

                if (coverable)
                {
                    modifier += obj.GetCoverType() switch
                    {
                        CoverType.NONE => 0,
                        CoverType.LIGHT => -20,
                        CoverType.HEAVY => -30,
                        _ => 0
                    };

                    obj.SetFlag();

                    break;
                }
            }
        }
        
        return (int) modifier;
    }

    public UnityEvent<int, int> OnHpChanged => onHpChanged;

    #endregion

    public void AddCoverable(CoverableObj obj)
    {
        currentCoverables ??= new List<CoverableObj>();

        currentCoverables.Add(obj);
    }
    
    public void RemoveCoverable(CoverableObj obj)
    {
        currentCoverables?.Remove(obj);
    }
}


