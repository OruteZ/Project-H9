using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISystem : MonoBehaviour
{
    protected List<UISystem> uiSubsystems = new List<UISystem>();

    /// <summary>
    /// UI를 열었을 때 작동합니다.
    /// </summary>
    public virtual void OpenUI() 
    {
        foreach (UISystem sys in uiSubsystems) 
        {
            sys.OpenUI();
        }
    }
    /// <summary>
    /// UI를 닫았을 때 작동합니다.
    /// </summary>
    public virtual void CloseUI()
    {
        foreach (UISystem sys in uiSubsystems)
        {
            sys.CloseUI();
        }
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
