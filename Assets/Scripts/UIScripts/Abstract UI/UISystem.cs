using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISystem : Generic.Singleton<UISystem>
{
    public virtual void OpenUI() 
    {
        enabled = true;
    }
    public virtual void CloseUI()
    {
        enabled = false;
    }

    public virtual void OpenPopupWindow()
    {
        //UIManager.instance.previousLayer = CurrentLayerNumber;
        //OpenWindow();
    }
    public virtual void ClosePopupWindow()
    {
        //UIManager.instance.previousLayer = LowLayerNumber;
        //CloseWindow();
    }
}
