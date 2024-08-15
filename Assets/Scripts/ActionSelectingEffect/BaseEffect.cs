using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public abstract class BaseSelectingActionEffect : ScriptableObject, IActionSelectingEffect
{
    protected ActionSelectingEffectSetting setting;
    protected ActionSelectingEffector effector;

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

    public void SetupTileEffect(ActionSelectingEffectSetting setting, ActionSelectingEffector effector)
    {
        this.setting = setting;
        this.effector = effector;
        OnSetup();
    }

    protected virtual void OnSetup()
    {
        
    }
}