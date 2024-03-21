using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortraitUI : UIElement
{

    public void SetPortraitUI() 
    { 
    }
    public void ClickPortrait()
    {
        CameraManager.instance.worldCamera.SetPosition(FieldSystem.unitSystem.GetPlayer().transform.position);
    }
}
