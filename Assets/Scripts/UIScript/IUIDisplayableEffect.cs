using System.Dynamic;
using UnityEngine.Events;

public interface IUIDisplayableEffect
{
    public string GetEffectName();
    public int GetStack();
    public int GetDuration();
}

public struct StatusEffectInfo
{
    public StatusEffectType statusEffectType;
    public int stack;
    public int duration;
}