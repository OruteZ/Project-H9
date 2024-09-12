using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "LightSetting", menuName = "Light/LightSetting")]
public class LightSetting : ScriptableObject
{
    public StageStyle stageStyle;

    [Header("Directional Light")]
    public Color globalColor = Color.white;
    public float globalIntensity = 1;
    public float globalIndirectMultiplier = 1;
    
    [Header("Reflection Light")]
    public Color reflectionColor= Color.white;
    public float reflectionIntensity = 1;
    public float reflectionIndirectMultiplier = 1;
    
    [Header("PP Volume")]
    public VolumeProfile ppVolume;
}