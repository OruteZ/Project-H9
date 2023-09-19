using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatActionTooltip : UIElement
{
    void Start()
    {
        CloseUI();
    }

    public void SetCombatActionTooltip(IUnitAction action, IUnitAction sAction, Vector3 pos)
    {
        OpenUI();

        string actionName = action.GetActionType().ToString();
        //string actionDescription = action.GetActionDescription().ToString();
        string actionDescription = "Description";
        if (actionName == "Idle")
        {
            actionName = "Cancel " + sAction.GetActionType().ToString();
            //actionDescription = _activeAction.GetActionDescription().ToString();
        }
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = actionName;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = actionDescription;

        GetComponent<RectTransform>().position = pos;
    }
}
