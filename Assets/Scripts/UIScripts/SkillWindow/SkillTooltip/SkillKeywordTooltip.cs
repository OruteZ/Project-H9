using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillKeywordTooltip : UIElement
{
    [SerializeField] private GameObject _keywordNameText;
    [SerializeField] private GameObject _keywordDescriptionText;

    public void SetSkillKeywordTooltip(string name, string description, int order)
    {
        if (isOpenUI) return;
        OpenUI();
        GetComponent<RectTransform>().localPosition = Vector3.zero;

        _keywordNameText.GetComponent<TextMeshProUGUI>().text = name;
        _keywordNameText.GetComponent<TextMeshProUGUI>().color = UICustomColor.UINameColor;
        _keywordDescriptionText.GetComponent<TextMeshProUGUI>().text = description;
        _keywordDescriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        GetComponent<ContentSizeFitter>().SetLayoutVertical();
    }
}
