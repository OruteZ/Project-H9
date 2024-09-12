using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UnitCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CinemachineTargetGroup _targetGroup;

    [SerializeField] private float unitRadius = 2;

    private Unit _unit;
    private Tile _actionTargetTile;

    private void Awake()
    {
        _targetGroup = new GameObject("ViewPoint").AddComponent<CinemachineTargetGroup>();
        
        _virtualCamera.Follow = _targetGroup.transform;
        _virtualCamera.LookAt = _targetGroup.transform;
    }

    public void SetOwner(Unit owner)
    {
        _unit = owner;
        CatchTarget(owner.transform);
        // _virtualCamera.Follow = owner.transform;
        
        owner.onDead.AddListener((u) =>
        {
            //destroy gameObject
            Destroy(gameObject);    
        });
        
        FieldSystem.onCombatFinish.AddListener((a) =>
        {
            Destroy(gameObject);
        });
        
        owner.onFinishShoot.AddListener((context) =>
        {
            ShakeCamera(5, 1, 0.1f);
        });
        
        owner.onActionStart.AddListener((a, t) =>
        {
            if (t == Hex.none) t = a.GetUnit().hexPosition;
            
            _actionTargetTile = FieldSystem.tileSystem.GetTile(t);
            if (_actionTargetTile is null) return;
            
            CatchTarget(_actionTargetTile.transform);
        });
        
        owner.onFinishAction.AddListener((a) =>
        {
            if (_actionTargetTile is null) return;
            RemoveTarget(_actionTargetTile.transform);
            
            _actionTargetTile = null;
        });
    }

    public void SetPriority(int i)
    {
        _virtualCamera.Priority = i;
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        var perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() == null)
        {
            perlin = _virtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;

        StartCoroutine(StopShake(duration));
    }
    
    #region Coroutine
    private IEnumerator StopShake(float duration)
    {
        yield return new WaitForSeconds(duration);

        var perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }
    #endregion

    public Unit GetUnit()
    {
        return _unit;
    }

    private void CatchTarget(Transform target)
    {
        _targetGroup.AddMember(target, 1, unitRadius);
    }

    private void RemoveTarget(Transform target)
    {
        _targetGroup.RemoveMember(target);
    }

    private void OnDestroy()
    {
        Destroy(_targetGroup.gameObject);
    }
}