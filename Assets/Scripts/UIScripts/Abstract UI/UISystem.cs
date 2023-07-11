using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISystem : Generic.Singleton<UISystem>
{
    public abstract void OpenUI();
    public abstract void CloseUI();

    public virtual void ClosePopupWindow() 
    {

    }
}
