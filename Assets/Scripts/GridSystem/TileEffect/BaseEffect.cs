using UnityEngine;

public abstract class BaseSelectingActionEffect : ScriptableObject, IActionSelectingEffect
{
    protected TileEffectSetting Setting;
    
    public abstract void StopEffect();
    public abstract void ShowTileEffect(Unit user);
    public abstract ActionType GetActionType();

    public void SetupTileEffect(TileEffectSetting setting)
    {
        Setting = setting;
    }
}