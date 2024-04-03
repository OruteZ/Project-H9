using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipCostUI : UIElement
{
    public void SetTooltipCostUI(int apCost, int ammoCost)
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        int curAp = player.currentActionPoint;
        int curAmmo = player.weapon.currentAmmo;
        transform.GetChild(0).GetComponent<TooltipCostUIElement>().SetTooltipCostUIElement("Ap Cost", apCost, apCost <= curAp);
        transform.GetChild(1).GetComponent<TooltipCostUIElement>().SetTooltipCostUIElement("Ammo Cost", ammoCost, ammoCost <= curAmmo);
        transform.GetChild(2).GetComponent<TooltipCostUIElement>().SetTooltipCostUIElement("Etc Cost", 0, false);
        OpenUI();
    }
}
