using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    public virtual void OpenUI()
    {
        enabled = true;
    }
    public virtual void CloseUI()
    {
        enabled = false;
    }
}
