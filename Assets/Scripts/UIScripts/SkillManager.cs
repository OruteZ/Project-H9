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

public class Skill
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
    public int upgradedSkill { get; private set; }
    public int[] amount { get; private set; }
    public int range { get; private set; }
    public int width { get; private set; }

    private bool isLearned;
    private bool isLearnable;
    private int skillLevel;

    public Skill(List<string> _list) 
    {
        InitInformationValues(_list);
        InitStatusValues();
    }
    private void InitInformationValues(List<string> _list) 
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
        upgradedSkill = int.Parse(_list[10]);
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

    private void InitStatusValues()
    {
        isLearned = false;
        skillLevel = 0;
        InitIsLearnable();
    }
    private void InitIsLearnable()
    {
        if (precedenceIndex[0] == 0) isLearnable = true;
        else isLearnable = false;
    }


    public bool IsActive() 
    {
        if (activeOrPassive.Equals(SkillActiveOrPassive.ACTIVE)) 
        {
            return true;
        }
        return false;
    }

    public bool GetIsLearned()
    {
        return isLearned;
    }
    public bool GetIsLearnable()
    {
        return isLearnable;
    }
    public void UpdateIsLearnable(List<Skill> _skills)
    {

        if (skillLevel >= repeatCount)
        {
            isLearnable = false;
            return;
        }

        int cnt = 0;
        for (int i = 0; i < _skills.Count; i++) 
        {
            for (int j = 0; j < precedenceIndex.Length; j++) 
            {
                bool isPrecedSkill = (_skills[i].index == precedenceIndex[j]);
                bool isLearnedPrecedSkill = (_skills[i].GetIsLearned() == true);
                if (isPrecedSkill && isLearnedPrecedSkill) 
                {
                    cnt++;
                } 
            }
        }

        if (cnt == precedenceIndex.Length) isLearnable = true;
    }

    public void LearnSkill()
    {
        if (skillLevel >= repeatCount) { Debug.Log("스킬 레벨 비정상적 상승"); return; }
        skillLevel++;
        isLearned = true;
        if (skillLevel >= repeatCount)
        {
            isLearnable = false;
        }

    }
}

public class SkillManager : MonoBehaviour
{
    public const int REQUIRED_SKILL_POINT = 1;

    private List<List<string>> skillTable;
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

        skills = new List<Skill>();
        for (int i = 0; i < skillTable.Count; i++)
        {
            Skill skill = new Skill(skillTable[i]);
            skills.Add(skill);
        }

        //for (int i = 0; i < skills.Count; i++)
        //{
        //    Debug.Log(skills[i].GetIndex());
        //}
    }

    public List<Skill> GetAllSkills() 
    {
        return skills;
    }
    public Skill GetSkill(int index) 
    {
        for (int i = 0; i < skills.Count; i++) 
        {
            if (skills[i].index == index) 
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
            if (skills[i].index == index) 
            {
                if (!skills[i].GetIsLearnable()) { Debug.Log("습득 조건이 충족되지 않은 스킬입니다."); return false; }
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
