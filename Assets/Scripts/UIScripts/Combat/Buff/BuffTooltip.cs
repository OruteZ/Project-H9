using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuffTooltip : UIElement
{
    private string _buffName = "";
    private string _buffDesc = "";
    public IDisplayableEffect currentTooltipEffect { get; private set; }

    void Start()
    {
        CloseUI();
    }

    public void SetBuffTooltip(IDisplayableEffect effect, Vector3 pos)
    {

        currentTooltipEffect = effect;
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
        OpenUI();
    }
    private void SetBuffText(IDisplayableEffect effect)
    {
        _buffName = SkillManager.instance.GetSkillName(effect.GetIndex());
        _buffDesc = SkillManager.instance.GetSkillDescription(effect.GetIndex(), out var k);
    }
    private void SetDebuffText(IDisplayableEffect effect)
    {
        KeywordScript kw = SkillManager.instance.GetSkillKeyword(effect.GetIndex());
        if (kw == null) return;
        _buffName = kw.name;
        _buffDesc = kw.GetDescription();
    }
}
