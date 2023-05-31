using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SkillCategory
{
    Null,
    Character,
    Revoler,
    Repeater,
    Shotgun
}
enum SkillActiveOrPassive 
{
    Null,
    Active,
    Passive
}
enum Stat
{
    Null,
    HP,
    Concentration,
    Speed,
    FinalHitRate,
    Sight_Distance,
    Final_Mobility,
    FinalDamagePercent,
    FinalDamageInteger,
    HPRecovery,
    ActiveSkillDamageInteger,
    SkillDistance,
    SkillUseCount,
    UpHitRate,
    UpDistance,
    SkillDamageInteger
}
enum SkillActiveType 
{
    Null,
    Damage,
    Explode,
    Heal
}

public class SkillInfo
{
    public int index { get; private set; }
    public int iconNumber { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }
    public int category { get; private set; }

    private int activeOrPassive;
    public int[] precedenceIndex { get; private set; }
    public int repeatCount { get; private set; }
    public int[] stat { get; private set; }
    public int activeType { get; private set; }
    public int upgradeSkill { get; private set; }
    public int[] amount { get; private set; }
    public int range { get; private set; }
    public int width { get; private set; }

    public SkillInfo(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(""))
            {
                list[i] = "0";
            }
        }

        index = int.Parse(list[0]);
        iconNumber = int.Parse(list[1]);
        name = list[2];
        description = list[3];
        category = int.Parse(list[4]);
        activeOrPassive = int.Parse(list[5]);
        precedenceIndex = InitIntArrayValue(list[6]);
        repeatCount = int.Parse(list[7]);
        if (repeatCount == 0) repeatCount = 1;  //table issue
        stat = InitIntArrayValue(list[8]);
        activeType = int.Parse(list[9]);
        upgradeSkill = int.Parse(list[10]);
        amount = InitIntArrayValue(list[11]);
        range = int.Parse(list[12]);
        width = int.Parse(list[13]);
    }
    private int[] InitIntArrayValue(string str) 
    {
        const char SPLIT_CHAR = '$';
        if (str.Equals("0")) return new int[] { 0 };

        string[] splitString = str.Split(SPLIT_CHAR);

        int[] result = new int[splitString.Length];

        for (int i = 0; i < splitString.Length; i++) 
        {
            result[i] = int.Parse(splitString[i]);
        }

        return result;
    }

    public bool IsActive() 
    {
        if (activeOrPassive.Equals(SkillActiveOrPassive.Active)) 
        {
            return true;
        }
        return false;
    }
}

public class Skill
{
    public SkillInfo skillInfo { get; private set; }

    public bool isLearned { get; private set; }
    public bool isLearnable { get; private set; }
    public int skillLevel { get; private set; }
    public bool[] isLearnedPrecedeSkill { get; private set; }


    public Skill(SkillInfo info) 
    {
        this.skillInfo = info;

        isLearned = false;
        InitIsLearnable();
        skillLevel = 0;
        InitIsLearnedPrecedeSkill();
    }

    private void InitIsLearnable()
    {
        if (skillInfo.precedenceIndex[0] == 0) isLearnable = true;
        else isLearnable = false;
    }
    private void InitIsLearnedPrecedeSkill()
    {
        isLearnedPrecedeSkill = new bool[skillInfo.precedenceIndex.Length];
        for (int i = 0; i < isLearnedPrecedeSkill.Length; i++) 
        { 
            isLearnedPrecedeSkill[i] = false; 
        }
        if (skillInfo.precedenceIndex[0] == 0)
        {
            isLearnedPrecedeSkill[0] = true;
        }
    }
    public void UpdateIsLearnable(List<Skill> skills)
    {

        if (skillLevel >= skillInfo.repeatCount)
        {
            isLearnable = false;
            return;
        }

        CheckPrecedenceSkill(skills);
        if (IsLearnedAllPrecedenceSkills()) isLearnable = true;
    }
    private void CheckPrecedenceSkill(List<Skill> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            for (int j = 0; j < skillInfo.precedenceIndex.Length; j++)
            {
                bool isPrecedenceSkill = (skills[i].skillInfo.index == skillInfo.precedenceIndex[j]);
                if (isPrecedenceSkill)
                {
                    isLearnedPrecedeSkill[j] = skills[i].isLearned;
                }
            }
        }
    }
    private bool IsLearnedAllPrecedenceSkills()
    {
        for (int i = 0; i < isLearnedPrecedeSkill.Length; i++)
        {
            if (!isLearnedPrecedeSkill[i]) return false;
        }
        return true;
    }

    public void LearnSkill()
    {
        if (skillLevel >= skillInfo.repeatCount) { Debug.Log("스킬 레벨 비정상적 상승"); return; }
        skillLevel++;
        isLearned = true;
        if (skillLevel >= skillInfo.repeatCount)
        {
            isLearnable = false;
        }
    }
}

public class SkillManager : Generic.Singleton<SkillManager>
{
    const int REQUIRED_SKILL_POINT = 1;

    private List<List<string>> _skillTable;
    private List<SkillInfo> _skillInformations;
    private List<Skill> _skills;

    private int _skillPoint;

    private void Awake()
    {
        InitSkills();
        _skillPoint = 10;    //test
    }

    void InitSkills()
    {
        _skillTable = SkillRead.Read("SkillTable");
        if (_skillTable == null)
        {
            Debug.Log("skill table을 읽어오지 못했습니다.");
        }

        _skillInformations = new List<SkillInfo>();
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
