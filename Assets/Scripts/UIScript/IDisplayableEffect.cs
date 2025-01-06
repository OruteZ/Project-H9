using System;
using System.Dynamic;
using UnityEngine.Events;

public interface IDisplayableEffect
{
    public const int NONE = int.MaxValue;

    public int GetIndex();
    public int GetStack();
    public int GetDuration();

    public bool CanDisplay();
    
    // public int GetIconIndex();
}