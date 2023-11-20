using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuffTooltip : UIElement
{
    string _buffName = "";
    string _buffDesc = "";

    void Start()
    {
        CloseUI();
    }

    public void SetBuffTooltip(IDisplayableEffect effect, Vector3 pos)
    {
        OpenUI();

        if (effect is StatusEffect)
        {
            SetDebuffText(effect);
        }
        else 
        {
            SetBuffText(effect);
        }

        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _buffName;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _buffDesc;

        GetComponent<RectTransform>().position = pos;
    }
    private void SetBuffText(IDisplayableEffect effect)
    {
        _buffName = SkillManager.instance.GetSkillName(effect.GetIndex());
        _buffDesc = SkillManager.instance.GetSkillDescription(effect.GetIndex());
    }
    private void SetDebuffText(IDisplayableEffect effect)
    {
        KeywordScript kw = SkillManager.instance.GetSkillKeyword(effect.GetIndex());
        _buffName = kw.name;
        _buffDesc = kw.description;
    }
}
