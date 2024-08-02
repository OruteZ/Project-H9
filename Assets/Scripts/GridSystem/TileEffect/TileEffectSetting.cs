using UnityEngine;

/// <summary>
/// This Scriptable Object have information about setting TileEffect's variables, colors, ETC
/// </summary>
public class TileEffectSetting : ScriptableObject
{
    [Header("Default")]
    public GameObject tileEffectDefault;
    
    [Space(10)]
    [Header("Move Effect Inspector")]
    public float routeEffectRemoveDelay;
    public Color movableTileColor;
    public Color routeColor;
}