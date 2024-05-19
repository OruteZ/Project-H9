using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMainStatTooltip : UIElement
{
    [SerializeField] private GameObject _tooltipText;

    private const int TOOLTIP_X_POSITION_CORRECTION = 40;
    private const int TOOLTIP_Y_POSITION_CORRECTION = 40;
    public void SetPlayerMainStatTooltip(GameObject textObj)
    {
        if (textObj is null || !textObj.TryGetComponent<PlayerMainStatElement>(out var textObjElement))
        {
            Debug.Log("main stat tooltip에 대한 잘못된 접근.");
            return;
        }

        Vector3 pos = textObj.GetComponent<RectTransform>().position;
        pos += new Vector3(TOOLTIP_X_POSITION_CORRECTION, TOOLTIP_Y_POSITION_CORRECTION, 0);
        GetComponent<RectTransform>().position = pos;

        _tooltipText.GetComponent<TextMeshProUGUI>().text = textObjElement.statName;

        OpenUI();
    }
}
