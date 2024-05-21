using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatLevelUpTooltip : UIElement
{
    [SerializeField] private GameObject _tooltipNameText;
    [SerializeField] private GameObject _tooltipDescText;
    public string nameText { get; private set; }

    private const int STAT_TOOLTIP_YPOSITION_CORRECTION = 25;
    void Start()
    {
        nameText = "";
        CloseUI();
    }
    public void SetStatLevelUpTooltip(PlayerStatLevelInfo info, Vector3 pos)
    {
        if (nameText == info.statName) return;

        //Tooltip setting
        OpenUI();
        nameText = info.statName;
        _tooltipNameText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.statScript.GetStatScript(info.statName).name;
        _tooltipDescText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.statScript.GetStatScript(info.statName).description;
        pos.y -= STAT_TOOLTIP_YPOSITION_CORRECTION * UIManager.instance.GetCanvasScale();
        GetComponent<RectTransform>().position = pos;
    }
}
