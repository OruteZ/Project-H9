using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SkillTooltip : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _skillTooltipNameText;
    [SerializeField] private GameObject _skillTooltipDescriptionText;
    [SerializeField] private GameObject _skillTooltipButtonText;
    [SerializeField] private GameObject _skillKeywordTooltips;

    private SkillManager _skillManager;
    private int _currentSkillIndex;

    // Start is called before the first frame update
    void Start()
    {
        _skillManager = SkillManager.instance;
    }

    public void SetSkillTooltip(Vector3 pos, int skillIndex) 
    {
        GetComponent<RectTransform>().position = pos;
        Skill currentSkill = _skillManager.GetSkill(skillIndex);

        _currentSkillIndex = skillIndex;
        _skillTooltipNameText.GetComponent<TextMeshProUGUI>().text = SkillManager.instance.GetSkillName(_currentSkillIndex);
        _skillTooltipDescriptionText.GetComponent<TextMeshProUGUI>().text = SkillManager.instance.GetSkillDescription(_currentSkillIndex);

        TextMeshProUGUI buttonText = _skillTooltipButtonText.GetComponent<TextMeshProUGUI>();
        if (currentSkill.isLearnable)
        {
            if (_skillManager.IsEnoughSkillPoint())
            {
                buttonText.text = "습득";
            }
            else
            {
                buttonText.text = "스킬 포인트 부족";
            }
        }
        else
        {
            if (currentSkill.isLearned)
            {
                buttonText.text = "습득 완료";
            }
            else
            {
                buttonText.text = "습득 불가";
            }
        }
    }

    /// <summary>
    /// 해당 스킬을 습득하는 것이 가능하다면 SkillManager를 통해 스킬을 습득합니다.
    /// 스킬 툴팁창의 스킬습득버튼을 클릭할 시 실행됩니다.
    /// </summary>
    public void ClickLearnSkill()
    {
        if (_skillManager.LearnSkill(_currentSkillIndex))
        {
            UIManager.instance.skillUI.UpdateSkillUIImage();
        }
        SetSkillTooltip(GetComponent<RectTransform>().position, _currentSkillIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //기획 보고 Set 함수 호출 위치 체크해야 함.
        _skillKeywordTooltips.SetActive(true);
        return;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _skillKeywordTooltips.GetComponent<SkillKeywordTooltip>().CloseUI();
    }
}
