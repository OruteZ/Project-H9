using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassiveSkillListTooltip : UIElement
{
    private string _skillName = "";
    private string _skillDesc = "";
    public PassiveInfo currentPassiveInfo { get; private set; }

    void Start()
    {
        CloseUI();
    }
    public void SetPassiveSkillListTooltip(int index, Vector3 pos)
    {
        OpenUI();

        //PassiveInfo info = SkillManager.instance.passiveDB.GetPassiveInfo(index);
        //currentPassiveInfo = info;
        _skillName = SkillManager.instance.GetSkillName(index);
        _skillDesc = SkillManager.instance.GetSkillDescription(index, out var k);

        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _skillName;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _skillDesc;

        GetComponent<RectTransform>().position = pos;
    }
}
