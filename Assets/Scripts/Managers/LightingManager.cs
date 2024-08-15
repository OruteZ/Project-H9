using Generic;
using UnityEngine;
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
        westernPpVolume.SetActive(false);
        snowMountainLight.SetActive(false);
        snowMountainPpVolume.SetActive(false);
        
        // if data is null, set western
        var stageData = GameManager.instance.GetStageData();
        if (stageData is null)
        {
            westernLight.SetActive(true);
            westernPpVolume.SetActive(true);
            return;
        }
        
        var stageType = stageData.GetStageStyle();
        
        
        switch (stageType)
        {
            case StageStyle.WESTERN:
                westernLight.SetActive(true);
                westernPpVolume.SetActive(true);
                break;
            case StageStyle.SNOW:
                snowMountainLight.SetActive(true);
                snowMountainPpVolume.SetActive(true);
                break;
        }
    }
}