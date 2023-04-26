using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    private int index;
    private string icon;
    private string name;
    private string description;
    private int category;
    private int aop;    //aop??
    private int[] preced;
    private int repeat;
    private int[] stat;
    private int activeType;
    private int upgSkill;
    private int[] amount;
    private int range;
    private int width;

    private bool isLearned;
    private bool isLearnable;
    private int skillLevel;

    public Skill(List<string> _list) 
    {
        InitInfomationValues(_list);
        InitStatusValues();
    }
    private void InitInfomationValues(List<string> _list) 
    {
        for (int i = 0; i < _list.Count; i++) 
        {
            if (_list[i].Equals("")) 
            {
                _list[i] = "0";
            }
        }

        index = int.Parse(_list[0]);
        icon = _list[1];
        name = _list[2];
        description = _list[3];
        category = int.Parse(_list[4]);
        aop = int.Parse(_list[5]);
        preced = InitIntArrayValue(_list[6]);
        repeat = int.Parse(_list[7]);
        stat = InitIntArrayValue(_list[8]);
        activeType = int.Parse(_list[9]);
        upgSkill = int.Parse(_list[10]);
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
        skillLevel = 1;
        InitIsLearnable();
    }



    public int GetIndex() 
    {
        return index;
    }
    public string GetName()
    {
        return name;
    }
    public string GetDescription()
    {
        return description;
    }
    public int GetAoP() 
    {
        return aop;
    }

    public bool GetIsLearned()
    {
        return isLearned;
    }
    public bool GetIsLearnable()
    {
        return isLearnable;
    }
    public bool IsEnoughSkillPoint(int point) 
    {
        return point >= aop;
    }
    private void InitIsLearnable() 
    {
        if (preced[0] == 0) isLearnable = true;
        else isLearnable = false;
    }
    public void UpdateIsLearnable(List<Skill> _skills)
    {
        if (isLearned) 
        {
            if (skillLevel > repeat)
            {
                isLearnable = false;
                return;
            }
        }

        int cnt = 0;
        for (int i = 0; i < _skills.Count; i++) 
        {
            for (int j = 0; j < preced.Length; j++) 
            {
                bool isPrecedSkill = (_skills[i].GetIndex() == preced[j]);
                bool isLearnedPrecedSkill = (_skills[i].GetIsLearned() == true);
                if (isPrecedSkill && isLearnedPrecedSkill) 
                {
                    cnt++;
                } 
            }
        }

        if (cnt == preced.Length) isLearnable = true;
    }

    public void LearnSkill() 
    {
        isLearned = true;
    }
    public void LevelUpSkill()
    {
        if (skillLevel > repeat) { Debug.Log("스킬 레벨 비정상적 상승"); return; }
        skillLevel++;
    }
}

public class SkillManager : MonoBehaviour
{
    private List<List<string>> skillTable;
    private List<Skill> skills;

    private int skillPoint;

    private void Awake()
    {
        InitSkills();
        skillPoint = 10;
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
            if (skills[i].GetIndex() == index) 
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
            if (skills[i].GetIndex() == index) 
            {
                if (!skills[i].GetIsLearnable()) { Debug.Log("습득 조건이 충족되지 않은 스킬입니다."); return false; }
                if (skillPoint < skills[i].GetAoP()) { Debug.Log("스킬 포인트가 부족합니다."); return false; }

                skillPoint -= skills[i].GetAoP();
                if (skills[i].GetIsLearned())
                {
                    skills[i].LevelUpSkill();
                }
                else 
                {
                    skills[i].LearnSkill();
                }
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
}
