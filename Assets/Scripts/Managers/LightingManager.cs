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
    
    private StageStyle currentStageStyle;
    
    
    protected override void Awake()
    {
        base.Awake();
        if (this == null) return;
        if (instance != this)
        {
            Destroy(gameObject);
        }
        
        FieldSystem.onStageAwake.AddListener(OnStageAwake);
        
        westernLight.SetActive(false);
        westernPpVolume.GetComponent<Volume>().weight = 0;
        
        snowMountainLight.SetActive(false);
        snowMountainPpVolume.GetComponent<Volume>().weight = 0;
        
        // start point is plane , change after
        planeLight.SetActive(true);
        planePpVolume.GetComponent<Volume>().weight = 1;
        currentStageStyle = StageStyle.PLANE;
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
            currentStageStyle = StageStyle.PLANE;
            return;
        }

        SetStageStyle(stageData.GetStageStyle());
    }

    public void LerpChange(StageStyle from, StageStyle to, float value)
    {
        Mathf.Clamp01(value);
        
        var fromPPVolume = GetPPVolume(from);
        var toPPVolume = GetPPVolume(to);
        
        var fromLight = GetLight(from);
        var toLight = GetLight(to);
        
        if (fromPPVolume is null || toPPVolume is null) return;
        
        fromPPVolume.SetActive(true);
        toPPVolume.SetActive(true);
        
        fromPPVolume.GetComponent<Volume>().weight = 1 - value;
        toPPVolume.GetComponent<Volume>().weight = value;
        
        if (fromLight is null || toLight is null) return;
        
        if (value > 0.5f)
        {
            fromLight.SetActive(false);
            toLight.SetActive(true);
        }
        else
        {
            fromLight.SetActive(true);
            toLight.SetActive(false);
        }
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
    
    private GameObject GetLight(StageStyle stageStyle)
    {
        switch (stageStyle)
        {
            case StageStyle.WESTERN:
                return westernLight;
            case StageStyle.SNOW:
                return snowMountainLight;
            case StageStyle.PLANE:
                return planeLight;
            default:
                return null;
        }
    }

    public void SetStageStyle(StageStyle stageStyle)
    {
        LerpChange(currentStageStyle, stageStyle, 1);
        currentStageStyle = stageStyle;
    }
}