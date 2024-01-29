using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerApUI : UIElement
{
    public void SetApUI()
    {
        int maxAp = GameManager.instance.playerStat.maxActionPoint;
        int curAp = FieldSystem.unitSystem.GetPlayer().currentActionPoint;
        bool isExist = true;
        bool isFilled = true;
        for (int i = 0; i < transform.childCount; i++) 
        {
            if (i >= curAp) isFilled = false;
            if (i >= maxAp) isExist = false;

            transform.GetChild(i).GetComponent<PlayerApUIElement>().SetApUIElement(isExist, isFilled);
        }
    }
}
