using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMainStatTooltip : UIElement
{
    [SerializeField] private GameObject _tooltipText;

    private const int TOOLTIP_X_POSITION_CORRECTION = 0;
    private const int TOOLTIP_Y_POSITION_CORRECTION = 10;
    public void SetPlayerMainStatTooltip(GameObject textObj)
    {
        if (textObj is null)
        {
            Debug.Log("Wrong access for main stat tooltip");
            return;
        }
        string str = "";
        if (textObj.TryGetComponent<PlayerMainStatElement>(out var textObjElement)) 
        {
            str = UIManager.instance.statScript.GetStatScript(textObjElement.statName).name;
        }
        else if (textObj.TryGetComponent<PlayerHpUI>(out var hp))
        {
            str = UIManager.instance.UILocalization[26];
        }
        else if (textObj.TryGetComponent<PlayerApUI>(out var ap))
        {
            str = UIManager.instance.UILocalization[27];
        }
        else if (textObj.TryGetComponent<PlayerMagazineUI>(out var mag))
        {
            str = UIManager.instance.UILocalization[28];
        }
        else if (textObj.TryGetComponent<PlayerConcentrationUI>(out var conc))
        {
            str = UIManager.instance.UILocalization[29];
        }
        else
        {
            Debug.LogError("Can't Find Text for main stat tooltip");
            return;
        }

        Vector3 pos = textObj.GetComponent<RectTransform>().position;
        pos += new Vector3(TOOLTIP_X_POSITION_CORRECTION, textObj.GetComponent<RectTransform>().sizeDelta.y / 2 + TOOLTIP_Y_POSITION_CORRECTION, 0) * UIManager.instance.GetCanvasScale();
        GetComponent<RectTransform>().position = pos;

        _tooltipText.GetComponent<TextMeshProUGUI>().text = str;

        OpenUI();
    }
}
