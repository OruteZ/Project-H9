using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Generic.Singleton<SkillManager>
{
    const int REQUIRED_SKILL_POINT = 1;

    private List<Skill> _skills;

    private int _skillPoint;

    private new void Awake()
    {
        base.Awake();
        
        InitSkills();
        _skillPoint = 10;    //test
    }

    void InitSkills()
    {
        List<List<string>> _skillTable = FileRead.Read("SkillTable");
        if (_skillTable == null)
        {
            Debug.Log("skill table을 읽어오지 못했습니다.");
            return;
        }

        List<SkillInfo> _skillInformations = new List<SkillInfo>();
        for (int i = 0; i < _skillTable.Count; i++)
        {
            SkillInfo _skillInfo = new SkillInfo(_skillTable[i]);
            _skillInformations.Add(_skillInfo);
        }

        _skills = new List<Skill>();
        for (int i = 0; i < _skillInformations.Count; i++)
        {
            Skill _skill = new Skill(_skillInformations[i]);
            _skills.Add(_skill);
        }
    }

    public List<Skill> GetAllSkills() 
    {
        return _skills;
    }
    public List<Skill> GetAllLearnedSkills()
    {
        List<Skill> learnedSkills = new List<Skill>();
        for (int i = 0; i < _skills.Count; i++)
        {
            if (!_skills[i].isLearned) continue;
            learnedSkills.Add(_skills[i]);
        }
        return learnedSkills;
    }
    public Skill GetSkill(int index) 
    {
        for (int i = 0; i < _skills.Count; i++) 
        {
            if (_skills[i].skillInfo.index == index) 
            {
                return _skills[i];
            }
        }
        Debug.Log("해당 인덱스의 스킬을 찾지 못했습니다. 인덱스: " + index);
        return null;
    }

    public bool LearnSkill(int index) 
    {
        for (int i = 0; i < _skills.Count; i++) 
        {
            if (_skills[i].skillInfo.index == index) 
            {
                if (!_skills[i].isLearnable) { Debug.Log("습득 조건이 충족되지 않은 스킬입니다."); return false; }
                if (_skillPoint < REQUIRED_SKILL_POINT) { Debug.Log("스킬 포인트가 부족합니다."); return false; }

                _skillPoint -= REQUIRED_SKILL_POINT;
                _skills[i].LearnSkill();
                break;
            }
        }
        for (int i = 0; i < _skills.Count; i++)
        {
            _skills[i].UpdateIsLearnable(_skills);
        }

        return true;
    }

    public int GetSkillPoint() 
    {
        return _skillPoint;
    }
    public bool IsEnoughSkillPoint()
    {
        return REQUIRED_SKILL_POINT <= _skillPoint;
    }
}
