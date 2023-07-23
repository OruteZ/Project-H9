using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SkillUI : UISystem
{
    public enum LearnStatus
    {
        NotLearned,
        Learnable,
        AlreadyLearned
    };

    private SkillManager _skillManager;
    private int _currentSkillIndex;

    [Header("Skill UIs")]
    [SerializeField] private GameObject _skillWindow;//?
    [SerializeField] private GameObject _skillUIButtons;
    [SerializeField] private GameObject _skillTooltipWindow;
    [SerializeField] private GameObject _skillPointText;

    void Start()
    {
        _skillManager = SkillManager.instance;
        //GetComponent<Image>().sprite = ;
        UpdateSkillUIImage();
    }
    public override void OpenUI() 
    {
        base.OpenUI();
        UpdateSkillPointUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        CloseSkillTooltip();
    }
    public override void ClosePopupWindow()
    {
        UIManager.instance.previousLayer = 2;
        CloseSkillTooltip();
    }
    public void ClickSkillUIButton(Transform _transform, int btnIndex)
    {
        _skillTooltipWindow.transform.position = _transform.position;
        SetTooltipWindow(btnIndex);

        UIManager.instance.previousLayer = 3;
        _skillTooltipWindow.SetActive(true);
    }
    private void SetTooltipWindow(int index)
    {
        Skill currentSkill = _skillManager.GetSkill(index);
        _currentSkillIndex = index;
        _skillTooltipWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentSkill.skillInfo.name;
        _skillTooltipWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentSkill.skillInfo.description;

        TextMeshProUGUI buttonText = _skillTooltipWindow.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
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
    public void CloseSkillTooltip()
    {
        _skillTooltipWindow.SetActive(false);
    }
    public void ClickLearnSkill()
    {
        Debug.Log(_currentSkillIndex);
        if (_skillManager.LearnSkill(_currentSkillIndex))
        {
            UpdateSkillUIImage();
        }
        SetTooltipWindow(_currentSkillIndex);
    }
    private void UpdateSkillUIImage()
    {
        UpdateSkillPointUI();
        for (int i = 0; i < _skillUIButtons.transform.childCount; i++)
        {
            SkillTreeElement _skillElement = _skillUIButtons.transform.GetChild(i).GetComponent<SkillTreeElement>();
            Skill _skill = _skillManager.GetSkill(_skillElement.GetSkillUIIndex());

            LearnStatus state = LearnStatus.NotLearned;
            if (_skill.isLearned)
            {
                state = LearnStatus.AlreadyLearned;
            }
            if (_skill.isLearnable)
            {
                state = LearnStatus.Learnable;
            }
            _skillElement.SetSkillButtonEffect((int)state);

            if (_skill.skillLevel > 0)
            {
                _skillElement.SetSkillArrow();
            }
        }
    }
    private void UpdateSkillPointUI()
    {
        _skillPointText.GetComponent<TextMeshProUGUI>().text = "SP: " + _skillManager.GetSkillPoint().ToString();
    }
}
