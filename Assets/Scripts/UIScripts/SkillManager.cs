using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SkillCategory
{
    NULL,
    CHARACTER,
    REVOLVER,
    REPEATER,
    SHOTGUN
}
enum SkillActiveOrPassive 
{
    NULL,
    ACTIVE,
    PASSIVE
}
enum Stat
{
    NULL,
    HP,
    CONCENTRATION,
    SPEED,
    FINAL_HIT_RATE,
    SIGHT_DISTANCE,
    FINAL_MOBILITY,
    FINAL_DAMAGE_PERCENT,
    FINAL_DAMAGE_INTEGER,
    HP_RECOVERY,
    ACTIVE_SKILL_DAMAGE_INTEGER,
    SKILL_DISTANCE,
    SKILL_USE_COUNT,
    UP_HIT_RATE,
    UP_DISTANCE,
    SKILL_DAMAGE_INTEGER
}
enum SkillActiveType 
{
    NULL,
    DAMAGE,
    EXPLODE,
    HEAL
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

    public SkillInfo(List<string> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].Equals(""))
            {
                _list[i] = "0";
            }
        }

        index = int.Parse(_list[0]);
        iconNumber = int.Parse(_list[1]);
        name = _list[2];
        description = _list[3];
        category = int.Parse(_list[4]);
        activeOrPassive = int.Parse(_list[5]);
        precedenceIndex = InitIntArrayValue(_list[6]);
        repeatCount = int.Parse(_list[7]);
        if (repeatCount == 0) repeatCount = 1;  //table issue
        stat = InitIntArrayValue(_list[8]);
        activeType = int.Parse(_list[9]);
        upgradeSkill = int.Parse(_list[10]);
        amount = InitIntArrayValue(_list[11]);
        range = int.Parse(_list[12]);
        width = int.Parse(_list[13]);
    }
    private int[] InitIntArrayValue(string _string) 
    {
        char SPLIT_CHAR = '$';
        if (_string.Equals("0")) return new int[] { 0 };

        string[] splitString = _string.Split(SPLIT_CHAR);

        int[] result = new int[splitString.Length];

        for (int i = 0; i < splitString.Length; i++) 
        {
            result[i] = int.Parse(splitString[i]);
        }

        return result;
    }

    public bool IsActive() 
    {
        if (activeOrPassive.Equals(SkillActiveOrPassive.ACTIVE)) 
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


    public Skill(SkillInfo _skillInfo) 
    {
        skillInfo = _skillInfo;

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
    public void UpdateIsLearnable(List<Skill> _skills)
    {

        if (skillLevel >= skillInfo.repeatCount)
        {
            isLearnable = false;
            return;
        }

        CheckPrecedenceSkill(_skills);
        if (IsLearnedAllPrecedenceSkills()) isLearnable = true;
    }
    private void CheckPrecedenceSkill(List<Skill> _skills)
    {
        for (int i = 0; i < _skills.Count; i++)
        {
            for (int j = 0; j < skillInfo.precedenceIndex.Length; j++)
            {
                bool isPrecedenceSkill = (_skills[i].skillInfo.index == skillInfo.precedenceIndex[j]);
                if (isPrecedenceSkill)
                {
                    isLearnedPrecedeSkill[j] = _skills[i].isLearned;
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

public class SkillManager : MonoBehaviour
{
    const int REQUIRED_SKILL_POINT = 1;

    private List<List<string>> skillTable;
    private List<SkillInfo> skillInformations;
    private List<Skill> skills;

    private int skillPoint;

    private void Awake()
    {
        InitSkills();
        skillPoint = 10;    //test
    }

    void InitSkills()
    {
        skillTable = SkillRead.Read("SkillTable");
        if (skillTable == null)
        {
            Debug.Log("skill table을 읽어오지 못했습니다.");
        }

        skillInformations = new List<SkillInfo>();
        for (int i = 0; i < skillTable.Count; i++)
        {
            SkillInfo _skillInfo = new SkillInfo(skillTable[i]);
            skillInformations.Add(_skillInfo);
        }

        skills = new List<Skill>();
        for (int i = 0; i < skillInformations.Count; i++)
        {
            Skill _skill = new Skill(skillInformations[i]);
            skills.Add(_skill);
        }
    }

    public List<Skill> GetAllSkills() 
    {
        return skills;
    }
    public Skill GetSkill(int index) 
    {
        for (int i = 0; i < skills.Count; i++) 
        {
            if (skills[i].skillInfo.index == index) 
            {
                return skills[i];
            }
        }
        Debug.Log("해당 인덱스의 스킬을 찾지 못했습니다. 인덱스: " + index);
        return null;
    }

    public bool LearnSkill(int index) 
    {
        for (int i = 0; i < skills.Count; i++) 
        {
            if (skills[i].skillInfo.index == index) 
            {
                if (!skills[i].isLearnable) { Debug.Log("습득 조건이 충족되지 않은 스킬입니다."); return false; }
                if (skillPoint < REQUIRED_SKILL_POINT) { Debug.Log("스킬 포인트가 부족합니다."); return false; }

                skillPoint -= REQUIRED_SKILL_POINT;
                skills[i].LearnSkill();
                break;
            }
        }
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].UpdateIsLearnable(skills);
        }

        return true;
    }

    public int GetSkillPoint() 
    {
        return skillPoint;
    }
    public bool IsEnoughSkillPoint()
    {
        return REQUIRED_SKILL_POINT <= skillPoint;
    }
}
