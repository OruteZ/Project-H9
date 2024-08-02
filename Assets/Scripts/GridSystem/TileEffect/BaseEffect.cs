using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class BaseSelectingActionEffect : ScriptableObject, IActionSelectingEffect
{
    protected TileEffectSetting Setting;
    protected TileEffectManager Effecter;

    [SerializeField] private List<ActionType> targetTypes;
    
    public abstract void StopEffect();
    public abstract void ShowEffect(Unit user);
    public List<ActionType> GetActionType()
    {
        return targetTypes;
    }

    public bool HasActionType(ActionType type)
    {
        return targetTypes.Contains(type);
    }

    public void SetupTileEffect(TileEffectSetting setting, TileEffectManager effecter)
    {
        Setting = setting;
        Effecter = effecter;
        OnSetup();
    }

    protected virtual void OnSetup()
    {
        
    }
}