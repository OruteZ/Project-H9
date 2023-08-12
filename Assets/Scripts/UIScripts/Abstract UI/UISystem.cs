using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISystem : Generic.Singleton<UISystem>
{
    /// <summary>
    /// UI를 열었을 때 작동합니다.
    /// </summary>
    public virtual void OpenUI() 
    {
    }
    /// <summary>
    /// UI를 닫았을 때 작동합니다.
    /// </summary>
    public virtual void CloseUI()
    {
    }

    /// <summary>
    /// 현재 시스템에서 UI 팝업창을 열었을 때 작동합니다.
    /// </summary>
    public virtual void OpenPopupWindow()
    {
        //UIManager.instance.previousLayer = CurrentLayerNumber;
        //OpenWindow();
    }
    /// <summary>
    /// 현재 시스템에서 UI 팝업창을 닫았을 때 작동합니다.
    /// </summary>
    public virtual void ClosePopupWindow()
    {
        //UIManager.instance.previousLayer = LowLayerNumber;
        //CloseWindow();
    }
}
