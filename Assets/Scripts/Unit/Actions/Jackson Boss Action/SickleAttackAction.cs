using System.Collections;
using UnityEngine;

public class SickleAttackAction : BaseAction, IShootingAction
{
    private int _atkCount;
    public override ActionType GetActionType() => ActionType.SickleAttack;

    private Weapon Weapon => unit.weapon;
    private IDamageable _target;
    // private State _state;
    // private float _stateTimer;

    [SerializeField] private int damageMultiplier = 2;

    public override bool CanExecute()
    {
        if (_target == null)
        {
            Debug.Log("Target is null, cant attack");
            return false;
        }

        if (IsThereWallBetweenUnitAnd(_target.GetHex()))
        {
            Debug.Log("There is wall. cant attack");
            return false;
        }
        
        //vision check
        if (FieldSystem.tileSystem.VisionCheck(unit.GetHex(), _target.GetHex(), true) is false)
        {
            Debug.Log("Target is not in vision, cant attack");
            return false;
        }
        
        // if distance is greater than range, return false
        if (Hex.Distance(unit.hexPosition, _target.GetHex()) > range)
        {
            Debug.Log("Distance is greater than range, cant attack");
            return false;
        }

        if (_target is Barrel)
        {
            Debug.Log("Target is Barrel");
            return false;
        }

        return true;
    }
    public override bool CanExecute(Vector3Int targetPos)
    {
        if (FieldSystem.GetDamageable(targetPos) is null)
        {
            Debug.Log("There is no target, cant attack");
            return false;
        }
        
        if (IsThereWallBetweenUnitAnd(targetPos))
        {
            Debug.Log("There is wall. cant attack");
            return false;
        }
        
        //vision check
        if (FieldSystem.tileSystem.VisionCheck(unit.hexPosition, targetPos, true) is false)
        {
            Debug.Log("Target is not in vision, cant attack");
            return false;
        }

        // if distance is greater than range, return false
        if (Hex.Distance(unit.hexPosition, targetPos) > range)
        {
            Debug.Log("Distance is greater than range, cant attack");
            return false;
        }

        return true;
    }

    public override bool IsSelectable()
    {
        // if (unit.CheckAttackedTrigger()) return false;
        
        // Unarmed 디버프일때는 못 휘두르나?
        if (unit.HasStatusEffect(StatusEffectType.UnArmed)) return false;

        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    public override int GetCost()
    {
        return 1;
    }

    public override int GetAmmoCost()
    {
        return 0;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _target = FieldSystem.GetDamageable(targetPos);
        Debug.Log("Attack Target : " + _target);
    }
    public IDamageable GetTarget() 
    {
        return _target;
    }

    private bool IsThereWallBetweenUnitAnd(Vector3Int targetPos)
    {
        return !FieldSystem.tileSystem.RayThroughCheck(unit.hexPosition, targetPos);
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        unit.animator.SetTrigger(IDLE);
        
        float timer = 1f;
        while ((timer -= Time.deltaTime) > 0)
        {
            Transform tsf;
            Vector3 aimDirection = (Hex.Hex2World(_target.GetHex()) - (tsf = transform).position).normalized;
        
            float rotationSpeed = 10f;
            transform.forward = Vector3.Slerp(tsf.forward, aimDirection, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        
        unit.animator.ResetTrigger(IDLE);
        unit.animator.SetTrigger(MELEE);

        //총을 드는 시간 (애니메이션에 따라 다른 상수 값 , 무조건 프레임 단위로) 
        int cnt = 20;
        for(int i = 0; i < cnt; i++) yield return null;
        
        MeleeAtk(target: (Unit)_target);

        //Animation이 끝나고 IdleAction으로 돌아올 때 까지 대기 
        cnt = 69 - cnt;
        while (cnt-- > 0) yield return null;
        
        unit.animator.SetTrigger(IDLE);
        
        _atkCount++;
        if(unit.maximumShootCountInTurn <= _atkCount)
        {
            unit.TryAddStatus(new Recoil(unit));
        }
    }

    public override void SetUp(Unit unit)
    {
        base.SetUp(unit);
        
        unit.onTurnStart.AddListener(OnTurnStart);
    }

    private void OnTurnStart(Unit arg0)
    {
        _atkCount = 0;
    }

    private void MeleeAtk(Unit target)
    {
        unit.onStartShoot.Invoke(target);
        
        //===============build context================================
        Damage.Type type = Damage.Type.DEFAULT;
        
        Damage context = 
            new ((int)(Weapon.GetFinalDamage() * damageMultiplier), 
                (int)(Weapon.GetFinalDamage()* damageMultiplier), 
                type, unit, null, target);
        //================================================================

        bool existKey =
            VFXHelper.TryGetBloodingFXKey(Weapon.GetWeaponType(), out string fxBloodingKey, out float bloodingTime);
        if (existKey)
        {
            Vector3 targetPos = Hex.Hex2World(target.GetHex()) + Vector3.up;
            VFXManager.instance.TryInstantiate(fxBloodingKey, bloodingTime, targetPos);
        }
        
        target.TakeDamage(context);
        UIManager.instance.onPlayerStatChanged.Invoke();

        
        target.TryAddStatus(new Bleeding(1, unit));

        unit.onFinishShoot.Invoke(context);
    }

    public int GetRange()
    {
        return range;
    }
}
