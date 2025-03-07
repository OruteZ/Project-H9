using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SickleGrabAction : BaseAction
{
    private Unit _target;
    
    public override ActionType GetActionType()
    {
        return ActionType.SickleGrab;
    }

    public override void SetTarget(Vector3Int targetPos)
    {
        _target = FieldSystem.unitSystem.GetUnit(targetPos);
    }

    public override bool CanExecute()
    {
        // 1. target이 null이면 false
        if (_target == null)
        {
            Debug.Log("Target is null, cant attack");
            return false;
        }
        
        // 2. target까지 visioncheck, raycastcheck
        if (FieldSystem.tileSystem.VisionCheck(unit.GetHex(), _target.GetHex(), true) is false)
        {
            Debug.Log("Target is not in vision, cant attack");
            return false;
        }
        
        if (IsThereWallBetweenUnitAnd(_target.GetHex()))
        {
            Debug.Log("There is wall. cant attack");
            return false;
        }
        
        // 3. target 방향 앞에 빈칸인지 확인
        List<Vector3Int> line = Hex.DrawLine1(unit.hexPosition, _target.hexPosition);
        if (FieldSystem.unitSystem.GetUnit(line[1]) != null)
        {
            Debug.Log("There is unit in front of target");
            return false;
        }
        
        return true;
    }

    private bool IsThereWallBetweenUnitAnd(Vector3Int targetPos)
    {
        return !FieldSystem.tileSystem.RayThroughCheck(unit.hexPosition, targetPos);
    }

    public override bool CanExecute(Vector3Int targetPos)
    {
        Unit targetUnit = FieldSystem.unitSystem.GetUnit(targetPos);
        
        // 1. target이 null이면 false
        if (targetUnit == null)
        {
            Debug.Log("Target is null, cant attack");
            return false;
        }
        
        // 4. target까지의 거리가 range 이하인치 체크
        if (Hex.Distance(unit.hexPosition, targetPos) > range)
        {
            Debug.Log("Target is too far, cant attack");
            return false;
        }
        
        // 2. target까지 visioncheck, raycastcheck
        if (FieldSystem.tileSystem.VisionCheck(unit.GetHex(), targetPos, true) is false)
        {
            Debug.Log("Target is not in vision, cant attack");
            return false;
        }
        
        if (IsThereWallBetweenUnitAnd(targetPos))
        {
            Debug.Log("There is wall. cant attack");
            return false;
        }
        
        // 3. target 방향 앞에 빈칸인지 확인
        List<Vector3Int> line = Hex.DrawLine1(unit.hexPosition, targetPos);
        if (FieldSystem.unitSystem.GetUnit(line[1]) != null)
        {
            Debug.Log("There is unit in front of target");
            return false;
        }
        
        
        
        return true;
    }

    public override bool IsSelectable()
    {
        return true;
    }

    public override bool CanExecuteImmediately()
    {
        return false;
    }

    private bool _finishGrabTrigger = false;
    protected override IEnumerator ExecuteCoroutine()
    {
        _finishGrabTrigger = false;
        
        // 1. Target 쳐다보기
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
        unit.animator.SetTrigger(DYNAMITE);

        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => _finishGrabTrigger);
        
        List<Vector3Int> line = Hex.DrawLine1(unit.hexPosition, _target.hexPosition);
        _target.hexPosition = line[1];
        
        unit.animator.SetTrigger(IDLE);
    }
    
    #region THROW

    [SerializeField]
    private GameObject grabSicklePrefab;

    private void Throw()
    {
        Vector3 spawnPosition = unit.hand.position;
        
        // todo : grab effect spawn
        SickleGrabEffect effect = Instantiate(grabSicklePrefab, spawnPosition, Quaternion.identity)
            .GetComponent<SickleGrabEffect>();
        // check null
        if (effect == null)
        {
            Debug.LogError("Effect is null");
            return;
        }
        
        effect.ExtendRetractChain(_target.transform.position, () =>
        {
            _finishGrabTrigger = true;
            Destroy(effect.gameObject);
        });
    }

    public override void TossAnimationEvent(string args)
    {
        if (args != AnimationEventNames.THROW) return;
        {
            Throw();
        }
    }
    #endregion
}