using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatActionSkillTooltip : UIElement
{
    [SerializeField] private GameObject _skillNameText;
    [SerializeField] private GameObject _skillDescriptionText;
    [SerializeField] private GameObject _skillCostUI;
    [SerializeField] private GameObject _skillKeywords;
    [SerializeField] private GameObject _skillSectionIcon;
    [SerializeField] private GameObject _skillSectionText;

    private SkillInfo _currentSkillInfo = null;
    static private SkillKeywordPool _keywordPool = null;
    // Start is called before the first frame update
    void Awake()
    {

        if (_keywordPool == null)
        {
            _keywordPool = new SkillKeywordPool();
            _keywordPool.Init("Prefab/Keyword Tooltip", _skillKeywords.transform, 0);
        }
    }

    public void SetCombatSkillTooltip(GameObject btn)
    {
        int btnIdx = btn.GetComponent<CombatActionButtonElement>().buttonIndex;
        IUnitAction action = btn.GetComponent<CombatActionButtonElement>().buttonAction;
        SkillInfo info = SkillManager.instance.GetSkill(GameManager.instance.playerActiveIndexList[btnIdx]).skillInfo;
        if (info is null) 
        {
            Debug.LogError("저장된 action이 null이라 툴팁을 구성할 수 없습니다.");
            return;
        }
        _currentSkillInfo = info;

        string name = SkillManager.instance.GetSkillName(info.index);
        string desc = SkillManager.instance.GetSkillDescription(info.index, out var keywords);
        _skillNameText.GetComponent<TextMeshProUGUI>().text = name;
        _skillNameText.GetComponent<TextMeshProUGUI>().color = UICustomColor.UINameColor;
        _skillDescriptionText.GetComponent<TextMeshProUGUI>().text = desc;

        ItemType sectionType = ItemType.Etc + info.section;
        bool isThereSectionCondition = (ItemType.Revolver <= sectionType && sectionType <= ItemType.Shotgun);
        _skillSectionIcon.SetActive(isThereSectionCondition);
        _skillSectionText.SetActive(isThereSectionCondition);
        if (isThereSectionCondition)
        {
            _skillSectionIcon.GetComponent<Image>().sprite = UIManager.instance.iconDB.GetIconInfo(sectionType.ToString());
            _skillSectionText.GetComponent<TextMeshProUGUI>().text = sectionType.ToString();
        }


        ActiveInfo aInfo = SkillManager.instance.activeDB.GetActiveInfo(info.index);
        _skillCostUI.GetComponent<TooltipCostUI>().SetTooltipCostUI(action.GetCost(), action.GetAmmoCost(), false);

        _keywordPool.Reset();
        if (keywords is not null)
        {
            for (int i = 0; i < keywords.Count; i++)
            {
                KeywordScript kw = SkillManager.instance.GetSkillKeyword(keywords[i]);
                SkillKeywordWrapper t = _keywordPool.Set();
                t.tooltip.SetSkillKeywordTooltip(kw.name, kw.GetDescription(), i);
            }
        }
        OpenUI();
    }
}
