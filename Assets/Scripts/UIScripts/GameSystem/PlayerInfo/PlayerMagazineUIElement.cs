using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagazineUIElement : UIElement
{
    public void SetPlayerMagUIElement(int existCnt, int filledCnt) 
    {
        bool isExist = true;
        bool isFilled = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i >= filledCnt) isFilled = false;
            if (i >= existCnt) isExist = false;
            transform.GetChild(i).GetComponent<PlayerBulletUIElement>().SetBulletUIElement(isExist, isFilled);
        }
    }
}
