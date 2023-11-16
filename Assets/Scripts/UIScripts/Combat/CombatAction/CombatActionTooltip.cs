using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatActionTooltip : UIElement
{
    private const int COMBAT_ACTION_TOOLTIP_Y_POSITION_CORRECTION = 265;

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

        pos.y = COMBAT_ACTION_TOOLTIP_Y_POSITION_CORRECTION * Camera.main.pixelHeight / 1080.0f;
        GetComponent<RectTransform>().position = pos;
    }
}
