using Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class LightingManager : Singleton<LightingManager>
{
    [Header("Western")]
    [SerializeField] private GameObject westernLight;
    [SerializeField] private GameObject westernPpVolume;
    
    [Header("SNOW MOUNTAIN"), Space(10)]
    [SerializeField] private GameObject snowMountainLight;
    [SerializeField] private GameObject snowMountainPpVolume;
    
    protected override void Awake()
    {
        base.Awake();
        if (this == null) return;
        if (instance != this) return;
        
        FieldSystem.onStageAwake.AddListener(OnStageAwake);
        
        westernLight.SetActive(true);
        westernPpVolume.SetActive(true);
        snowMountainLight.SetActive(false);
        snowMountainPpVolume.SetActive(false);
    }

    private void OnStageAwake()
    {
        // set active false all
        westernLight.SetActive(false);
        snowMountainLight.SetActive(false);
        
        westernPpVolume.GetComponent<Volume>().weight = 0;
        snowMountainPpVolume.GetComponent<Volume>().weight = 0;
        
        // if data is null, set western
        var stageData = GameManager.instance.GetStageData();
        if (stageData is null)
        {
            westernPpVolume.GetComponent<Volume>().weight = 1;
            snowMountainPpVolume.GetComponent<Volume>().weight = 0;
            return;
        }
        
        var stageType = stageData.GetStageStyle();
        
        
        switch (stageType)
        {
            case StageStyle.WESTERN:
                westernLight.SetActive(true);
                westernPpVolume.GetComponent<Volume>().weight = 1;
                break;
            case StageStyle.SNOW:
                snowMountainLight.SetActive(true);
                snowMountainPpVolume.GetComponent<Volume>().weight = 1;
                break;
        }
    }

    public void LerpChange(StageStyle from, StageStyle to, float value)
    {
        Mathf.Clamp01(value);
        
        var fromPPVolume = GetPPVolume(from);
        var toPPVolume = GetPPVolume(to);
        
        if (fromPPVolume is null || toPPVolume is null) return;
        
        fromPPVolume.SetActive(true);
        toPPVolume.SetActive(true);
        
        fromPPVolume.GetComponent<Volume>().weight = 1 - value;
        toPPVolume.GetComponent<Volume>().weight = value;
    }
    
    private GameObject GetPPVolume(StageStyle stageStyle)
    {
        switch (stageStyle)
        {
            case StageStyle.WESTERN:
                return westernPpVolume;
            case StageStyle.SNOW:
                return snowMountainPpVolume;
            default:
                return null;
        }
    }
}