using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipCostUI : UIElement
{
    public void SetTooltipCostUI(int apCost, int ammoCost, bool forcedEnoughTrigger)
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        int curAp = player.currentActionPoint;
        int curAmmo = player.weapon.currentAmmo;

        bool apCheck = apCost <= curAp;
        bool ammoCheck = ammoCost <= curAmmo;
        if (forcedEnoughTrigger) 
        {
            apCheck = ammoCheck = forcedEnoughTrigger;
        }
        transform.GetChild(0).GetComponent<TooltipCostUIElement>().SetTooltipCostUIElement("Ap Cost", apCost, apCheck);
        transform.GetChild(1).GetComponent<TooltipCostUIElement>().SetTooltipCostUIElement("Ammo Cost", ammoCost, ammoCheck);
        transform.GetChild(2).GetComponent<TooltipCostUIElement>().SetTooltipCostUIElement("Etc Cost", 0, false);
        OpenUI();
    }
}
