using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionState
{
    public abstract void OnSelect();
    public abstract void OffSelect();
}
