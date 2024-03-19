using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UnitCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    private Unit _unit;

    public void SetTarget(Unit target)
    {
        _unit = target;
        _virtualCamera.Follow = target.transform;
        
        target.onDead.AddListener((u) =>
        {
            //destroy gameObject
            Destroy(gameObject);    
        });
        
        target.onFinishShoot.AddListener((a,b,c,d) =>
        {
            ShakeCamera(5, 1, 0.1f);
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
}