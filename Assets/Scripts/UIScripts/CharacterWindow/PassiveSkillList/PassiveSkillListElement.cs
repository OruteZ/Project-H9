using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PassiveSkillListElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _skillIcon;
    [SerializeField] private GameObject _skillName;

    [SerializeField] private int _skillIndex;

    public void SetPassiveSkillListElement(PassiveInfo info, int order)
    {
        if (isOpenUI) return;
        OpenUI();
        GetComponent<RectTransform>().localPosition = new Vector3(5, -35 - (GetComponent<RectTransform>().sizeDelta.y + 5) * order, 0);

        _skillIndex = info.index;
        _skillIcon.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo(SkillManager.instance.GetSkill(_skillIndex).skillInfo.icon);
        _skillName.GetComponent<TextMeshProUGUI>().text = SkillManager.instance.GetSkillName(info.index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 pos = GetComponent<RectTransform>().position;
        pos.y -= GetComponent<RectTransform>().sizeDelta.y / 2;
        UIManager.instance.characterUI.passiveSkillListUI.OpenPassiveSkillListTooltip(_skillIndex, pos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.characterUI.passiveSkillListUI.ClosePassiveSkillListTooltip();
    }
}
