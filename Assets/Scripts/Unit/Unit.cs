using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
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
    public SkinnedMeshRenderer visual;
    public WeaponModel weaponModel;
    public Transform hand;
    public Transform back;
    public Transform waist;

    [Header("Status")] 
    [SerializeField] protected UnitStat stat;

    // ReSharper disable once InconsistentNaming
     public static readonly UnityEvent<Unit> onAnyUnitActionFinished = new UnityEvent<Unit>();
     
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
    private bool _hasDead;
    
    public string unitName;
    public int currentActionPoint;
    public Weapon weapon;

    private GameObject _damageEffect = null;

    public GameObject damageEffect => _damageEffect ? _damageEffect :
        (_damageEffect = Resources.Load("Prefab/Damage Floater") as GameObject);
    public abstract void StartTurn();

    public virtual void GetDamage(int damage)
    {
        if (gameObject == null) return;

        animator.SetTrigger(GET_HIT1);
        CameraController.ShakeCamera();
        
        stat.curHp -= damage;
        onHit.Invoke(this, damage);
        
        //todo : Object Pool
        var dmgEffect = Instantiate(damageEffect).GetComponent<DamageFloat>();
        
        dmgEffect.SetPosition(transform.position, 2);
        dmgEffect.SetValue(damage);

        if (stat.curHp <= 0 && _hasDead is false)
        {
            _hasDead = true;
            animator.SetBool(DIE, true);
            onAnyUnitActionFinished.AddListener(DeadCall);
        }
    }

    public void DeadCall(Unit unit)
    {
        onDead.Invoke(this);
        onAnyUnitActionFinished.RemoveListener(DeadCall);
        Invoke(nameof(DestroyThis), 2f);
    }

    public bool hasAttacked;

    public int currentRound;

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
    public virtual void SetUp(string newName, UnitStat unitStat, Weapon newWeapon, GameObject unitModel)
    {
        unitName = newName;
        stat = unitStat;
        stat.curHp = stat.curHp;
        
        foreach (IUnitAction action in _unitActionArray)
        {
            action.SetUp(this);
        }


        var model = Instantiate(unitModel, transform);
        visual = model.GetComponentInChildren<SkinnedMeshRenderer>();
        hand = model.GetComponent<UnitModel>().hand;
        if (hand == null)
        {
            Debug.LogError("Hand is NULL");
        }

        waist = model.GetComponent<UnitModel>().waist;
        if (waist == null)
        {
            Debug.LogError("Waist (Hip) is NULL");
        }
        
        EquipWeapon(newWeapon);
        // FieldSystem.onCombatAwake.AddListener(() => {animator.SetTrigger(START);});
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
        _unitActionArray = GetComponents<IUnitAction>();
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

    public UnitStat GetStat()
    {
        return stat;
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
        if (activeUnitAction is null) return GetAction<IdleAction>();
        return activeUnitAction;
    }

    public bool TryAttack(Unit target)
    {
        bool hit = weapon.GetFinalHitRate(target) > UnityEngine.Random.value;

#if UNITY_EDITOR
        Debug.Log(hit ? "뱅" : "빗나감");
#endif
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
            weapon.Attack(target, out var isHeadShot);
            var existKey = VFXHelper.TryGetBloodingFXKey(weapon.GetWeaponType(), out var fxBloodingKey, out var bloodingTime);
            if (existKey)
            {
                var targetPos = target.transform.position + Vector3.up;
                VFXManager.instance.TryInstantiate(fxBloodingKey, bloodingTime, targetPos);
            }
        }
        weapon.currentAmmo--;

        return hit;
    }

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
            (AnimatorController)Resources.Load("Animator/" + (type is WeaponType.Null ? "Standing" : type) + " Animator Controller");
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    public void ConsumeCost(int value)
    {
        if (currentActionPoint < value)
        {
            Debug.LogError("Consume More Cost");
            return;
        }

        if (value == 0) return;

        currentActionPoint -= value;
        onCostChanged.Invoke(currentActionPoint);
    }
}

