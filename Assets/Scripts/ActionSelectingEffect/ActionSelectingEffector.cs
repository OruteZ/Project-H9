using System.Collections.Generic;
using UnityEngine;

public class ActionSelectingEffector : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private ActionSelectingEffectSetting _setting;
    [SerializeField] private List<IActionSelectingEffect> _effects;
    
    public Canvas GetCanvas()
    {
        return _canvas;
    }
    
    public ActionSelectingEffectSetting GetSetting()
    {
        return _setting;
    }
    
    private void Awake()
    {
        foreach (IActionSelectingEffect effect in _effects)
        {
            effect.SetupTileEffect(_setting, this);
        }
    }
}