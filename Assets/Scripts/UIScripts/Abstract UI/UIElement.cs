using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    /// <summary>
    /// UI를 열었을 때 작동합니다.
    /// </summary>
    public virtual void OpenUI()
    {
        enabled = true;
    }
    /// <summary>
    /// UI를 닫았을 때 작동합니다.
    /// </summary>
    public virtual void CloseUI()
    {
        enabled = false;
    }
}
