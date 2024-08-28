using Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class LightingManager : Singleton<LightingManager>
{
    [Header("Western")]
    [SerializeField] private GameObject westernLight;
    [SerializeField] private GameObject westernPpVolume;
    
    [Space(10),Header("SNOW MOUNTAIN")]
    [SerializeField] private GameObject snowMountainLight;
    [SerializeField] private GameObject snowMountainPpVolume;

    [Space(10), Header("Plane")] 
    [SerializeField] private GameObject planeLight;
    [SerializeField] private GameObject planePpVolume;
    
    
    protected override void Awake()
    {
        base.Awake();
        if (this == null) return;
        if (instance != this) return;
        
        FieldSystem.onStageAwake.AddListener(OnStageAwake);
        
        westernLight.SetActive(false);
        westernPpVolume.GetComponent<Volume>().weight = 0;
        
        snowMountainLight.SetActive(false);
        snowMountainPpVolume.GetComponent<Volume>().weight = 0;
        
        // start point is plane , change after
        planeLight.SetActive(true);
        planePpVolume.GetComponent<Volume>().weight = 1;
    }

    private void OnStageAwake()
    {
        // set active false all
        westernLight.SetActive(false);
        snowMountainLight.SetActive(false);
        
        westernPpVolume.GetComponent<Volume>().weight = 0;
        snowMountainPpVolume.GetComponent<Volume>().weight = 0;
        planePpVolume.GetComponent<Volume>().weight = 0;
        
        // if data is null, set western
        var stageData = GameManager.instance.GetStageData();
        if (stageData is null)
        {
            // default if plane
            planeLight.SetActive(true);
            planePpVolume.GetComponent<Volume>().weight = 1;
            return;
        }
        
        StageStyle stageType = stageData.GetStageStyle();
        
        
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
            case StageStyle.PLANE:
                planeLight.SetActive(true);
                planePpVolume.GetComponent<Volume>().weight = 1;
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
            case StageStyle.PLANE:
                return planePpVolume;
            default:
                return null;
        }
    }
}