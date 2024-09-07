using Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class LightingManager : Singleton<LightingManager>
{
    [SerializeField] private Light sunLight;
    [SerializeField] private Light reflectionLight;
    [SerializeField] private Volume PpVolume;
    
    [SerializeField] private LightSetting[] _lightSettings;
    
    private StageStyle _currentStageStyle;
    
    
    protected override void Awake()
    {
        base.Awake();
        if (this == null) return;
        if (instance != this)
        {
            Destroy(gameObject);
        }
        
        FieldSystem.onStageAwake.AddListener(OnStageAwake);
    }

    private void OnStageAwake()
    {
        ApplyLightSetting(_lightSettings[0]);
        _currentStageStyle = _lightSettings[0].stageStyle;
    }

    private void LerpChange(StageStyle from, StageStyle to, float value)
    {
        if (TryGetLight(from, out LightSetting fromLight) && TryGetLight(to, out LightSetting toLight))
        {
            sunLight.color = Color.Lerp(fromLight.globalColor, toLight.globalColor, value);
            sunLight.intensity = Mathf.Lerp(fromLight.globalIntensity, toLight.globalIntensity, value);
            sunLight.bounceIntensity = Mathf.Lerp(fromLight.globalIndirectMultiplier, toLight.globalIndirectMultiplier, value);
            
            reflectionLight.color = Color.Lerp(fromLight.reflectionColor, toLight.reflectionColor, value);
            reflectionLight.intensity = Mathf.Lerp(fromLight.reflectionIntensity, toLight.reflectionIntensity, value);
            reflectionLight.bounceIntensity = Mathf.Lerp(fromLight.reflectionIndirectMultiplier, toLight.reflectionIndirectMultiplier, value);
            
            PpVolume.profile = fromLight.ppVolume;
        }
    }

    public void SetStageStyle(StageStyle stageStyle)
    {
        LerpChange(_currentStageStyle, stageStyle, 1);
        
        _currentStageStyle = stageStyle;
    }
    
    private bool TryGetLight(StageStyle stageStyle, out LightSetting ret)
    {
        foreach (LightSetting lightSetting in _lightSettings)
        {
            if (lightSetting.stageStyle == stageStyle)
            {
                ret = lightSetting;
                return true;
            }
        }

        ret = null;
        return false;
    }
    
    private void ApplyLightSetting(LightSetting lightSetting)
    {
        sunLight.color = lightSetting.globalColor;
        sunLight.intensity = lightSetting.globalIntensity;
        sunLight.bounceIntensity = lightSetting.globalIndirectMultiplier;
        
        reflectionLight.color = lightSetting.reflectionColor;
        reflectionLight.intensity = lightSetting.reflectionIntensity;
        reflectionLight.bounceIntensity = lightSetting.reflectionIndirectMultiplier;
        
        PpVolume.profile = lightSetting.ppVolume;
    }
}