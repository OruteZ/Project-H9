using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    protected bool isOpenUI;
    /// <summary>
    /// UI를 열었을 때 작동합니다.
    /// </summary>
    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
        isOpenUI = true;
    }
    /// <summary>
    /// UI를 닫았을 때 작동합니다.
    /// </summary>
    public virtual void CloseUI()
    {
        isOpenUI = false;
        gameObject.SetActive(false);
    }

    public virtual bool IsInteractable() 
    {
        return true;
    }
}
