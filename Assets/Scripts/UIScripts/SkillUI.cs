using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SkillUI : MonoBehaviour
{
    public enum LearnState
    {
        NotLearned,
        Learnable,
        AlreadyLearned
    };

    [SerializeField] private SkillManager _skillManager;
    [SerializeField] private GameObject _skillWindow;
    [SerializeField] private GameObject _skillUIButtons;
    [SerializeField] private GameObject _skillTooltipWindow;
    [SerializeField] private GameObject _skillPointText;
    private int _currentSkillIndex;

    void Start()
    {
        //GetComponent<Image>().sprite = ;
        UpdateSkillUIImage();
    }
    public void ClickSkillUIButton(Transform _transform, int btnIndex)
    {
        _skillTooltipWindow.transform.position = _transform.position;
        SetTooltipWindow(btnIndex);
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
                buttonText.text = "����";
            }
            else
            {
                buttonText.text = "��ų ����Ʈ ����";
            }
        }
        else
        {
            if (currentSkill.isLearned)
            {
                buttonText.text = "���� �Ϸ�";
            }
            else
            {
                buttonText.text = "���� �Ұ�";
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

            LearnState state = LearnState.NotLearned;
            if (_skill.isLearned)
            {
                state = LearnState.AlreadyLearned;
            }
            if (_skill.isLearnable)
            {
                state = LearnState.Learnable;
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
