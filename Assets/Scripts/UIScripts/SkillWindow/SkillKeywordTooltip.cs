using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillKeywordTooltip : UIElement
{
    [SerializeField] private GameObject _keywordNameText;
    [SerializeField] private GameObject _keywordDescriptionText;

    public void SetSkillKeywordTooltip(int index, string name, string description)
    {
        if (isOpenUI) return;
        OpenUI();
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 pos = Vector3.zero;
        pos.y = (rt.sizeDelta.y + 10) * index;
        GetComponent<RectTransform>().localPosition = pos;

        _keywordNameText.GetComponent<TextMeshProUGUI>().text = name;
        _keywordDescriptionText.GetComponent<TextMeshProUGUI>().text = description;
    }
}