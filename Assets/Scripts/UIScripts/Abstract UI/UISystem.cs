using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISystem : Generic.Singleton<UISystem>
{
    public virtual void OpenUI() { }
    public virtual void CloseUI() { }

    public virtual void ClosePopupWindow() { }
}
