using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActionSkillCostUI : UIElement
{
    public void SetTooltipCostUI(int apCost, int ammoCost)
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        int curAp = player.currentActionPoint;
        int curAmmo = player.weapon.currentAmmo;
        transform.GetChild(0).GetComponent<CombatActionSkillCostUIElement>().SetTooltipCostUIElement(apCost, apCost <= curAp);
        transform.GetChild(1).GetComponent<CombatActionSkillCostUIElement>().SetTooltipCostUIElement(ammoCost, ammoCost <= curAmmo);
        transform.GetChild(2).GetComponent<CombatActionSkillCostUIElement>().SetTooltipCostUIElement(0, false);
        transform.GetChild(3).GetComponent<CombatActionSkillCostUIElement>().SetTooltipCostUIElement(0, false);
    }
}
